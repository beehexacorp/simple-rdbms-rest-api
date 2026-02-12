import { ref, computed, readonly, type Ref } from 'vue'
import type { SSOUser } from './types'
import { getAccessToken, hasTokens, clearTokens, getAuthHeaders } from './storage'
import { refreshTokens as refreshTokensFn } from './oauth'

const user: Ref<SSOUser | null> = ref(null)
const isLoading = ref(false)
const error = ref<string | null>(null)

export interface UseAuthOptions {
  apiBaseUrl?: string
  tokenEndpoint?: string
  userEndpoint?: string
  logoutEndpoint?: string
  clientId?: string
  fetchFn?: typeof fetch
  showMessage?: (message: string, type: 'success' | 'error') => void
}

function defaultNotification(message: string, type: 'success' | 'error') {
  if (type === 'success') console.log(message)
  else console.error(message)
}

function getEnvValue(key: string, fallback: string): string {
  if (typeof import.meta !== 'undefined' && (import.meta.env as any)?.[key]) {
    return (import.meta.env as any)[key] as string
  }
  return fallback
}

function getApiBaseUrl(options?: UseAuthOptions): string {
  return options?.apiBaseUrl || getEnvValue('VITE_API_URL', 'http://localhost:5000')
}

function getClientId(options?: UseAuthOptions): string {
  return options?.clientId || getEnvValue('VITE_SSO_CLIENT_ID', 'simple_rdbms_dashboard')
}

export function useAuth(options?: UseAuthOptions) {
  const apiBaseUrl = getApiBaseUrl(options)
  const clientId = getClientId(options)

  const tokenEndpoint = options?.tokenEndpoint || `${apiBaseUrl}/api/sso/v3/auth/token`
  const userEndpoint = options?.userEndpoint || `${apiBaseUrl}/api/sso/v3/auth/me`
  const logoutEndpoint = options?.logoutEndpoint || `${apiBaseUrl}/api/sso/v3/auth/revoke`

  const showMessage = options?.showMessage || defaultNotification
  const fetchFn = options?.fetchFn || fetch

  async function apiFetch(url: string, init?: RequestInit): Promise<Response> {
    const headers = {
      ...getAuthHeaders(),
      ...init?.headers,
    }

    return fetchFn(url, { ...init, headers })
  }

  async function refreshTokens(): Promise<boolean> {
    const tokens = await refreshTokensFn(tokenEndpoint, clientId)
    return tokens !== null
  }

  async function getSSOUser(): Promise<SSOUser | null> {
    isLoading.value = true
    error.value = null

    try {
      const response = await apiFetch(userEndpoint, { method: 'GET' })

      if (!response.ok) {
        if (response.status === 401) {
          const refreshed = await refreshTokens()
          if (!refreshed) {
            throw new Error('Session expired')
          }

          const retry = await apiFetch(userEndpoint, { method: 'GET' })
          if (!retry.ok) {
            throw new Error('Failed to get user info')
          }

          const userData = await retry.json()
          user.value = userData
          return userData
        }

        throw new Error('Failed to get user info')
      }

      const userData = await response.json()
      user.value = userData
      return userData
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to get user info'
      return null
    } finally {
      isLoading.value = false
    }
  }

  async function logout(): Promise<void> {
    try {
      const refreshToken = getStorage().getItem('refresh_token')
      if (refreshToken) {
        await fetchFn(logoutEndpoint, {
          method: 'POST',
          headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
          body: new URLSearchParams({ token: refreshToken }),
        })
      }
      showMessage('Logged out successfully', 'success')
    } catch {
      // ignore
    } finally {
      clearTokens()
      user.value = null
      showMessage('Logged out successfully', 'success')
      window.location.href = '/dashboard'
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

  return {
    user: readonly(user),
    isLoading: readonly(isLoading),
    error: readonly(error),
    isAuthenticated: computed(() => hasTokens()),

    getSSOUser,
    logout,

    getAccessToken,
    getAuthHeaders,
    hasTokens,

    apiFetch,
    refreshTokens,
  }
}
