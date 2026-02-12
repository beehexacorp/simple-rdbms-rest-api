import { defineAsyncComponent } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'

import { createAuthGuard } from './auth'

const router = createRouter({
  // /* @ts-ignore: Unreachable code error */
  // history: createWebHistory(import.meta.env.BASE_URL),
  history: createWebHistory((import.meta.env?.BASE_URL as string) || '/'),
  routes: [
    // Public auth routes
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/Login.vue'),
      meta: { requiresAuth: false },
    },
    {
      path: '/sso/callback',
      name: 'callback',
      component: () => import('../views/Callback.vue'),
      meta: { requiresAuth: false },
    },
    {
      path: '/callback',
      redirect: '/sso/callback',
      meta: { requiresAuth: false },
    },
    {
      path: '/',
      name: 'home',
      component: () => import('../views/HomeView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '',
      name: 'empty-home',
      component: () => import('../views/HomeView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/documentation',
      name: 'documentation',
      component: () => import('../views/Documentation.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/settings',
      name: 'settings',
      component: () => import('../views/Settings.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/documentation/entity/detail',
      name: 'documentation-entity-detail',
      component: () => import('../views/EntityDetailView.vue'),
      props: (route) => ({ detail: route.query.detail }), // Pass query parameter as props
      meta: { requiresAuth: true },
    }
  ],
})

router.beforeEach(async (to, from, next) => {
  const guard = createAuthGuard()
  await guard(to, from, next)
})

export default router
