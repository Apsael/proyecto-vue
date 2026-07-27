<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import Modal from '@/components/Modal.vue'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'
import { api } from '@/services/api'
import type { UsuarioResponse } from '@/services/api'

const store = useStore()
const toast = useToast()

const usuarios = ref<UsuarioResponse[]>([])
const loading = ref(true)
const showAddModal = ref(false)
const showEditModal = ref(false)

const form = ref({ nombre: '', email: '', password: '', rol: 'cliente' })
const editForm = ref({ id: 0, nombre: '', email: '', rol: 'cliente', activo: true })

onMounted(loadUsuarios)

async function loadUsuarios() {
  try {
    usuarios.value = await api.usuarios.getAll()
  } catch {
    toast.error('Error al cargar usuarios')
  } finally {
    loading.value = false
  }
}

async function handleAdd() {
  if (!form.value.nombre || !form.value.email || !form.value.password) {
    toast.error('Complete todos los campos')
    return
  }
  try {
    await api.usuarios.create(form.value)
    await loadUsuarios()
    form.value = { nombre: '', email: '', password: '', rol: 'cliente' }
    showAddModal.value = false
    toast.success('Usuario creado correctamente')
  } catch (e: any) {
    toast.error(e.message || 'Error al crear usuario')
  }
}

function openEdit(u: UsuarioResponse) {
  editForm.value = { id: u.id, nombre: u.nombre, email: u.email, rol: u.rol, activo: u.activo }
  showEditModal.value = true
}

async function handleEdit() {
  try {
    await api.usuarios.update(editForm.value.id, {
      nombre: editForm.value.nombre,
      email: editForm.value.email,
      rol: editForm.value.rol,
      activo: editForm.value.activo,
    })
    await loadUsuarios()
    showEditModal.value = false
    toast.success('Usuario actualizado correctamente')
  } catch (e: any) {
    toast.error(e.message || 'Error al actualizar usuario')
  }
}

async function handleDelete(id: number) {
  if (!confirm('Desactivar este usuario?')) return
  try {
    await api.usuarios.delete(id)
    await loadUsuarios()
    toast.success('Usuario desactivado')
  } catch (e: any) {
    toast.error(e.message || 'Error al desactivar usuario')
  }
}
</script>

<template>
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-users"></i> Gestion de Usuarios</h1>
      <button class="btn-primary" @click="showAddModal = true">
        <i class="fas fa-plus"></i> Nuevo Usuario
      </button>
    </div>

    <div v-if="loading" class="loading-state"><i class="fas fa-spinner fa-spin"></i> Cargando...</div>

    <div v-else-if="usuarios.length > 0" class="table-container">
      <table>
        <thead>
          <tr><th>ID</th><th>Nombre</th><th>Email</th><th>Rol</th><th>Estado</th><th>Registro</th><th>Acciones</th></tr>
        </thead>
        <tbody>
          <tr v-for="u in usuarios" :key="u.id">
            <td>{{ u.id }}</td>
            <td><strong>{{ u.nombre }}</strong></td>
            <td>{{ u.email }}</td>
            <td><span class="badge" :class="u.rol === 'admin' ? 'badge-admin' : 'badge-cliente'">{{ u.rol }}</span></td>
            <td><span class="badge" :class="u.activo ? 'badge-active' : 'badge-inactive'">{{ u.activo ? 'Activo' : 'Inactivo' }}</span></td>
            <td>{{ new Date(u.fechaCreacion).toLocaleDateString('es-ES') }}</td>
            <td>
              <div class="actions">
                <button class="btn-icon btn-edit" title="Editar" @click="openEdit(u)"><i class="fas fa-pen"></i></button>
                <button class="btn-icon btn-delete" title="Desactivar" @click="handleDelete(u.id)"><i class="fas fa-user-slash"></i></button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="empty-state"><i class="fas fa-users"></i><p>No hay usuarios registrados</p></div>
  </div>

  <Modal :visible="showAddModal" title="Crear Usuario" @close="showAddModal = false">
    <form @submit.prevent="handleAdd">
      <div class="form-group"><label>Nombre *</label><input type="text" v-model="form.nombre" placeholder="Nombre completo" required /></div>
      <div class="form-group"><label>Email *</label><input type="email" v-model="form.email" placeholder="correo@ejemplo.com" required /></div>
      <div class="form-group"><label>Contrasena *</label><input type="password" v-model="form.password" placeholder="Minimo 6 caracteres" required /></div>
      <div class="form-group"><label>Rol</label>
        <select v-model="form.rol">
          <option value="cliente">Cliente</option>
          <option value="admin">Administrador</option>
        </select>
      </div>
      <div class="modal-actions">
        <button type="button" class="btn-cancel" @click="showAddModal = false">Cancelar</button>
        <button type="submit" class="btn-save"><i class="fas fa-save"></i> Crear</button>
      </div>
    </form>
  </Modal>

  <Modal :visible="showEditModal" title="Editar Usuario" @close="showEditModal = false">
    <form @submit.prevent="handleEdit">
      <div class="form-group"><label>Nombre *</label><input type="text" v-model="editForm.nombre" required /></div>
      <div class="form-group"><label>Email *</label><input type="email" v-model="editForm.email" required /></div>
      <div class="form-group"><label>Rol</label>
        <select v-model="editForm.rol">
          <option value="cliente">Cliente</option>
          <option value="admin">Administrador</option>
        </select>
      </div>
      <div class="form-group">
        <label class="checkbox-label">
          <input type="checkbox" v-model="editForm.activo" /> Activo
        </label>
      </div>
      <div class="modal-actions">
        <button type="button" class="btn-cancel" @click="showEditModal = false">Cancelar</button>
        <button type="submit" class="btn-save"><i class="fas fa-save"></i> Actualizar</button>
      </div>
    </form>
  </Modal>
