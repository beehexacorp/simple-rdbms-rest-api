import type { StoredTokens, Tokens } from './types'

const ACCESS_TOKEN_KEY = 'access_token'
const REFRESH_TOKEN_KEY = 'refresh_token'
const TOKEN_EXPIRY_KEY = 'token_expiry'
const CODE_VERIFIER_KEY = 'pkce_code_verifier'

function getStorage(): Storage {
  if (typeof import.meta !== 'undefined' && import.meta.env?.USE_SESSION_STORAGE === 'true') {
    return sessionStorage
  }
  if (typeof window !== 'undefined' && (window as any).USE_SESSION_STORAGE === 'true') {
    return sessionStorage
  }
  return localStorage
}

export function storeTokens(tokens: Tokens): void {
  const expiry = Date.now() + tokens.expires_in * 1000
  const storage = getStorage()

  storage.setItem(ACCESS_TOKEN_KEY, tokens.access_token)
  storage.setItem(REFRESH_TOKEN_KEY, tokens.refresh_token)
  storage.setItem(TOKEN_EXPIRY_KEY, expiry.toString())
}

export function clearTokens(): void {
  const storage = getStorage()
  storage.removeItem(ACCESS_TOKEN_KEY)
  storage.removeItem(REFRESH_TOKEN_KEY)
  storage.removeItem(TOKEN_EXPIRY_KEY)
}

export function getTokens(): StoredTokens | null {
  const storage = getStorage()
  const accessToken = storage.getItem(ACCESS_TOKEN_KEY)
  const refreshToken = storage.getItem(REFRESH_TOKEN_KEY)
  const expiryStr = storage.getItem(TOKEN_EXPIRY_KEY)

  if (!accessToken || !refreshToken || !expiryStr) return null

  return {
    access_token: accessToken,
    refresh_token: refreshToken,
    token_expiry: parseInt(expiryStr, 10),
  }
}

export function hasTokens(): boolean {
  return !!getAccessToken()
}

export function getAccessToken(): string | null {
  const storage = getStorage()
  return storage.getItem(ACCESS_TOKEN_KEY)
}

export function getRefreshToken(): string | null {
  const storage = getStorage()
  return storage.getItem(REFRESH_TOKEN_KEY)
}

export function getTokenExpiry(): number {
  const storage = getStorage()
  const expiryStr = storage.getItem(TOKEN_EXPIRY_KEY)
  return expiryStr ? parseInt(expiryStr, 10) : 0
}

export function isTokenExpired(bufferSeconds: number = 60): boolean {
  const expiry = getTokenExpiry()
  if (!expiry) return true
  return Date.now() + bufferSeconds * 1000 > expiry
}

export function getAuthHeaders(): Record<string, string> {
  const token = getAccessToken()
  if (token && token.trim() !== '' && token !== 'undefined') {
    return {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    }
  }

  return { 'Content-Type': 'application/json' }
}

export function storeCodeVerifier(verifier: string): void {
  const storage = getStorage()
  storage.setItem(CODE_VERIFIER_KEY, verifier)
}

export function getCodeVerifier(): string | null {
  const storage = getStorage()
  return storage.getItem(CODE_VERIFIER_KEY)
}

export function clearCodeVerifier(): void {
  const storage = getStorage()
  storage.removeItem(CODE_VERIFIER_KEY)
}
