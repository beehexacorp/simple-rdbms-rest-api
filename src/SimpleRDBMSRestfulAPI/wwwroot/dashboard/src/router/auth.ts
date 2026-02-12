/**
 * Vue Router navigation guard for SSO v3.
 *
 * Mirrors cyberbot-v2's src/router/auth.ts, but adapted to:
 * - dashboard base path (/dashboard)
 * - callback route (/callback)
 */

import type { AuthGuardOptions } from '@/auth/types'
import { authGuard } from '@/auth/routerGuard'
import { refreshTokens } from '@/auth/oauth'
import {
  storeTokens,
  isTokenExpired,
  clearTokens,
  getAccessToken,
  getRefreshToken as getRefreshTokenFromStorage,
} from '@/auth/storage'

export * from '@/auth'

const SSO_URL = (import.meta.env?.VITE_SSO_PUBLIC_URL as string) || 'https://sso.hexasync.com'
const CLIENT_ID = (import.meta.env?.VITE_SSO_CLIENT_ID as string) || 'simple_rdbms_dashboard'
const API_URL = (import.meta.env?.VITE_API_URL as string) || 'http://localhost:5000'

// Existing backend token exchange endpoint (same pattern as cyberbot-v2)
const TOKEN_ENDPOINT = `${API_URL}/api/sso/v3/auth/token`

export async function proactiveTokenRefresh(): Promise<boolean> {
  if (getAccessToken() && !isTokenExpired(60)) {
    return true
  }

  const tokens = await refreshTokens(TOKEN_ENDPOINT, CLIENT_ID)
  if (tokens) {
    storeTokens({
      access_token: tokens.access_token,
      refresh_token: tokens.refresh_token,
      token_type: tokens.token_type || 'Bearer',
      expires_in: tokens.expires_in || 3600,
    })
    return true
  }

  return false
}

export async function handle401(): Promise<boolean> {
  const tokens = await refreshTokens(TOKEN_ENDPOINT, CLIENT_ID)

  if (tokens) {
    storeTokens({
      access_token: tokens.access_token,
      refresh_token: tokens.refresh_token,
      token_type: tokens.token_type || 'Bearer',
      expires_in: tokens.expires_in || 3600,
    })
    return true
  }

  clearTokens()
  window.location.href = '/dashboard'
  return false
}

export async function ssoLogout(): Promise<void> {
  const refreshToken = getRefreshToken()

  if (refreshToken) {
    try {
      await fetch(`${API_URL}/api/sso/v3/auth/revoke`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: new URLSearchParams({ token: refreshToken }),
      })
    } catch {
      // ignore
    }
  }

  clearTokens()

  const storage = getStorage()
  Object.keys(storage).forEach((key) => {
    if (key.startsWith('oauth_state_')) {
      storage.removeItem(key)
    }
  })
}

function getRefreshToken(): string | null {
  return getRefreshTokenFromStorage()
}

function getStorage(): Storage {
  if (typeof import.meta !== 'undefined' && import.meta.env?.USE_SESSION_STORAGE === 'true') {
    return sessionStorage
  }
  if (typeof window !== 'undefined' && (window as any).USE_SESSION_STORAGE === 'true') {
    return sessionStorage
  }
  return localStorage
}

export function createAuthGuard() {
  return authGuard({
    ssoUrl: SSO_URL,
    clientId: CLIENT_ID,
    // redirectUri computed in guard as ${window.location.origin}/dashboard/callback
  })
}

export function getAuthConfig(): Partial<AuthGuardOptions> {
  return {
    ssoUrl: SSO_URL,
    clientId: CLIENT_ID,
  }
}

export { TOKEN_ENDPOINT }
