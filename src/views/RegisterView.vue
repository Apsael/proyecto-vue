<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from '@/composables/useToast'
import { api } from '@/services/api'

const router = useRouter()
const toast = useToast()

const nombre = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const registered = ref(false)
const showMap = ref(false)
const latitud = ref(-17.7853)
const longitud = ref(-63.1806)
const mapInitialized = ref(false)
let mapInstance: any = null
let markerInstance: any = null

const passwordStrength = computed(() => {
  const pwd = password.value
  let score = 0
  if (pwd.length >= 8) score++
  if (pwd.length >= 12) score++
  if (/[a-z]/.test(pwd)) score++
  if (/[A-Z]/.test(pwd)) score++
  if (/\d/.test(pwd)) score++
  if (/[^a-zA-Z\d]/.test(pwd)) score++
  return score
})

const passwordLabel = computed(() => {
  const s = passwordStrength.value
  if (s <= 2) return { text: 'Débil', color: '#ef5350', percent: 25 }
  if (s <= 3) return { text: 'Media', color: '#ff9800', percent: 50 }
  if (s <= 4) return { text: 'Fuerte', color: '#2196f3', percent: 75 }
  return { text: 'Muy segura', color: '#4caf50', percent: 100 }
})

const passwordErrors = computed(() => {
  const pwd = password.value
  const errors: string[] = []
  if (pwd && pwd.length < 8) errors.push('Mínimo 8 caracteres')
  if (pwd && !/[A-Z]/.test(pwd)) errors.push('Al menos una mayúscula')
  if (pwd && !/[a-z]/.test(pwd)) errors.push('Al menos una minúscula')
  if (pwd && !/\d/.test(pwd)) errors.push('Al menos un número')
  if (pwd && !/[^a-zA-Z\d]/.test(pwd)) errors.push('Al menos un carácter especial')
  return errors
})

onMounted(() => {
  if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        latitud.value = pos.coords.latitude
        longitud.value = pos.coords.longitude
      },
      () => {}
    )
  }
})

function initMap() {
  showMap.value = true
  setTimeout(() => {
    const L = (window as any).L
    if (!L || mapInitialized.value) return
    mapInitialized.value = true
    mapInstance = L.map('register-map').setView([latitud.value, longitud.value], 14)
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(mapInstance)
    markerInstance = L.marker([latitud.value, longitud.value], { draggable: true }).addTo(mapInstance)
    markerInstance.on('dragend', () => {
      const pos = markerInstance.getLatLng()
      latitud.value = pos.lat
      longitud.value = pos.lng
    })
    mapInstance.on('click', (e: any) => {
      markerInstance.setLatLng(e.latlng)
      latitud.value = e.latlng.lat
      longitud.value = e.latlng.lng
    })
  }, 300)
}

function removeMap() {
  if (mapInstance) {
    mapInstance.remove()
    mapInstance = null
    mapInitialized.value = false
  }
  showMap.value = false
}

function confirmLocation() {
  removeMap()
  toast.success('Ubicación seleccionada correctamente')
}

