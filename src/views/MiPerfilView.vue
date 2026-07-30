<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'
import { api } from '@/services/api'

const store = useStore()
const toast = useToast()

const nombre = ref('')
const email = ref('')
const currentPassword = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const latitud = ref(-17.7853)
const longitud = ref(-63.1806)
const showMap = ref(false)
const mapInitialized = ref(false)
let mapInstance: any = null
let markerInstance: any = null

const passwordErrors = ref<string[]>([])

onMounted(() => {
  const user = store.getSession()
  if (user) {
    nombre.value = user.nombre
    email.value = user.email
    if (user.latitud && user.longitud) {
      latitud.value = user.latitud
      longitud.value = user.longitud
    }
  }
})

function checkPasswordStrength(pwd: string) {
  const errors: string[] = []
  if (pwd && pwd.length < 8) errors.push('Mínimo 8 caracteres')
  if (pwd && !/[A-Z]/.test(pwd)) errors.push('Al menos una mayúscula')
  if (pwd && !/[a-z]/.test(pwd)) errors.push('Al menos una minúscula')
  if (pwd && !/\d/.test(pwd)) errors.push('Al menos un número')
  if (pwd && !/[^a-zA-Z\d]/.test(pwd)) errors.push('Al menos un carácter especial')
  passwordErrors.value = errors
}

