<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from '@/composables/useStore'

const router = useRouter()
const store = useStore()

const nombre = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const error = ref('')
const success = ref('')

function handleRegister() {
  error.value = ''
  success.value = ''

  if (!nombre.value || !email.value || !password.value || !confirmPassword.value) {
    error.value = 'Complete todos los campos.'
    return
  }
  if (password.value !== confirmPassword.value) {
    error.value = 'Las contrasenas no coinciden.'
    return
  }
  const result = store.register(nombre.value, email.value, password.value)
  if (result) {
    error.value = result
  } else {
    success.value = 'Cuenta creada exitosamente. Ya puedes iniciar sesion.'
    setTimeout(() => router.push('/login'), 2000)
  }
}
</script>

<template>
  <div class="register-page">
    <div class="register-container">
      <div class="register-logo">
        <i class="fas fa-user-plus"></i>
      </div>
      <h1 class="register-title">Crear Cuenta</h1>
      <p class="register-subtitle">Unete a La Dolce Vita</p>

      <div v-if="error" class="error-msg">
        <i class="fas fa-exclamation-circle"></i> {{ error }}
      </div>

      <div v-if="success" class="success-msg">
        <i class="fas fa-check-circle"></i> {{ success }}
      </div>

      <form @submit.prevent="handleRegister">
        <div class="form-group">
          <label for="nombre">Nombre Completo</label>
          <div class="input-wrapper">
            <i class="fas fa-user"></i>
            <input id="nombre" v-model="nombre" type="text" placeholder="Juan Perez" required />
          </div>
        </div>

        <div class="form-group">
          <label for="email">Correo Electronico</label>
          <div class="input-wrapper">
            <i class="fas fa-envelope"></i>
            <input id="email" v-model="email" type="email" placeholder="correo@ejemplo.com" required />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="password">Contrasena</label>
            <div class="input-wrapper">
              <i class="fas fa-lock"></i>
              <input id="password" v-model="password" type="password" placeholder="Minimo 6 caracteres" required />
            </div>
          </div>
          <div class="form-group">
            <label for="confirm">Confirmar</label>
            <div class="input-wrapper">
              <i class="fas fa-lock"></i>
              <input id="confirm" v-model="confirmPassword" type="password" placeholder="Repita la contrasena" required />
            </div>
          </div>
        </div>

        <button type="submit" class="btn-register">
          <i class="fas fa-paper-plane"></i> Registrarse
        </button>
      </form>

      <div class="login-link">
        Ya tienes cuenta? <router-link to="/login">Inicia sesion</router-link>
      </div>

      <div class="footer-decor">
        Heladeria La Dolce Vita &copy; 2026
      </div>
    </div>
  </div>
</template>

<style scoped>
.register-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #a18cd1 0%, #fbc2eb 100%);
  padding: 20px;
}

.register-container {
  width: 100%;
  max-width: 480px;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 24px;
  padding: 45px 40px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.1);
}

.register-logo {
  text-align: center;
  margin-bottom: 10px;
}

.register-logo i {
  font-size: 45px;
  color: #9c27b0;
}

.register-title {
  text-align: center;
  font-size: 24px;
  font-weight: 700;
  color: #333;
  margin-bottom: 6px;
}

.register-subtitle {
  text-align: center;
  font-size: 14px;
  color: #888;
  margin-bottom: 30px;
}

.form-row {
  display: flex;
  gap: 15px;
}

.form-row .form-group {
  flex: 1;
}

.form-group {
  margin-bottom: 18px;
}

.form-group label {
  display: block;
  font-size: 13px;
  font-weight: 500;
  color: #555;
  margin-bottom: 6px;
}

.input-wrapper {
  position: relative;
}

.input-wrapper i {
  position: absolute;
  left: 16px;
  top: 50%;
  transform: translateY(-50%);
  color: #bbb;
  font-size: 15px;
}

.input-wrapper input {
  width: 100%;
  padding: 13px 16px 13px 46px;
  border: 2px solid #eee;
  border-radius: 14px;
  font-size: 14px;
  font-family: 'Poppins', sans-serif;
  transition: all 0.3s ease;
  background: #fafafa;
}

.input-wrapper input:focus {
  outline: none;
  border-color: #9c27b0;
  background: #fff;
  box-shadow: 0 0 0 4px rgba(156, 39, 176, 0.1);
}

.btn-register {
  width: 100%;
  padding: 15px;
  background: linear-gradient(135deg, #9c27b0, #ba68c8);
  color: white;
  border: none;
  border-radius: 14px;
  font-size: 16px;
  font-weight: 600;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 8px;
}

.btn-register:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(156, 39, 176, 0.4);
}

.error-msg {
  background: #fff0f3;
  color: #c62828;
  padding: 12px 16px;
  border-radius: 10px;
  font-size: 13px;
  margin-bottom: 18px;
  border-left: 4px solid #c62828;
}

.success-msg {
  background: #f0fff4;
  color: #2e7d32;
  padding: 12px 16px;
  border-radius: 10px;
  font-size: 13px;
  margin-bottom: 18px;
  border-left: 4px solid #2e7d32;
}

.login-link {
  text-align: center;
  margin-top: 22px;
  font-size: 14px;
  color: #888;
}

.login-link a {
  color: #9c27b0;
  text-decoration: none;
  font-weight: 600;
}

.login-link a:hover {
  text-decoration: underline;
}

.footer-decor {
  text-align: center;
  margin-top: 25px;
  color: #ccc;
  font-size: 12px;
}
</style>
