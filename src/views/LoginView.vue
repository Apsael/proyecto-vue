<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'
import { api } from '@/services/api'

const router = useRouter()
const store = useStore()
const toast = useToast()

const email = ref('')
const password = ref('')
const loading = ref(false)
const notVerified = ref(false)

async function handleLogin() {
  if (!email.value || !password.value) {
    toast.error('Complete todos los campos')
    return
  }
  loading.value = true
  const error = await store.login(email.value, password.value)
  loading.value = false

  if (error) {
    if (error.toLowerCase().includes('verificar') || error.toLowerCase().includes('verificado')) {
      notVerified.value = true
    }
    toast.error(error)
  } else {
    notVerified.value = false
    toast.success('Bienvenido!')
    const user = store.getSession()
    if (user?.rol === 'admin') {
      router.push('/admin/dashboard')
    } else {
      router.push('/')
    }
  }
}

async function reenviarVerificacion() {
  if (!email.value) return
  loading.value = true
  try {
    const res = await api.auth.reenviarVerificacion(email.value)
    const data: any = res
    const verificationLink = `${window.location.origin}/verificar?token=${data.token}`
    const emailBody = `
      <h2>Verifica tu cuenta - La Dolce Vita</h2>
      <p>Haz clic en el siguiente enlace para verificar tu correo:</p>
      <div style="text-align:center;margin:30px 0">
        <a href="${verificationLink}" style="background-color:#e91e63;color:white;padding:12px 30px;text-decoration:none;border-radius:5px;display:inline-block">Verificar mi cuenta</a>
      </div>
      <p style="color:#999;font-size:14px">O copia este enlace: ${verificationLink}</p>`
    await api.mail.send(email.value, 'Verifica tu correo - La Dolce Vita', emailBody)
    toast.success('Correo de verificación reenviado')
  } catch (e: any) {
    toast.error(e.message || 'Error al reenviar verificación')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-page">
    <div class="login-container">
      <div class="login-logo">
        <i class="fas fa-ice-cream"></i>
      </div>
      <h1 class="login-title">La Dolce Vita</h1>
      <p class="login-subtitle">Heladeria Artesanal</p>

      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="email">Correo Electronico</label>
          <div class="input-wrapper">
            <i class="fas fa-envelope"></i>
            <input id="email" v-model="email" type="email" placeholder="correo@ejemplo.com" required />
          </div>
        </div>

        <div class="form-group">
          <label for="password">Contrasena</label>
          <div class="input-wrapper">
            <i class="fas fa-lock"></i>
            <input id="password" v-model="password" type="password" placeholder="Ingrese su contrasena" required />
          </div>
        </div>

        <button type="submit" class="btn-login" :disabled="loading">
          <i :class="loading ? 'fas fa-spinner fa-spin' : 'fas fa-sign-in-alt'"></i>
          {{ loading ? 'Ingresando...' : 'Iniciar Sesion' }}
        </button>
      </form>

      <div v-if="notVerified" class="verify-reminder">
        <i class="fas fa-envelope"></i>
        <p>Tu correo no ha sido verificado. Revisa tu bandeja de entrada.</p>
        <button class="btn-resend" @click="reenviarVerificacion" :disabled="loading">
          <i class="fas fa-paper-plane"></i> Reenviar correo de verificación
        </button>
      </div>

      <div class="register-link">
        No tienes cuenta? <router-link to="/register">Registrate aqui</router-link>
      </div>

      <div class="footer-decor">Heladeria La Dolce Vita &copy; 2026</div>
    </div>
  </div>
</template>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #ff9a9e 0%, #fecfef 50%, #fdfcfb 100%);
  padding: 20px;
}

.login-container {
  width: 100%;
  max-width: 440px;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 24px;
  padding: 50px 40px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.1);
}

.login-logo { text-align: center; margin-bottom: 10px; }
.login-logo i { font-size: 50px; color: #e91e63; }
.login-title { text-align: center; font-size: 26px; font-weight: 700; color: #333; margin-bottom: 6px; }
.login-subtitle { text-align: center; font-size: 14px; color: #888; margin-bottom: 35px; }

.form-group { margin-bottom: 20px; }
.form-group label { display: block; font-size: 13px; font-weight: 500; color: #555; margin-bottom: 6px; }

.input-wrapper { position: relative; }
.input-wrapper i { position: absolute; left: 16px; top: 50%; transform: translateY(-50%); color: #bbb; font-size: 16px; }
.input-wrapper input {
  width: 100%;
  padding: 14px 16px 14px 46px;
  border: 2px solid #eee;
  border-radius: 14px;
  font-size: 15px;
  font-family: 'Poppins', sans-serif;
  transition: all 0.3s ease;
  background: #fafafa;
}
.input-wrapper input:focus { outline: none; border-color: #e91e63; background: #fff; box-shadow: 0 0 0 4px rgba(233, 30, 99, 0.1); }

.btn-login {
  width: 100%;
  padding: 15px;
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  border: none;
  border-radius: 14px;
  font-size: 16px;
  font-weight: 600;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 10px;
}
.btn-login:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 8px 25px rgba(233, 30, 99, 0.4); }
.btn-login:disabled { opacity: 0.7; cursor: not-allowed; }

.register-link { text-align: center; margin-top: 25px; font-size: 14px; color: #888; }
.register-link a { color: #e91e63; text-decoration: none; font-weight: 600; }
.register-link a:hover { text-decoration: underline; }

.footer-decor { text-align: center; margin-top: 30px; color: #ccc; font-size: 12px; }

.verify-reminder { margin-top: 20px; padding: 16px; background: #fff3e0; border-radius: 12px; text-align: center; }
.verify-reminder i { font-size: 28px; color: #e65100; margin-bottom: 8px; display: block; }
.verify-reminder p { font-size: 13px; color: #e65100; margin-bottom: 12px; }
.btn-resend { padding: 8px 18px; background: #e65100; color: white; border: none; border-radius: 8px; font-size: 12px; font-family: 'Poppins', sans-serif; cursor: pointer; font-weight: 500; transition: all 0.2s; }
.btn-resend:hover { background: #bf360c; }
.btn-resend:disabled { opacity: 0.6; cursor: not-allowed; }
</style>
