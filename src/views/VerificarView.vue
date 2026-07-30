<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { api } from '@/services/api'
import { useToast } from '@/composables/useToast'

const route = useRoute()
const router = useRouter()
const toast = useToast()

const status = ref<'verificando' | 'exitoso' | 'error'>('verificando')
const mensaje = ref('')

onMounted(async () => {
  const token = route.query.token as string
  if (!token) {
    status.value = 'error'
    mensaje.value = 'Token de verificación no proporcionado'
    return
  }
  try {
    await api.auth.verificarEmail(token)
    status.value = 'exitoso'
    mensaje.value = '¡Correo verificado exitosamente!'
    toast.success('Email verificado correctamente')
  } catch (e: any) {
    status.value = 'error'
    mensaje.value = e.message || 'Error al verificar el correo'
  }
})
</script>

<template>
  <div class="verificar-page">
    <div class="verificar-card">
      <div class="verificar-icon" :class="status">
        <i v-if="status === 'verificando'" class="fas fa-spinner fa-spin"></i>
        <i v-else-if="status === 'exitoso'" class="fas fa-check-circle"></i>
        <i v-else class="fas fa-times-circle"></i>
      </div>
      <h2 v-if="status === 'verificando'">Verificando...</h2>
      <h2 v-else-if="status === 'exitoso'">¡Verificado!</h2>
      <h2 v-else>Error</h2>
      <p>{{ mensaje }}</p>
      <router-link to="/" class="btn-home"><i class="fas fa-home"></i> Ir al inicio</router-link>
    </div>
  </div>
</template>

<style scoped>
.verificar-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #a18cd1, #fbc2eb);
  padding: 20px;
}
.verificar-card {
  background: white;
  border-radius: 24px;
  padding: 50px 40px;
  text-align: center;
  max-width: 420px;
  box-shadow: 0 20px 60px rgba(0,0,0,0.1);
}
.verificar-icon { font-size: 64px; margin-bottom: 20px; }
.verificar-icon.verificando { color: #2196f3; }
.verificar-icon.exitoso { color: #4caf50; }
.verificar-icon.error { color: #ef5350; }
.verificar-card h2 { font-size: 22px; color: #333; margin-bottom: 10px; }
.verificar-card p { color: #666; margin-bottom: 25px; }
.btn-home {
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  padding: 12px 28px;
  border-radius: 12px;
  text-decoration: none;
  font-weight: 600;
  display: inline-flex;
  align-items: center;
  gap: 8px;
}
</style>
