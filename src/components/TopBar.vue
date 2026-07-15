<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from '@/composables/useStore'

const route = useRoute()
const router = useRouter()
const store = useStore()

const user = computed(() => store.getSession())

const links = [
  { to: '/dashboard', label: 'Inicio', icon: 'fas fa-home' },
  { to: '/productos', label: 'Productos', icon: 'fas fa-ice-cream' },
  { to: '/ventas', label: 'Ventas', icon: 'fas fa-cash-register' },
  { to: '/reportes', label: 'Reportes', icon: 'fas fa-chart-bar' },
]

function handleLogout() {
  store.logout()
  router.push('/login')
}
</script>

<template>
  <div class="topbar">
    <router-link to="/dashboard" class="topbar-brand">
      <i class="fas fa-ice-cream"></i> La Dolce Vita
    </router-link>
    <div class="topbar-nav">
      <router-link
        v-for="link in links"
        :key="link.to"
        :to="link.to"
        class="nav-link"
        :class="{ active: route.path === link.to }"
      >
        <i :class="link.icon"></i> {{ link.label }}
      </router-link>
    </div>
    <div class="topbar-user">
      <span v-if="user"><i class="fas fa-user-circle"></i> <strong>{{ user.nombre }}</strong></span>
      <button class="btn-logout" @click="handleLogout">
        <i class="fas fa-sign-out-alt"></i> Salir
      </button>
    </div>
  </div>
</template>

<style scoped>
.topbar {
  background: white;
  padding: 14px 40px;
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
  font-size: 18px;
  font-weight: 700;
  color: #e91e63;
  text-decoration: none;
}

.topbar-nav {
  display: flex;
  gap: 8px;
}

.nav-link {
  text-decoration: none;
  color: #666;
  font-size: 13px;
  padding: 8px 16px;
  border-radius: 10px;
  transition: all 0.2s;
  font-weight: 500;
}

.nav-link:hover,
.nav-link.active {
  background: #fce4ec;
  color: #e91e63;
}

.topbar-user {
  display: flex;
  align-items: center;
  gap: 16px;
  color: #333;
  font-size: 14px;
}

.btn-logout {
  background: rgba(233, 30, 99, 0.08);
  color: #e91e63;
  border: 1px solid rgba(233, 30, 99, 0.2);
  padding: 8px 18px;
  border-radius: 10px;
  font-size: 13px;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  transition: all 0.3s;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-weight: 500;
}

.btn-logout:hover {
  background: #fce4ec;
}

@media (max-width: 768px) {
  .topbar {
    flex-wrap: wrap;
    gap: 10px;
    padding: 12px 16px;
  }
  .topbar-nav {
    order: 3;
    width: 100%;
    overflow-x: auto;
  }
}
</style>
