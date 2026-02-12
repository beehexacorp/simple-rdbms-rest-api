/**
 * Authentication composable for HexaSync SSO v3 authentication.
 *
 * Mirrors the cyberbot-v2 pattern: wraps the shared auth middleware
 * with app-specific env defaults + Ant Design message integration.
 */

import type { UseAuthOptions } from '@/auth/useAuth'
import { useAuth as baseUseAuth } from '@/auth/useAuth'
import { message } from 'ant-design-vue'

const API_BASE_URL = (import.meta.env?.VITE_API_URL as string) || 'http://localhost:5000'
const CLIENT_ID = (import.meta.env?.VITE_SSO_CLIENT_ID as string) || 'simple_rdbms_dashboard'

export function useAuth(options?: Partial<UseAuthOptions>) {
  return baseUseAuth({
    apiBaseUrl: API_BASE_URL,
    clientId: CLIENT_ID,
    showMessage: (msg: string, type: 'success' | 'error') => {
      if (type === 'success') message.success(msg)
      else message.error(msg)
    },
    ...options,
  })
}

export type { SSOUser } from '@/auth/types'
export type { UseAuthOptions } from '@/auth/useAuth'
