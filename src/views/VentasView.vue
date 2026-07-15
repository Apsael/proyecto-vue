<script setup lang="ts">
import { ref, computed } from 'vue'
import TopBar from '@/components/TopBar.vue'
import { useStore } from '@/composables/useStore'
import type { MetodoPago } from '@/types'

const store = useStore()

const message = ref('')
const messageType = ref<'success' | 'error'>('success')

const saleItems = ref<{ productoId: number; cantidad: number }[]>([
  { productoId: 0, cantidad: 1 },
])
const metodoPago = ref<MetodoPago>('efectivo')
const observaciones = ref('')

const productos = computed(() => store.getProductos().filter(p => p.stock > 0))
const ventas = computed(() => store.getVentas().slice(0, 20))

function getVendedor(id: number): string {
  return store.getUsuarioById(id)?.nombre ?? 'Desconocido'
}

const totalVenta = computed(() => {
  return saleItems.value.reduce((sum, item) => {
    const prod = productos.value.find(p => p.id === item.productoId)
    return sum + (prod ? prod.precio * item.cantidad : 0)
  }, 0)
})

function addItem() {
  saleItems.value.push({ productoId: 0, cantidad: 1 })
}

function removeItem(index: number) {
  if (saleItems.value.length > 1) {
    saleItems.value.splice(index, 1)
  }
}

function showMsg(msg: string, type: 'success' | 'error') {
  message.value = msg
  messageType.value = type
  setTimeout(() => (message.value = ''), 4000)
}

function handleSale() {
  const validItems = saleItems.value.filter(i => i.productoId > 0 && i.cantidad > 0)
  if (validItems.length === 0) {
    showMsg('Agrega al menos un producto a la venta.', 'error')
    return
  }

  const user = store.getSession()
  if (!user) return

  const error = store.addVenta(user.id, validItems, metodoPago.value, observaciones.value)
  if (error) {
    showMsg(error, 'error')
  } else {
    showMsg(`Venta registrada correctamente. Total: $${totalVenta.value.toFixed(2)}`, 'success')
    saleItems.value = [{ productoId: 0, cantidad: 1 }]
    metodoPago.value = 'efectivo'
    observaciones.value = ''
  }
}

function handleDeleteVenta(id: number) {
  if (confirm('Eliminar esta venta?')) {
    store.deleteVenta(id)
    showMsg('Venta eliminada.', 'success')
  }
}

function getBadgeClass(metodo: string): string {
  const map: Record<string, string> = {
    efectivo: 'badge-efectivo',
    tarjeta: 'badge-tarjeta',
    transferencia: 'badge-transferencia',
  }
  return map[metodo] ?? ''
}
</script>

<template>
  <TopBar />
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-cash-register"></i> Registro de Ventas</h1>
    </div>

    <div v-if="message" class="msg" :class="messageType">
      <i :class="messageType === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-circle'"></i>
      {{ message }}
    </div>

    <div class="two-col">
      <div class="card">
        <h2><i class="fas fa-cart-plus" style="color:#e91e63"></i> Nueva Venta</h2>
        <form @submit.prevent="handleSale">
          <div class="sale-items">
            <div v-for="(item, idx) in saleItems" :key="idx" class="sale-item">
              <select v-model.number="item.productoId" required>
                <option :value="0">-- Producto --</option>
                <option v-for="p in productos" :key="p.id" :value="p.id">
                  {{ p.nombre }} - ${{ p.precio.toFixed(2) }} (Stock: {{ p.stock }})
                </option>
              </select>
              <input type="number" v-model.number="item.cantidad" min="1" placeholder="Cant." required />
              <button type="button" class="btn-remove-item" @click="removeItem(idx)">
                <i class="fas fa-times"></i>
              </button>
            </div>
          </div>

          <button type="button" class="btn-add-item" @click="addItem">
            <i class="fas fa-plus"></i> Agregar otro producto
          </button>

          <div class="total-display">Total: ${{ totalVenta.toFixed(2) }}</div>

          <div class="form-group">
            <label>Metodo de Pago</label>
            <select v-model="metodoPago">
              <option value="efectivo">Efectivo</option>
              <option value="tarjeta">Tarjeta</option>
              <option value="transferencia">Transferencia</option>
            </select>
          </div>

          <div class="form-group">
            <label>Observaciones</label>
            <textarea v-model="observaciones" placeholder="Notas adicionales..."></textarea>
          </div>

          <button type="submit" class="btn-submit">
            <i class="fas fa-check-circle"></i> Registrar Venta
          </button>
        </form>
      </div>

      <div class="card">
        <h2><i class="fas fa-history" style="color:#ff9800"></i> Ultimas Ventas</h2>
        <table v-if="ventas.length > 0">
          <thead>
            <tr>
              <th>#</th>
              <th>Total</th>
              <th>Pago</th>
              <th>Vendedor</th>
              <th>Fecha</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="v in ventas" :key="v.id">
              <td><strong>{{ v.id }}</strong></td>
              <td class="price">${{ v.total.toFixed(2) }}</td>
              <td>
                <span class="badge" :class="getBadgeClass(v.metodo_pago)">
                  {{ v.metodo_pago.charAt(0).toUpperCase() + v.metodo_pago.slice(1) }}
                </span>
              </td>
              <td>{{ getVendedor(v.id_usuario) }}</td>
              <td>{{ new Date(v.fecha_venta).toLocaleDateString('es-ES') }}</td>
              <td>
                <button class="btn-delete" @click="handleDeleteVenta(v.id)">
                  <i class="fas fa-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
        <div v-else class="empty-state">
          <i class="fas fa-receipt"></i>
          <p>No hay ventas registradas aun</p>
        </div>
      </div>
    </div>
  </div>
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

