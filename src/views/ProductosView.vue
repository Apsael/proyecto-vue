<script setup lang="ts">
import { ref, computed } from 'vue'
import TopBar from '@/components/TopBar.vue'
import Modal from '@/components/Modal.vue'
import { useStore } from '@/composables/useStore'

const store = useStore()

const message = ref('')
const messageType = ref<'success' | 'error'>('success')
const showAddModal = ref(false)
const showEditModal = ref(false)

const form = ref({
  nombre: '',
  descripcion: '',
  precio: 0,
  stock: 0,
  id_categoria: 0,
})

const editForm = ref({
  id: 0,
  nombre: '',
  descripcion: '',
  precio: 0,
  stock: 0,
  id_categoria: 0,
})

const categorias = computed(() => store.getCategorias())

const productos = computed(() => store.getAllProductos().filter(p => p.activo))

function getCategoriaNombre(id: number): string {
  return categorias.value.find(c => c.id === id)?.nombre ?? 'Sin categoria'
}

function showSuccess(msg: string) {
  message.value = msg
  messageType.value = 'success'
  setTimeout(() => (message.value = ''), 3000)
}

function showError(msg: string) {
  message.value = msg
  messageType.value = 'error'
  setTimeout(() => (message.value = ''), 3000)
}

function handleAdd() {
  if (!form.value.nombre || form.value.precio <= 0) {
    showError('Complete los campos obligatorios.')
    return
  }
  store.addProducto({ ...form.value })
  form.value = { nombre: '', descripcion: '', precio: 0, stock: 0, id_categoria: 0 }
  showAddModal.value = false
  showSuccess('Producto agregado correctamente.')
}

function openEdit(p: { id: number; nombre: string; descripcion: string; precio: number; stock: number; id_categoria: number }) {
  editForm.value = { ...p }
  showEditModal.value = true
}

function handleEdit() {
  if (!editForm.value.nombre || editForm.value.precio <= 0) {
    showError('Complete los campos obligatorios.')
    return
  }
  store.updateProducto(editForm.value.id, { ...editForm.value })
  showEditModal.value = false
  showSuccess('Producto actualizado correctamente.')
}

function handleDelete(id: number) {
  if (confirm('Eliminar este producto?')) {
    store.deleteProducto(id)
    showSuccess('Producto eliminado.')
  }
}
</script>

<template>
  <TopBar />
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-boxes-stacked"></i> Catalogo de Productos</h1>
      <button class="btn-primary" @click="showAddModal = true">
        <i class="fas fa-plus"></i> Nuevo Producto
      </button>
    </div>

    <div v-if="message" class="msg" :class="messageType">
      <i :class="messageType === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-circle'"></i>
      {{ message }}
    </div>

    <div class="table-container">
      <table v-if="productos.length > 0">
        <thead>
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Descripcion</th>
            <th>Precio</th>
            <th>Stock</th>
            <th>Categoria</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="p in productos" :key="p.id">
            <td>{{ p.id }}</td>
            <td><strong>{{ p.nombre }}</strong></td>
            <td>{{ p.descripcion }}</td>
            <td class="price">${{ p.precio.toFixed(2) }}</td>
            <td>
              <span class="badge" :class="p.stock < 20 ? 'badge-stock low' : 'badge-stock'">
                {{ p.stock }} uds
              </span>
            </td>
            <td><span class="badge badge-cat">{{ getCategoriaNombre(p.id_categoria) }}</span></td>
            <td>
              <div class="actions">
                <button class="btn-icon btn-edit" title="Editar" @click="openEdit(p)">
                  <i class="fas fa-pen"></i>
                </button>
                <button class="btn-icon btn-delete" title="Eliminar" @click="handleDelete(p.id)">
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-else class="empty-state">
        <i class="fas fa-box-open"></i>
        <p>No hay productos registrados</p>
      </div>
    </div>
  </div>

  <Modal :visible="showAddModal" title="Agregar Producto" @close="showAddModal = false">
    <form @submit.prevent="handleAdd">
      <div class="form-group">
        <label>Nombre *</label>
        <input type="text" v-model="form.nombre" placeholder="Ej: Helado de Vainilla" required />
      </div>
      <div class="form-group">
        <label>Descripcion</label>
        <textarea v-model="form.descripcion" placeholder="Descripcion del producto..."></textarea>
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>Precio *</label>
          <input type="number" step="0.01" v-model.number="form.precio" placeholder="0.00" min="0.01" required />
        </div>
        <div class="form-group">
          <label>Stock</label>
          <input type="number" v-model.number="form.stock" min="0" />
        </div>
      </div>
      <div class="form-group">
        <label>Categoria</label>
        <select v-model.number="form.id_categoria">
          <option :value="0">-- Seleccionar --</option>
          <option v-for="c in categorias" :key="c.id" :value="c.id">{{ c.nombre }}</option>
        </select>
      </div>
      <div class="modal-actions">
        <button type="button" class="btn-cancel" @click="showAddModal = false">Cancelar</button>
        <button type="submit" class="btn-save"><i class="fas fa-save"></i> Guardar</button>
      </div>
    </form>
  </Modal>

  <Modal :visible="showEditModal" title="Editar Producto" @close="showEditModal = false">
    <form @submit.prevent="handleEdit">
      <div class="form-group">
        <label>Nombre *</label>
        <input type="text" v-model="editForm.nombre" required />
      </div>
      <div class="form-group">
        <label>Descripcion</label>
        <textarea v-model="editForm.descripcion"></textarea>
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>Precio *</label>
          <input type="number" step="0.01" v-model.number="editForm.precio" min="0.01" required />
        </div>
        <div class="form-group">
          <label>Stock</label>
          <input type="number" v-model.number="editForm.stock" min="0" />
        </div>
      </div>
      <div class="form-group">
        <label>Categoria</label>
        <select v-model.number="editForm.id_categoria">
          <option :value="0">-- Seleccionar --</option>
          <option v-for="c in categorias" :key="c.id" :value="c.id">{{ c.nombre }}</option>
        </select>
      </div>
      <div class="modal-actions">
        <button type="button" class="btn-cancel" @click="showEditModal = false">Cancelar</button>
        <button type="submit" class="btn-save"><i class="fas fa-save"></i> Actualizar</button>
      </div>
    </form>
  </Modal>