</template>

<style scoped>
.container { max-width: 1100px; margin: 30px auto; padding: 0 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; }
.page-header h1 { font-size: 26px; color: #333; }

.btn-primary {
  background: linear-gradient(135deg, #e91e63, #f06292); color: white; border: none;
  padding: 12px 24px; border-radius: 12px; font-size: 14px; font-weight: 600;
  font-family: 'Poppins', sans-serif; cursor: pointer; transition: all 0.3s;
  display: inline-flex; align-items: center; gap: 8px;
}
.btn-primary:hover { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(233, 30, 99, 0.3); }

.loading-state { text-align: center; padding: 40px; color: #888; }

.table-container { background: white; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06); }
table { width: 100%; border-collapse: collapse; }
thead th { background: #fafafa; padding: 14px 18px; text-align: left; font-size: 13px; font-weight: 600; color: #555; border-bottom: 2px solid #f0f0f0; }
tbody td { padding: 14px 18px; font-size: 14px; color: #444; border-bottom: 1px solid #f5f5f5; }
tbody tr:hover { background: #fdf2f8; }

.badge { display: inline-block; padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: 600; }
.badge-admin { background: #ede7f6; color: #673ab7; }
.badge-cliente { background: #e3f2fd; color: #1565c0; }
.badge-active { background: #e8f5e9; color: #2e7d32; }
.badge-inactive { background: #fce4ec; color: #c62828; }

.actions { display: flex; gap: 6px; }
.btn-icon { width: 34px; height: 34px; border-radius: 8px; border: none; cursor: pointer; font-size: 13px; transition: all 0.2s; display: flex; align-items: center; justify-content: center; }
.btn-edit { background: #e3f2fd; color: #1565c0; }
.btn-delete { background: #fce4ec; color: #c62828; }
.btn-icon:hover { transform: scale(1.1); }

.empty-state { text-align: center; padding: 50px 20px; color: #aaa; }
.empty-state i { font-size: 48px; margin-bottom: 15px; display: block; }

.form-group { margin-bottom: 16px; }
.form-group label { display: block; font-size: 13px; font-weight: 500; color: #555; margin-bottom: 5px; }
.form-group input, .form-group select {
  width: 100%; padding: 11px 14px; border: 2px solid #eee; border-radius: 10px;
  font-size: 14px; font-family: 'Poppins', sans-serif; transition: all 0.3s; background: #fafafa;
}
.form-group input:focus, .form-group select:focus { outline: none; border-color: #e91e63; background: #fff; }
.checkbox-label { display: flex; align-items: center; gap: 8px; cursor: pointer; }
.modal-actions { display: flex; gap: 10px; margin-top: 20px; }
.btn-cancel { flex: 1; padding: 12px; background: #f5f5f5; color: #666; border: none; border-radius: 10px; font-size: 14px; font-family: 'Poppins', sans-serif; cursor: pointer; font-weight: 500; }
.btn-save { flex: 1; padding: 12px; background: linear-gradient(135deg, #e91e63, #f06292); color: white; border: none; border-radius: 10px; font-size: 14px; font-weight: 600; font-family: 'Poppins', sans-serif; cursor: pointer; }
</style>