.msg {
  padding: 14px 20px;
  border-radius: 12px;
  margin-bottom: 20px;
  font-size: 14px;
}

.msg.success { background: #f0fff4; color: #2e7d32; border-left: 4px solid #4caf50; }
.msg.error   { background: #fff0f3; color: #c62828; border-left: 4px solid #ef5350; }

.two-col {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 25px;
}

.card {
  background: white;
  border-radius: 16px;
  padding: 28px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
}

.card h2 {
  font-size: 18px;
  color: #333;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.form-group {
  margin-bottom: 14px;
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

.form-group textarea {
  resize: vertical;
  min-height: 60px;
}

.sale-items {
  max-height: 300px;
  overflow-y: auto;
}

.sale-item {
  display: flex;
  gap: 10px;
  align-items: center;
  margin-bottom: 10px;
  padding: 10px;
  background: #fafafa;
  border-radius: 10px;
}

.sale-item select { flex: 2; }
.sale-item input  { flex: 1; }

.sale-item select,
.sale-item input {
  padding: 9px 12px;
  border: 2px solid #eee;
  border-radius: 8px;
  font-size: 13px;
  font-family: 'Poppins', sans-serif;
  background: white;
}

.sale-item select:focus,
.sale-item input:focus {
  outline: none;
  border-color: #e91e63;
}

.btn-remove-item {
  background: #fce4ec;
  color: #c62828;
  border: none;
  width: 32px;
  height: 32px;
  border-radius: 8px;
  cursor: pointer;
  font-size: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.btn-remove-item:hover { background: #f8bbd0; }

.btn-add-item {
  background: #e8f5e9;
  color: #2e7d32;
  border: 2px dashed #a5d6a7;
  padding: 10px;
  border-radius: 10px;
  width: 100%;
  font-size: 13px;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s;
  margin-bottom: 14px;
}

.btn-add-item:hover { background: #c8e6c9; }

.total-display {
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  padding: 16px 20px;
  border-radius: 12px;
  text-align: right;
  font-size: 20px;
  font-weight: 700;
  margin-bottom: 14px;
}

.btn-submit {
  width: 100%;
  padding: 14px;
  background: linear-gradient(135deg, #2e7d32, #66bb6a);
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 15px;
  font-weight: 600;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-submit:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(46, 125, 50, 0.3);
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
  padding: 13px 18px;
  font-size: 14px;
  color: #444;
  border-bottom: 1px solid #f5f5f5;
}

tbody tr:hover { background: #fdf2f8; }

.badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}

.badge-efectivo   { background: #e8f5e9; color: #2e7d32; }
.badge-tarjeta    { background: #e3f2fd; color: #1565c0; }
.badge-transferencia { background: #fff3e0; color: #e65100; }

.btn-delete {
  background: #fce4ec;
  color: #c62828;
  border: none;
  width: 30px;
  height: 30px;
  border-radius: 8px;
  cursor: pointer;
  font-size: 12px;
}

.btn-delete:hover { background: #f8bbd0; }

.price { font-weight: 600; color: #2e7d32; }

.empty-state {
  text-align: center;
  padding: 40px 20px;
  color: #aaa;
}

.empty-state i { font-size: 40px; margin-bottom: 10px; display: block; }

@media (max-width: 768px) {
  .two-col { grid-template-columns: 1fr; }
}
</style>