async function handleRegister() {
  if (!nombre.value || !email.value || !password.value || !confirmPassword.value) {
    toast.error('Complete todos los campos')
    return
  }
  if (password.value !== confirmPassword.value) {
    toast.error('Las contraseñas no coinciden')
    return
  }
  if (passwordErrors.value.length > 0) {
    toast.error('La contraseña no cumple los requisitos de seguridad')
    return
  }
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
    toast.error('Ingrese un correo electrónico válido')
    return
  }

  loading.value = true
  try {
    const res = await api.auth.register(nombre.value, email.value, password.value, latitud.value, longitud.value)

    const verificationLink = `${window.location.origin}/verificar?token=${res.token}`

    const emailBody = `
      <!DOCTYPE html>
      <html><head><meta charset="UTF-8"></head>
      <body style="margin:0;padding:0;font-family:Arial,sans-serif;background:#f5f6fa;">
        <table width="100%" cellpadding="0" cellspacing="0" style="padding:20px;">
          <tr><td align="center">
            <table width="600" cellpadding="0" cellspacing="0" style="background:#fff;border-radius:16px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);">
              <tr><td style="background:linear-gradient(135deg,#e91e63,#f06292);padding:30px;text-align:center;">
                <h1 style="color:#fff;margin:0;font-size:26px;">La Dolce Vita</h1>
                <p style="color:rgba(255,255,255,0.85);margin:5px 0 0;font-size:15px;">¡Bienvenido!</p>
              </td></tr>
              <tr><td style="padding:30px;">
                <h2 style="color:#333;">Hola, ${nombre.value}</h2>
                <p style="color:#666;line-height:1.6;">Gracias por registrarte. Para activar tu cuenta y poder disfrutar de nuestros helados artesanales, haz clic en el siguiente botón:</p>
                <div style="text-align:center;margin:30px 0;">
                  <a href="${verificationLink}" style="background:linear-gradient(135deg,#e91e63,#f06292);color:#fff;padding:14px 32px;text-decoration:none;border-radius:12px;display:inline-block;font-weight:600;font-size:16px;">Verificar mi Correo</a>
                </div>
                <p style="color:#666;line-height:1.6;">O copia este enlace en tu navegador:</p>
                <p style="color:#888;font-size:13px;word-break:break-all;">${verificationLink}</p>
                <p style="color:#999;font-size:12px;margin-top:20px;">Si no creaste una cuenta, ignora este mensaje.</p>
              </td></tr>
              <tr><td style="background:#f9fafb;padding:20px;text-align:center;border-top:1px solid #eee;">
                <p style="margin:0;color:#999;font-size:12px;">&copy; 2026 Heladería La Dolce Vita. Todos los derechos reservados.</p>
                <p style="margin:5px 0 0;color:#999;font-size:12px;">Calle Beni #123, Santa Cruz, Bolivia</p>
              </td></tr>
            </table>
          </td></tr>
        </table>
      </body></html>
    `

    try {
      await api.mail.send(email.value, 'Verifica tu correo - La Dolce Vita', emailBody)
    } catch {
      // email sending failed but account was created
    }

    registered.value = true
  } catch (e: any) {
    toast.error(e.message || 'Error al registrarse')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="register-page">
    <div class="register-container" v-if="!registered">
      <div class="register-logo">
        <img src="/logo.png" alt="La Dolce Vita" class="logo-img" />
      </div>
      <h1 class="register-title">Crear Cuenta</h1>
      <p class="register-subtitle">Únete a La Dolce Vita</p>

      <form @submit.prevent="handleRegister">
        <div class="form-group">
          <label for="nombre">Nombre Completo</label>
          <div class="input-wrapper">
            <i class="fas fa-user"></i>
            <input id="nombre" v-model="nombre" type="text" placeholder="Juan Pérez" required />
          </div>
        </div>

        <div class="form-group">
          <label for="email">Correo Electrónico</label>
          <div class="input-wrapper">
            <i class="fas fa-envelope"></i>
            <input id="email" v-model="email" type="email" placeholder="correo@ejemplo.com" required />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="password">Contraseña</label>
            <div class="input-wrapper">
              <i class="fas fa-lock"></i>
              <input id="password" v-model="password" type="password" placeholder="Mínimo 8 caracteres" required />
            </div>
            <div v-if="password" class="password-strength">
              <div class="strength-bar">
                <div class="strength-fill" :style="{ width: passwordLabel.percent + '%', background: passwordLabel.color }"></div>
              </div>
              <span class="strength-label" :style="{ color: passwordLabel.color }">{{ passwordLabel.text }}</span>
            </div>
            <ul v-if="passwordErrors.length > 0" class="password-errors">
              <li v-for="err in passwordErrors" :key="err"><i class="fas fa-times-circle"></i> {{ err }}</li>
            </ul>
          </div>
          <div class="form-group">
            <label for="confirm">Confirmar</label>
            <div class="input-wrapper">
              <i class="fas fa-lock"></i>
              <input id="confirm" v-model="confirmPassword" type="password" placeholder="Repita la contraseña" required />
            </div>
          </div>
        </div>

        <div class="form-group">
          <label>Ubicación de entrega</label>
          <button type="button" class="btn-location" @click="initMap">
            <i class="fas fa-map-marker-alt"></i> Seleccionar ubicación en mapa
          </button>
          <p class="location-coords" v-if="showMap">
            Lat: {{ latitud.toFixed(6) }}, Lng: {{ longitud.toFixed(6) }}
          </p>
        </div>

        <div v-if="showMap" class="map-container">
          <div id="register-map" class="leaflet-map"></div>
          <button type="button" class="btn-confirm-location" @click="confirmLocation">
            <i class="fas fa-check"></i> Confirmar ubicación
          </button>
        </div>

        <button type="submit" class="btn-register" :disabled="loading">
          <i :class="loading ? 'fas fa-spinner fa-spin' : 'fas fa-paper-plane'"></i>
          {{ loading ? 'Creando cuenta...' : 'Registrarse' }}
        </button>
      </form>

      <div class="login-link">
        ¿Ya tienes cuenta? <router-link to="/login">Inicia sesión</router-link>
      </div>

      <div class="footer-decor">Heladería La Dolce Vita &copy; 2026</div>
    </div>

    <div class="register-success" v-else>
      <div class="success-card">
        <div class="success-icon"><i class="fas fa-envelope-open-text"></i></div>
        <h2>¡Registro Exitoso!</h2>
        <p>Te hemos enviado un correo de verificación a <strong>{{ email }}</strong>.</p>
        <p class="success-hint">Revisa tu bandeja de entrada y haz clic en el enlace para activar tu cuenta.</p>
        <router-link to="/login" class="btn-login-success">
          <i class="fas fa-sign-in-alt"></i> Ir a Iniciar Sesión
        </router-link>
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

.register-container, .register-success {
  width: 100%;
  max-width: 520px;
}

.register-container {
  background: rgba(255, 255, 255, 0.97);
  border-radius: 24px;
  padding: 40px 35px;
  box-shadow: 0 25px 80px rgba(0, 0, 0, 0.12);
}

.success-card {
  background: white;
  border-radius: 24px;
  padding: 50px 40px;
  text-align: center;
  box-shadow: 0 25px 80px rgba(0, 0, 0, 0.12);
}

.success-icon { font-size: 72px; color: #4caf50; margin-bottom: 16px; }
.success-card h2 { font-size: 24px; color: #333; margin-bottom: 12px; }
.success-card p { color: #666; line-height: 1.6; margin-bottom: 6px; }
.success-hint { font-size: 13px; color: #888; margin-bottom: 24px !important; }

.btn-login-success {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 14px 32px;
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  border-radius: 12px;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.3s;
}
.btn-login-success:hover { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(233,30,99,0.3); }

.logo-img { height: 50px; }
.register-logo { text-align: center; margin-bottom: 10px; }
.register-title { text-align: center; font-size: 24px; font-weight: 700; color: #333; margin-bottom: 4px; }
.register-subtitle { text-align: center; font-size: 14px; color: #888; margin-bottom: 25px; }

.form-row { display: flex; gap: 15px; }
.form-row .form-group { flex: 1; }

.form-group { margin-bottom: 16px; }
.form-group label { display: block; font-size: 13px; font-weight: 500; color: #555; margin-bottom: 6px; }

.input-wrapper { position: relative; }
.input-wrapper i { position: absolute; left: 16px; top: 50%; transform: translateY(-50%); color: #bbb; font-size: 15px; }
.input-wrapper input {
  width: 100%;
  padding: 12px 16px 12px 46px;
  border: 2px solid #eee;
  border-radius: 12px;
  font-size: 14px;
  font-family: 'Poppins', sans-serif;
  transition: all 0.3s ease;
  background: #fafafa;
}
.input-wrapper input:focus { outline: none; border-color: #9c27b0; background: #fff; box-shadow: 0 0 0 4px rgba(156, 39, 176, 0.1); }

.password-strength { display: flex; align-items: center; gap: 10px; margin-top: 6px; }
.strength-bar { flex: 1; height: 4px; background: #eee; border-radius: 4px; overflow: hidden; }
.strength-fill { height: 100%; border-radius: 4px; transition: all 0.3s ease; }
.strength-label { font-size: 11px; font-weight: 600; min-width: 60px; text-align: right; }

.password-errors { list-style: none; padding: 0; margin: 6px 0 0; }
.password-errors li { font-size: 11px; color: #ef5350; display: flex; align-items: center; gap: 4px; }
.password-errors li i { font-size: 10px; }

.btn-location {
  width: 100%;
  padding: 12px;
  background: #f3e5f5;
  color: #7b1fa2;
  border: 2px dashed #ce93d8;
  border-radius: 12px;
  font-size: 13px;
  font-weight: 600;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}
.btn-location:hover { background: #e1bee7; }

.location-coords { font-size: 11px; color: #888; margin-top: 4px; }

.map-container { margin-bottom: 16px; }
.leaflet-map { height: 250px; border-radius: 12px; border: 2px solid #ce93d8; z-index: 1; }
.btn-confirm-location {
  width: 100%;
  padding: 10px;
  margin-top: 8px;
  background: linear-gradient(135deg, #7b1fa2, #9c27b0);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 13px;
  font-weight: 600;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  transition: all 0.3s;
}
.btn-confirm-location:hover { transform: translateY(-1px); box-shadow: 0 4px 15px rgba(123, 31, 162, 0.3); }

.btn-register {
  width: 100%;
  padding: 14px;
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
.btn-register:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 8px 25px rgba(156, 39, 176, 0.4); }
.btn-register:disabled { opacity: 0.7; cursor: not-allowed; }

.login-link { text-align: center; margin-top: 20px; font-size: 14px; color: #888; }
.login-link a { color: #9c27b0; text-decoration: none; font-weight: 600; }
.login-link a:hover { text-decoration: underline; }

.footer-decor { text-align: center; margin-top: 20px; color: #ccc; font-size: 12px; }

@media (max-width: 500px) {
  .register-container { padding: 30px 20px; }
  .form-row { flex-direction: column; gap: 0; }
}
</style>
