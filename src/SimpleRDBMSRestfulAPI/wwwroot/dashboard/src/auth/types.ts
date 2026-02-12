export interface PKCEPair {
  code_verifier: string
  code_challenge: string
}

export type TokenBodyType = 'json' | 'urlencoded'

export interface Tokens {
  access_token: string
  refresh_token: string
  token_type?: string
  expires_in: number
}

export interface StoredTokens {
  access_token: string
  refresh_token: string
  token_expiry: number
}

export interface AuthGuardOptions {
  ssoUrl: string
  clientId: string
  redirectUri?: string
  scope?: string
}

export interface SSOUser {
  sub: string
  email?: string
  client_id?: string
  [key: string]: any
}