function initMap() {
  showMap.value = true
  setTimeout(() => {
    const L = (window as any).L
    if (!L || mapInitialized.value) return
    mapInitialized.value = true
    mapInstance = L.map('profile-map').setView([latitud.value, longitud.value], 14)
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

async function updateProfile() {
  if (!nombre.value || !email.value) {
    toast.error('Nombre y correo son requeridos')
    return
  }
  try {
    const res = await api.auth.updateProfile(nombre.value, email.value)
    store.setSession({ ...store.getSession()!, nombre: res.nombre, email: res.email, latitud: res.latitud, longitud: res.longitud })
    toast.success('Perfil actualizado correctamente')
  } catch (e: any) {
    toast.error(e.message || 'Error al actualizar perfil')
  }
}

async function updateLocation() {
  try {
    await api.auth.updateUbicacion(latitud.value, longitud.value)
    const session = store.getSession()
    if (session) {
      store.setSession({ ...session, latitud: latitud.value, longitud: longitud.value })
    }
    removeMap()
    toast.success('Ubicación actualizada correctamente')
  } catch (e: any) {
    toast.error(e.message || 'Error al actualizar ubicación')
  }
}

async function changePassword() {
  if (!currentPassword.value || !newPassword.value) {
    toast.error('Completa todos los campos de contraseña')
    return
  }
  if (newPassword.value !== confirmPassword.value) {
    toast.error('Las contraseñas no coinciden')
    return
  }
  if (passwordErrors.value.length > 0) {
    toast.error('La contraseña no cumple los requisitos de seguridad')
    return
  }
  try {
    await api.auth.changePassword(currentPassword.value, newPassword.value)
    toast.success('Contraseña actualizada correctamente')
    currentPassword.value = ''
    newPassword.value = ''
    confirmPassword.value = ''
    passwordErrors.value = []
  } catch (e: any) {
    toast.error(e.message || 'Error al cambiar contraseña')
  }
}

onUnmounted(() => {
  removeMap()
})
</script>

<template>
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-user-circle"></i> Mi Perfil</h1>
    </div>

    <div class="profile-layout">
      <div class="card">
        <h2><i class="fas fa-user" style="color:#e91e63"></i> Datos Personales</h2>
        <form @submit.prevent="updateProfile">
          <div class="form-group">
            <label>Nombre Completo</label>
            <input type="text" v-model="nombre" required />
          </div>
          <div class="form-group">
            <label>Correo Electrónico</label>
            <input type="email" v-model="email" required />
          </div>
          <button type="submit" class="btn-save">
            <i class="fas fa-save"></i> Guardar Cambios
          </button>
        </form>
      </div>

      <div class="card">
        <h2><i class="fas fa-lock" style="color:#ff9800"></i> Cambiar Contraseña</h2>
        <form @submit.prevent="changePassword">
          <div class="form-group">
            <label>Contraseña Actual</label>
            <input type="password" v-model="currentPassword" placeholder="Ingrese su contraseña actual" required />
          </div>
          <div class="form-group">
            <label>Nueva Contraseña</label>
            <input type="password" v-model="newPassword" @input="checkPasswordStrength(($event.target as HTMLInputElement).value)" placeholder="Mínimo 8 caracteres con mayúsculas, números y símbolos" required />
          </div>
          <ul v-if="passwordErrors.length > 0" class="password-errors">
            <li v-for="err in passwordErrors" :key="err"><i class="fas fa-times-circle"></i> {{ err }}</li>
          </ul>
          <div class="form-group">
            <label>Confirmar Nueva Contraseña</label>
            <input type="password" v-model="confirmPassword" placeholder="Repita la nueva contraseña" required />
          </div>
          <button type="submit" class="btn-password">
            <i class="fas fa-key"></i> Actualizar Contraseña
          </button>
        </form>
      </div>

      <div class="card">
        <h2><i class="fas fa-map-marker-alt" style="color:#2196f3"></i> Mi Ubicación</h2>
        <p class="coords-display" v-if="!showMap">
          <i class="fas fa-location-dot"></i>
          Lat: <strong>{{ latitud.toFixed(6) }}</strong> |
          Lng: <strong>{{ longitud.toFixed(6) }}</strong>
        </p>
        <button type="button" class="btn-location" @click="initMap" v-if="!showMap">
          <i class="fas fa-map"></i> Actualizar ubicación en el mapa
        </button>

        <div v-if="showMap" class="map-section">
          <div id="profile-map" class="leaflet-map"></div>
          <div class="map-actions">
            <button type="button" class="btn-cancel-map" @click="removeMap">Cancelar</button>
            <button type="button" class="btn-save-map" @click="updateLocation">
              <i class="fas fa-check"></i> Guardar ubicación
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container { max-width: 900px; margin: 30px auto; padding: 0 20px; }
.page-header h1 { font-size: 26px; color: #333; margin-bottom: 25px; }

.profile-layout { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }

.card {
  background: white;
  border-radius: 16px;
  padding: 28px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
  transition: transform 0.3s ease;
}

.card h2 { font-size: 17px; color: #333; margin-bottom: 20px; display: flex; align-items: center; gap: 10px; }

.form-group { margin-bottom: 16px; }
.form-group label { display: block; font-size: 13px; font-weight: 500; color: #555; margin-bottom: 5px; }
.form-group input {
  width: 100%; padding: 11px 14px; border: 2px solid #eee; border-radius: 10px;
  font-size: 14px; font-family: 'Poppins', sans-serif; background: #fafafa; transition: all 0.3s;
}
.form-group input:focus { outline: none; border-color: #e91e63; background: #fff; }

.password-errors { list-style: none; padding: 0; margin: 6px 0; }
.password-errors li { font-size: 11px; color: #ef5350; display: flex; align-items: center; gap: 4px; padding: 2px 0; }
.password-errors li i { font-size: 10px; }

.btn-save {
  width: 100%; padding: 12px;
  background: linear-gradient(135deg, #e91e63, #f06292); color: white; border: none;
  border-radius: 10px; font-size: 14px; font-weight: 600; font-family: 'Poppins', sans-serif;
  cursor: pointer; display: flex; align-items: center; justify-content: center; gap: 8px;
  transition: all 0.3s;
}
.btn-save:hover { transform: translateY(-2px); box-shadow: 0 6px 20px rgba(233, 30, 99, 0.3); }

.btn-password {
  width: 100%; padding: 12px;
  background: linear-gradient(135deg, #ff9800, #ffb74d); color: white; border: none;
  border-radius: 10px; font-size: 14px; font-weight: 600; font-family: 'Poppins', sans-serif;
  cursor: pointer; display: flex; align-items: center; justify-content: center; gap: 8px;
  transition: all 0.3s;
}
.btn-password:hover { transform: translateY(-2px); box-shadow: 0 6px 20px rgba(255, 152, 0, 0.3); }

.coords-display { font-size: 13px; color: #555; margin-bottom: 12px; display: flex; align-items: center; gap: 6px; }
.coords-display i { color: #e91e63; }

.btn-location {
  width: 100%; padding: 12px;
  background: #e3f2fd; color: #1565c0; border: 2px dashed #90caf9;
  border-radius: 10px; font-size: 13px; font-weight: 600; font-family: 'Poppins', sans-serif;
  cursor: pointer; display: flex; align-items: center; justify-content: center; gap: 8px;
  transition: all 0.3s;
}
.btn-location:hover { background: #bbdefb; }

.map-section { margin-top: 10px; }
.leaflet-map { height: 280px; border-radius: 12px; border: 2px solid #90caf9; z-index: 1; }

.map-actions { display: flex; gap: 10px; margin-top: 10px; }
.btn-cancel-map {
  flex: 1; padding: 10px; background: #f5f5f5; color: #666; border: none;
  border-radius: 10px; font-size: 13px; font-family: 'Poppins', sans-serif; cursor: pointer;
}
.btn-save-map {
  flex: 1; padding: 10px; background: linear-gradient(135deg, #2196f3, #42a5f5); color: white;
  border: none; border-radius: 10px; font-size: 13px; font-weight: 600; font-family: 'Poppins', sans-serif;
  cursor: pointer; display: flex; align-items: center; justify-content: center; gap: 6px;
}
.btn-save-map:hover { box-shadow: 0 4px 15px rgba(33, 150, 243, 0.3); }

@media (max-width: 768px) { .profile-layout { grid-template-columns: 1fr; } }
</style>
