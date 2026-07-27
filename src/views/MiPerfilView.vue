<script setup lang="ts">
import { ref, onMounted } from 'vue'
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

onMounted(() => {
  const user = store.getSession()
  if (user) {
    nombre.value = user.nombre
    email.value = user.email
  }
})

async function updateProfile() {
  if (!nombre.value || !email.value) {
    toast.error('Nombre y correo son requeridos')
    return
  }
  try {
    const res = await api.auth.updateProfile(nombre.value, email.value)
    store.setSession({ ...store.getSession()!, nombre: res.nombre, email: res.email })
    toast.success('Perfil actualizado correctamente')
  } catch (e: any) {
    toast.error(e.message || 'Error al actualizar perfil')
  }
}

async function changePassword() {
  if (!currentPassword.value || !newPassword.value) {
    toast.error('Completa todos los campos de contrasena')
    return
  }
  if (newPassword.value !== confirmPassword.value) {
    toast.error('Las contrasenas no coinciden')
    return
  }
  if (newPassword.value.length < 6) {
    toast.error('La nueva contrasena debe tener al menos 6 caracteres')
    return
  }
  try {
    await api.auth.changePassword(currentPassword.value, newPassword.value)
    toast.success('Contrasena actualizada correctamente')
    currentPassword.value = ''
    newPassword.value = ''
    confirmPassword.value = ''
  } catch (e: any) {
    toast.error(e.message || 'Error al cambiar contrasena')
  }
}
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
            <label>Correo Electronico</label>
            <input type="email" v-model="email" required />
          </div>
          <button type="submit" class="btn-save">
            <i class="fas fa-save"></i> Guardar Cambios
          </button>
        </form>
      </div>

      <div class="card">
        <h2><i class="fas fa-lock" style="color:#ff9800"></i> Cambiar Contrasena</h2>
        <form @submit.prevent="changePassword">
          <div class="form-group">
            <label>Contrasena Actual</label>
            <input type="password" v-model="currentPassword" placeholder="Ingrese su contrasena actual" required />
          </div>
          <div class="form-group">
            <label>Nueva Contrasena</label>
            <input type="password" v-model="newPassword" placeholder="Minimo 6 caracteres" required />
          </div>
          <div class="form-group">
            <label>Confirmar Nueva Contrasena</label>
            <input type="password" v-model="confirmPassword" placeholder="Repita la nueva contrasena" required />
          </div>
          <button type="submit" class="btn-password">
            <i class="fas fa-key"></i> Actualizar Contrasena
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container {
  max-width: 900px;
  margin: 30px auto;
  padding: 0 20px;
}

.page-header h1 {
  font-size: 26px;
  color: #333;
  margin-bottom: 25px;
}

.profile-layout {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
}

.card {
  background: white;
  border-radius: 16px;
  padding: 28px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
}

.card h2 {
  font-size: 17px;
  color: #333;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.form-group {
  margin-bottom: 16px;
}

.form-group label {
  display: block;
  font-size: 13px;
  font-weight: 500;
  color: #555;
  margin-bottom: 5px;
}

.form-group input {
  width: 100%;
  padding: 11px 14px;
  border: 2px solid #eee;
  border-radius: 10px;
  font-size: 14px;
  font-family: 'Poppins', sans-serif;
  background: #fafafa;
  transition: all 0.3s;
}

.form-group input:focus {
  outline: none;
  border-color: #e91e63;
  background: #fff;
}

.btn-save {
  width: 100%;
  padding: 12px;
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 14px;
  font-weight: 600;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  transition: all 0.3s;
}

.btn-save:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(233, 30, 99, 0.3);
}

.btn-password {
  width: 100%;
  padding: 12px;
  background: linear-gradient(135deg, #ff9800, #ffb74d);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 14px;
  font-weight: 600;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  transition: all 0.3s;
}

.btn-password:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(255, 152, 0, 0.3);
}

@media (max-width: 768px) {
  .profile-layout { grid-template-columns: 1fr; }
}
</style>
