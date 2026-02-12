import type { AuthGuardOptions } from './types'
import { generatePKCE } from './pkce'
import { storeCodeVerifier, hasTokens, getTokenExpiry, getRefreshToken, clearTokens } from './storage'

function getEnvValue(key: string, fallback: string): string {
  if (typeof import.meta !== 'undefined' && (import.meta.env as any)?.[key]) {
    return (import.meta.env as any)[key] as string
  }
  return fallback
}

function getApiBaseUrl(): string {
  return getEnvValue('VITE_API_URL', 'http://localhost:5000')
}

function getClientId(): string {
  return getEnvValue('VITE_SSO_CLIENT_ID', 'simple_rdbms_dashboard')
}

function isTokenHardExpired(): boolean {
  const expiryStr = getTokenExpiry()
  if (!expiryStr) return true
  return Date.now() > expiryStr
}

async function attemptTokenRefresh(): Promise<boolean> {
  const refreshToken = getRefreshToken()
  if (!refreshToken) return false

  try {
    const tokenEndpoint = `${getApiBaseUrl()}/api/sso/v3/auth/token`
    const clientId = getClientId()

    const response = await fetch(tokenEndpoint, {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body: new URLSearchParams({
        grant_type: 'refresh_token',
        refresh_token: refreshToken,
        client_id: clientId,
      }),
    })

    if (!response.ok) return false

    const tokens = await response.json()

    const storage = getStorage()
    const expiry = Date.now() + ((tokens.expires_in || 3600) * 1000)

    storage.setItem('access_token', tokens.access_token)
    storage.setItem('refresh_token', tokens.refresh_token || refreshToken)
    storage.setItem('token_expiry', expiry.toString())

    return true
  } catch {
    return false
  }
}

function getStorage(): Storage {
  if (typeof import.meta !== 'undefined' && (import.meta.env as any)?.USE_SESSION_STORAGE === 'true') {
    return sessionStorage
  }
  if (typeof window !== 'undefined' && (window as any).USE_SESSION_STORAGE === 'true') {
    return sessionStorage
  }
  return localStorage
}

export async function initiateSSOLogin(currentUrl: string, options?: Partial<AuthGuardOptions>): Promise<never> {
  const baseUrl = (import.meta.env?.BASE_URL as string) || '/'
  const normalizedBaseUrl = baseUrl.endsWith('/') ? baseUrl : `${baseUrl}/`

  const {
    ssoUrl = getEnvValue('VITE_SSO_PUBLIC_URL', 'https://sso.hexasync.com'),
    clientId = getEnvValue('VITE_SSO_CLIENT_ID', 'simple_rdbms_dashboard'),
    redirectUri =
      getEnvValue('VITE_SSO_REDIRECT_URI', '') || `${window.location.origin}${normalizedBaseUrl}sso/callback`,
  } = options || {}

  const { code_verifier, code_challenge } = await generatePKCE()
  storeCodeVerifier(code_verifier)

  const loginUrl = new URL('/login', ssoUrl)
  loginUrl.searchParams.set('client_id', clientId)
  loginUrl.searchParams.set('redirect_uri', redirectUri)
  loginUrl.searchParams.set('state', btoa(currentUrl))
  loginUrl.searchParams.set('code_challenge', code_challenge)
  loginUrl.searchParams.set('code_challenge_method', 'S256')

  window.location.href = loginUrl.toString()
  throw new Error('Redirecting to SSO login')
}

export function authGuard(options?: Partial<AuthGuardOptions>) {
  return async (to: any, _from: any, next: (arg?: any) => void) => {
    if (to.path === '/sso/callback' || to.path === '/callback') {
      return next()
    }

    if (to.meta?.requiresAuth === false) {
      return next()
    }

    if (hasTokens() && !isTokenHardExpired()) {
      return next()
    }

    if (hasTokens() && isTokenHardExpired()) {
      const refreshed = await attemptTokenRefresh()
      if (refreshed) {
        return next()
      }
      clearTokens()
    }

    const currentUrl =
      typeof to?.fullPath === 'string' && to.fullPath.trim() !== ''
        ? to.fullPath
        : window.location.pathname + window.location.search

    try {
      await initiateSSOLogin(currentUrl, options)
    } catch {
      // swallow
    }

    return
  }
}
