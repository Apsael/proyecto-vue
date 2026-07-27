import { createRouter, createWebHistory } from 'vue-router'

import HomeView from '@/views/HomeView.vue'
import AboutView from '@/views/AboutView.vue'
import LoginView from '@/views/LoginView.vue'
import RegisterView from '@/views/RegisterView.vue'
import CarritoView from '@/views/CarritoView.vue'
import CheckoutView from '@/views/CheckoutView.vue'
import MisComprasView from '@/views/MisComprasView.vue'
import MiPerfilView from '@/views/MiPerfilView.vue'
import AdminDashboardView from '@/views/AdminDashboardView.vue'
import AdminProductosView from '@/views/AdminProductosView.vue'
import AdminUsuariosView from '@/views/AdminUsuariosView.vue'
import AdminVentasView from '@/views/AdminVentasView.vue'
import AdminReportesView from '@/views/AdminReportesView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'home', component: HomeView },
    { path: '/about', name: 'about', component: AboutView },
    { path: '/login', name: 'login', component: LoginView },
    { path: '/register', name: 'register', component: RegisterView },
    { path: '/carrito', name: 'carrito', component: CarritoView, meta: { requiresAuth: true } },
    { path: '/checkout', name: 'checkout', component: CheckoutView, meta: { requiresAuth: true } },
    { path: '/mis-compras', name: 'mis-compras', component: MisComprasView, meta: { requiresAuth: true } },
    { path: '/mi-perfil', name: 'mi-perfil', component: MiPerfilView, meta: { requiresAuth: true } },
    { path: '/admin/dashboard', name: 'admin-dashboard', component: AdminDashboardView, meta: { requiresAuth: true, requiresAdmin: true } },
    { path: '/admin/productos', name: 'admin-productos', component: AdminProductosView, meta: { requiresAuth: true, requiresAdmin: true } },
    { path: '/admin/usuarios', name: 'admin-usuarios', component: AdminUsuariosView, meta: { requiresAuth: true, requiresAdmin: true } },
    { path: '/admin/ventas', name: 'admin-ventas', component: AdminVentasView, meta: { requiresAuth: true, requiresAdmin: true } },
    { path: '/admin/reportes', name: 'admin-reportes', component: AdminReportesView, meta: { requiresAuth: true, requiresAdmin: true } },
  ]
})

router.beforeEach((to) => {
  const session = sessionStorage.getItem('heladeria_session')
  const user = session ? JSON.parse(session) : null

  if (to.meta.requiresAuth && !user) {
    return { name: 'login' }
  }

  if (to.meta.requiresAdmin && user?.rol !== 'admin') {
    return { name: 'home' }
  }
})

export default router