</template>

<style scoped>
.container {
  max-width: 1100px;
  margin: 30px auto;
  padding: 0 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
}

.page-header h1 {
  font-size: 26px;
  color: #333;
}

.btn-primary {
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 600;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  transition: all 0.3s;
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(233, 30, 99, 0.3);
}

.msg {
  padding: 14px 20px;
  border-radius: 12px;
  margin-bottom: 20px;
  font-size: 14px;
}

.msg.success { background: #f0fff4; color: #2e7d32; border-left: 4px solid #4caf50; }
.msg.error   { background: #fff0f3; color: #c62828; border-left: 4px solid #ef5350; }

.table-container {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
}

table {
  width: 100%;
  border-collapse: collapse;
}

thead th {
  background: #fafafa;
  padding: 14px 18px;
  text-align: left;
  font-size: 13px;
  font-weight: 600;
  color: #555;
  border-bottom: 2px solid #f0f0f0;
}

tbody td {
  padding: 14px 18px;
  font-size: 14px;
  color: #444;
  border-bottom: 1px solid #f5f5f5;
}

tbody tr:hover {
  background: #fdf2f8;
}

.badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}

.badge-cat { background: #ede7f6; color: #673ab7; }
.badge-stock { background: #e8f5e9; color: #2e7d32; }
.badge-stock.low { background: #fff3e0; color: #e65100; }

.actions {
  display: flex;
  gap: 6px;
}

.btn-icon {
  width: 34px;
  height: 34px;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  font-size: 13px;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-edit { background: #e3f2fd; color: #1565c0; }
.btn-delete { background: #fce4ec; color: #c62828; }
.btn-icon:hover { transform: scale(1.1); }

.price { font-weight: 600; color: #2e7d32; }

.empty-state {
  text-align: center;
  padding: 50px 20px;
  color: #aaa;
}

.empty-state i { font-size: 48px; margin-bottom: 15px; display: block; }

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

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  padding: 11px 14px;
  border: 2px solid #eee;
  border-radius: 10px;
  font-size: 14px;
  font-family: 'Poppins', sans-serif;
  transition: all 0.3s;
  background: #fafafa;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #e91e63;
  background: #fff;
}

.form-group textarea { resize: vertical; min-height: 70px; }

.form-row {
  display: flex;
  gap: 12px;
}

.form-row .form-group {
  flex: 1;
}

.modal-actions {
  display: flex;
  gap: 10px;
  margin-top: 20px;
}

.btn-cancel {
  flex: 1;
  padding: 12px;
  background: #f5f5f5;
  color: #666;
  border: none;
  border-radius: 10px;
  font-size: 14px;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  font-weight: 500;
}

.btn-save {
  flex: 1;
  padding: 12px;
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 14px;
  font-weight: 600;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
}
</style>
