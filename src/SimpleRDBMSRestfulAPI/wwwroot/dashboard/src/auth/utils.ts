export function base64UrlEncode(bytes: Uint8Array): string {
  const binString = String.fromCharCode(...bytes)
  const base64 = btoa(binString)
  return base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/g, '')
}

export async function sha256(text: string): Promise<Uint8Array> {
  const enc = new TextEncoder()
  const data = enc.encode(text)
  const digest = await crypto.subtle.digest('SHA-256', data)
  return new Uint8Array(digest)
}
