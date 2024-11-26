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
      component: () => import('../views/HomeView.vue'),
    },
    {
      path: '',
      name: 'empty-home',
      component: () => import('../views/HomeView.vue'),
    },
    {
      path: '/documentation',
      name: 'documentation',
      component: () => import('../views/Documentation.vue'),
    },
    {
      path: '/settings',
      name: 'settings',
      component: () => import('../views/Settings.vue'),
    },
    {
      path: '/documentation/entity/detail',
      name: 'documentation-entity-detail',
      component: () => import('../views/EntityDetailView.vue'),
      props: (route) => ({ detail: route.query.detail }), // Pass query parameter as props
    }
  ],
})

export default router
