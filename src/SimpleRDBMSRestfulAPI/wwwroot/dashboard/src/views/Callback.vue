<template>
  <div class="callback-container">
    <a-spin :size="64" :indicator="indicator" />
    <p class="callback-text">{{ statusText }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref, h, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { LoadingOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'

import { storeTokens, getCodeVerifier, clearCodeVerifier, buildTokenBody, type TokenBodyType } from '@/auth'

const router = useRouter()
const route = useRoute()

const statusText = ref('Completing authentication...')

const indicator = h(LoadingOutlined, { style: { fontSize: '48px', color: '#1890ff' }, spin: true })

const CLIENT_ID = (import.meta.env?.VITE_SSO_CLIENT_ID as string) || 'simple_rdbms_dashboard'
const API_URL = (import.meta.env?.VITE_API_URL as string) || 'http://localhost:5000'

// Existing backend endpoint for SSO token exchange (mirrors cyberbot-v2)
const TOKEN_ENDPOINT = `${API_URL}/api/sso/v3/auth/token`

function getRedirectUri() {
  const configured = import.meta.env?.VITE_SSO_REDIRECT_URI as string
  if (configured && configured.trim() !== '') {
    return configured
  }

  const baseUrl = (import.meta.env?.BASE_URL as string) || '/'
  const normalizedBaseUrl = baseUrl.endsWith('/') ? baseUrl : `${baseUrl}/`
  return `${window.location.origin}${normalizedBaseUrl}sso/callback`
}

function getTokenBodyType(): TokenBodyType {
  const envValue = import.meta.env?.EXCHANGE_TOKEN_POST_BODY_TYPE as string
  if (envValue === 'urlencoded' || envValue === 'form') return 'urlencoded'
  return 'json'
}

function normalizeLocalRedirect(path: string | null): string | null {
  if (!path) return null
  if (!path.startsWith('/')) return null
  if (path.startsWith('//')) return null
  if (path.includes('://')) return null
  return path
}

function tryParseJSON(text: string): any | null {
  try {
    return JSON.parse(text)
  } catch {
    return null
  }
}

onMounted(async () => {
  const params = new URLSearchParams(window.location.search)
  const code = params.get('code')
  const state = params.get('state')
  const error = params.get('error')
  const errorDescription = params.get('error_description')

  if (error) {
    statusText.value = errorDescription || 'Authentication failed'
    message.error(errorDescription || `Authentication error: ${error}`)
    clearCodeVerifier()
    setTimeout(() => router.push('/'), 1500)
    return
  }

  if (!code) {
    statusText.value = 'Missing authorization code'
    message.error('Invalid authentication response')
    clearCodeVerifier()
    setTimeout(() => router.push('/'), 1500)
    return
  }

  const codeVerifier = getCodeVerifier()
  if (!codeVerifier) {
    statusText.value = 'Authentication session expired'
    message.error('Please try logging in again')
    setTimeout(() => router.push('/'), 1500)
    return
  }

  // cyberbot-v2 encodes original URL in state as base64
  let originalUrl = '/'
  if (state) {
    try {
      originalUrl = atob(state)
    } catch {
      originalUrl = '/'
    }
  }

  // Requirement: support explicit redirect query param too
  const redirectOverride =
    typeof route.query.redirect === 'string' ? normalizeLocalRedirect(route.query.redirect) : null
  if (redirectOverride) {
    originalUrl = redirectOverride
  }

  try {
    const bodyType = getTokenBodyType()
    const { body, headers } = buildTokenBody(
      {
        grant_type: 'authorization_code',
        code,
        redirect_uri: getRedirectUri(),
        client_id: CLIENT_ID,
        code_verifier: codeVerifier,
      },
      bodyType,
    )

    const response = await fetch(TOKEN_ENDPOINT, {
      method: 'POST',
      headers,
      body: body as BodyInit,
    })

    const responseText = await response.text()

    if (!response.ok) {
      const errorData = tryParseJSON(responseText)
      throw new Error(
        errorData?.detail || errorData?.error || `Token exchange failed: ${response.status} ${response.statusText}`,
      )
    }

    const tokens = tryParseJSON(responseText)
    if (!tokens) throw new Error('Invalid JSON response from token endpoint')

    storeTokens({
      access_token: tokens.access_token,
      refresh_token: tokens.refresh_token,
      token_type: tokens.token_type || 'Bearer',
      expires_in: tokens.expires_in || 3600,
    })

    clearCodeVerifier()

    statusText.value = 'Authentication successful!'
    message.success('Login successful!')

    setTimeout(() => {
      router.push(originalUrl)
    }, 200)
  } catch (err) {
    statusText.value = 'Authentication failed'
    const errorMessage = err instanceof Error ? err.message : 'An error occurred'
    message.error(errorMessage)
    clearCodeVerifier()
    setTimeout(() => router.push('/'), 1500)
  }
})
</script>

<style scoped>
.callback-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
}

.callback-text {
  margin-top: 24px;
  font-size: 16px;
  font-weight: 500;
  text-align: center;
}
</style>
