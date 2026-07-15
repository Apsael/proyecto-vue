<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from '@/composables/useStore'

const router = useRouter()
const store = useStore()

const email = ref('')
const password = ref('')
const error = ref('')

function handleLogin() {
  error.value = ''
  if (!email.value || !password.value) {
    error.value = 'Complete todos los campos.'
    return
  }
  const user = store.login(email.value, password.value)
  if (user) {
    router.push('/dashboard')
  } else {
    error.value = 'Correo o contrasena incorrectos.'
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

      <div v-if="error" class="error-msg">
        <i class="fas fa-exclamation-circle"></i> {{ error }}
      </div>

      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="email">Correo Electronico</label>
          <div class="input-wrapper">
            <i class="fas fa-envelope"></i>
            <input
              id="email"
              v-model="email"
              type="email"
              placeholder="correo@ejemplo.com"
              required
            />
          </div>
        </div>

        <div class="form-group">
          <label for="password">Contrasena</label>
          <div class="input-wrapper">
            <i class="fas fa-lock"></i>
            <input
              id="password"
              v-model="password"
              type="password"
              placeholder="Ingrese su contrasena"
              required
            />
          </div>
        </div>

        <button type="submit" class="btn-login">
          <i class="fas fa-sign-in-alt"></i> Iniciar Sesion
        </button>
      </form>

      <div class="register-link">
        No tienes cuenta? <router-link to="/register">Registrate aqui</router-link>
      </div>

      <div class="footer-decor">
        Heladeria La Dolce Vita &copy; 2026
      </div>
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
  backdrop-filter: blur(10px);
}

.login-logo {
  text-align: center;
  margin-bottom: 10px;
}

.login-logo i {
  font-size: 50px;
  color: #e91e63;
}

.login-title {
  text-align: center;
  font-size: 26px;
  font-weight: 700;
  color: #333;
  margin-bottom: 6px;
}

.login-subtitle {
  text-align: center;
  font-size: 14px;
  color: #888;
  margin-bottom: 35px;
}

.form-group {
  margin-bottom: 20px;
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
  font-size: 16px;
}

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

.input-wrapper input:focus {
  outline: none;
  border-color: #e91e63;
  background: #fff;
  box-shadow: 0 0 0 4px rgba(233, 30, 99, 0.1);
}

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

.btn-login:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(233, 30, 99, 0.4);
}

.error-msg {
  background: #fff0f3;
  color: #c62828;
  padding: 12px 16px;
  border-radius: 10px;
  font-size: 13px;
  margin-bottom: 20px;
  border-left: 4px solid #c62828;
}

.register-link {
  text-align: center;
  margin-top: 25px;
  font-size: 14px;
  color: #888;
}

.register-link a {
  color: #e91e63;
  text-decoration: none;
  font-weight: 600;
}

.register-link a:hover {
  text-decoration: underline;
}

.footer-decor {
  text-align: center;
  margin-top: 30px;
  color: #ccc;
  font-size: 12px;
}
</style>
