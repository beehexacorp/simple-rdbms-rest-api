import { base64UrlEncode, sha256 } from './utils'
import type { PKCEPair } from './types'

export async function generatePKCE(): Promise<PKCEPair> {
  const array = new Uint8Array(32)
  crypto.getRandomValues(array)

  const codeVerifier = base64UrlEncode(array)
  const hash = await sha256(codeVerifier)
  const codeChallenge = base64UrlEncode(hash)

  return { code_verifier: codeVerifier, code_challenge: codeChallenge }
}
