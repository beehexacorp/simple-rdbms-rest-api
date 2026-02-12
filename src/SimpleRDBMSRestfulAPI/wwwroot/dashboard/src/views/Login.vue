<template>
  <div class="login-container">
    <a-spin size="large" />
    <div class="login-text">Redirecting to SSO…</div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { initiateSSOLogin } from '@/auth/routerGuard'
import { getAuthConfig } from '@/router/auth'

const route = useRoute()

onMounted(async () => {
  const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : '/'
  const authConfig = getAuthConfig()

  try {
    await initiateSSOLogin(redirect, authConfig)
  } catch {
    // initiateSSOLogin intentionally redirects
  }
})
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.login-text {
  margin-top: 16px;
}
</style>
