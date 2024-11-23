import { defineAsyncComponent } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  // /* @ts-ignore: Unreachable code error */
  // history: createWebHistory(import.meta.env.BASE_URL),
  history: createWebHistory('/dashboard'),
  routes: [
    {
      path: '/',
      name: 'home',
      component: defineAsyncComponent(() => import('../views/HomeView.vue')),
    },
    {
      path: '',
      name: 'empty-home',
      component: defineAsyncComponent(() => import('../views/HomeView.vue')),
    },
    {
      path: '/documentation',
      name: 'documentation',
      component: defineAsyncComponent(() => import('../views/Documentation.vue')),
    },
    {
      path: '/settings',
      name: 'settings',
      component: defineAsyncComponent(() => import('../views/Settings.vue')),
    },
  ],
})

export default router
