import type { TokenBodyType } from './types'
import { storeTokens, clearTokens, getRefreshToken, getTokens } from './storage'

export type { TokenBodyType } from './types'

export function getTokenBodyType(): TokenBodyType {
  const envValue =
    (typeof import.meta !== 'undefined' && (import.meta.env as any)?.EXCHANGE_TOKEN_POST_BODY_TYPE) ||
    (typeof window !== 'undefined' && (window as any).EXCHANGE_TOKEN_POST_BODY_TYPE)

  if (envValue === 'urlencoded' || envValue === 'form') {
    return 'urlencoded'
  }

  return 'json'
}

export function buildTokenBody(
  data: Record<string, string | number>,
  bodyType: TokenBodyType,
): { body: string | URLSearchParams; headers: Record<string, string> } {
  if (bodyType === 'urlencoded') {
    return {
      body: new URLSearchParams(
        Object.entries(data).reduce((acc, [key, value]) => {
          acc[key] = String(value)
          return acc
        }, {} as Record<string, string>),
      ),
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    }
  }

  return {
    body: JSON.stringify(data),
    headers: { 'Content-Type': 'application/json' },
  }
}

export async function refreshTokens(tokenEndpoint: string, clientId: string, bodyType?: TokenBodyType) {
  const refreshToken = getRefreshToken()
  if (!refreshToken) return null

  const actualBodyType = bodyType ?? getTokenBodyType()

  try {
    const { body, headers } = buildTokenBody(
      {
        grant_type: 'refresh_token',
        refresh_token: refreshToken,
        client_id: clientId,
      },
      actualBodyType,
    )

    const response = await fetch(tokenEndpoint, {
      method: 'POST',
      headers,
      body: body as BodyInit,
    })

    if (!response.ok) {
      clearTokens()
      return null
    }

    const data = await response.json()
    storeTokens({
      access_token: data.access_token,
      refresh_token: data.refresh_token || refreshToken,
      expires_in: data.expires_in || 3600,
      token_type: data.token_type || 'Bearer',
    })

    return data
  } catch {
    clearTokens()
    return null
  }
}

export async function ensureValidToken(
  tokenEndpoint: string,
  clientId: string,
  bodyType?: TokenBodyType,
  bufferSeconds: number = 60,
): Promise<boolean> {
  const tokens = getTokens()
  if (!tokens) return false

  if (Date.now() + bufferSeconds * 1000 > tokens.token_expiry) {
    const result = await refreshTokens(tokenEndpoint, clientId, bodyType)
    return result !== null
  }

  return true
}
