<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from '@/composables/useStore'

const route = useRoute()
const router = useRouter()
const store = useStore()

const user = computed(() => store.getSession())
const isAdmin = computed(() => store.isAdmin())
const cartCount = computed(() => store.getCartCount())

const publicLinks = [
  { to: '/', label: 'Inicio', icon: 'fas fa-home' },
  { to: '/about', label: 'Nosotros', icon: 'fas fa-heart' },
]

const clientLinks = [
  { to: '/mis-compras', label: 'Mis Compras', icon: 'fas fa-shopping-bag' },
  { to: '/mi-perfil', label: 'Mi Perfil', icon: 'fas fa-user' },
]

const adminLinks = [
  { to: '/admin/dashboard', label: 'Panel', icon: 'fas fa-th-large' },
  { to: '/admin/productos', label: 'Productos', icon: 'fas fa-ice-cream' },
  { to: '/admin/ventas', label: 'Ventas', icon: 'fas fa-cash-register' },
  { to: '/admin/despacho', label: 'Despacho', icon: 'fas fa-truck' },
  { to: '/admin/empresa', label: 'Empresa', icon: 'fas fa-store' },
  { to: '/admin/usuarios', label: 'Usuarios', icon: 'fas fa-users' },
  { to: '/admin/reportes', label: 'Reportes', icon: 'fas fa-chart-bar' },
]

function handleLogout() {
  store.logout()
  router.push('/')
}
</script>

<template>
  <div class="topbar">
    <router-link to="/" class="topbar-brand">
      <img src="/logo.png" alt="La Dolce Vita" class="topbar-logo" />
      La Dolce Vita
    </router-link>

    <div class="topbar-nav">
      <template v-if="isAdmin">
        <router-link
          v-for="link in adminLinks"
          :key="link.to"
          :to="link.to"
          class="nav-link"
          :class="{ active: route.path === link.to }"
        >
          <i :class="link.icon"></i> {{ link.label }}
        </router-link>
      </template>
      <template v-else>
        <router-link
          v-for="link in publicLinks"
          :key="link.to"
          :to="link.to"
          class="nav-link"
          :class="{ active: route.path === link.to }"
        >
          <i :class="link.icon"></i> {{ link.label }}
        </router-link>
        <template v-if="user">
          <router-link
            v-for="link in clientLinks"
            :key="link.to"
            :to="link.to"
            class="nav-link"
            :class="{ active: route.path === link.to }"
          >
            <i :class="link.icon"></i> {{ link.label }}
          </router-link>
        </template>
      </template>
    </div>

    <div class="topbar-right">
      <router-link v-if="user && !isAdmin" to="/carrito" class="cart-btn">
        <i class="fas fa-shopping-cart"></i>
        <span v-if="cartCount > 0" class="cart-badge">{{ cartCount }}</span>
      </router-link>

      <div v-if="user" class="topbar-user">
        <span><i class="fas fa-user-circle"></i> <strong>{{ user.nombre }}</strong></span>
        <button class="btn-logout" @click="handleLogout">
          <i class="fas fa-sign-out-alt"></i> Salir
        </button>
      </div>
      <div v-else class="topbar-auth">
        <router-link to="/login" class="btn-login-nav"><i class="fas fa-sign-in-alt"></i> Ingresar</router-link>
        <router-link to="/register" class="btn-register-nav"><i class="fas fa-user-plus"></i> Registrarse</router-link>
      </div>
    </div>
  </div>
</template>

<style scoped>
.topbar {
  background: white;
  padding: 12px 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.06);
  position: sticky;
  top: 0;
  z-index: 100;
}

.topbar-brand {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 17px;
  font-weight: 700;
  color: #e91e63;
  text-decoration: none;
  flex-shrink: 0;
}

.topbar-logo { height: 36px; }

.topbar-nav {
  display: flex;
  gap: 4px;
  flex-wrap: wrap;
}

.nav-link {
  text-decoration: none;
  color: #666;
  font-size: 13px;
  padding: 7px 14px;
  border-radius: 10px;
  transition: all 0.2s;
  font-weight: 500;
}

.nav-link:hover, .nav-link.active {
  background: #fce4ec;
  color: #e91e63;
}

.topbar-right {
  display: flex;
  align-items: center;
  gap: 14px;
  flex-shrink: 0;
}

.cart-btn {
  position: relative;
  color: #666;
  font-size: 18px;
  padding: 6px;
  text-decoration: none;
  transition: color 0.2s;
}

.cart-btn:hover { color: #e91e63; }

.cart-badge {
  position: absolute;
  top: -4px;
  right: -6px;
  background: #e91e63;
  color: white;
  font-size: 10px;
  font-weight: 700;
  width: 18px;
  height: 18px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.topbar-user {
  display: flex;
  align-items: center;
  gap: 12px;
  color: #333;
  font-size: 13px;
}

.btn-logout {
  background: rgba(233, 30, 99, 0.08);
  color: #e91e63;
  border: 1px solid rgba(233, 30, 99, 0.2);
  padding: 6px 14px;
  border-radius: 8px;
  font-size: 12px;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  transition: all 0.3s;
  display: inline-flex;
  align-items: center;
  gap: 5px;
  font-weight: 500;
}

.btn-logout:hover { background: #fce4ec; }

.topbar-auth {
  display: flex;
  gap: 8px;
}

.btn-login-nav {
  color: #e91e63;
  text-decoration: none;
  font-size: 13px;
  font-weight: 500;
  padding: 7px 14px;
  border-radius: 8px;
  transition: all 0.2s;
}

.btn-login-nav:hover { background: #fce4ec; }

.btn-register-nav {
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  text-decoration: none;
  font-size: 13px;
  font-weight: 500;
  padding: 7px 14px;
  border-radius: 8px;
  transition: all 0.2s;
}

.btn-register-nav:hover { box-shadow: 0 4px 12px rgba(233, 30, 99, 0.3); }

@media (max-width: 768px) {
  .topbar { flex-wrap: wrap; gap: 8px; padding: 10px 14px; }
  .topbar-nav { order: 3; width: 100%; overflow-x: auto; }
  .topbar-right { gap: 8px; }
}
</style>
