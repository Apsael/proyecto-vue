import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '@/views/LoginView.vue'
import RegisterView from '@/views/RegisterView.vue'
import DashboardView from '@/views/DashboardView.vue'
import ProductosView from '@/views/ProductosView.vue'
import VentasView from '@/views/VentasView.vue'
import ReportesView from '@/views/ReportesView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/login' },
    { path: '/login', name: 'login', component: LoginView },
    { path: '/register', name: 'register', component: RegisterView },
    { path: '/dashboard', name: 'dashboard', component: DashboardView, meta: { requiresAuth: true } },
    { path: '/productos', name: 'productos', component: ProductosView, meta: { requiresAuth: true } },
    { path: '/ventas', name: 'ventas', component: VentasView, meta: { requiresAuth: true } },
    { path: '/reportes', name: 'reportes', component: ReportesView, meta: { requiresAuth: true } },
  ]
})

router.beforeEach((to) => {
  if (to.meta.requiresAuth) {
    const session = sessionStorage.getItem('heladeria_session')
    if (!session) return { name: 'login' }
  }
})

export default router
