<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'
import { api } from '@/services/api'

const store = useStore()
const toast = useToast()
const loading = ref(true)

onMounted(async () => {
  try {
    await store.loadVentas()
    await store.loadCategorias()
  } catch {
    toast.error('Error al cargar datos')
  } finally {
    loading.value = false
  }
})

const ventas = computed(() => store.getVentas().slice(0, 20))

function getBadgeClass(metodo: string): string {
  const map: Record<string, string> = { efectivo: 'badge-efectivo', tarjeta: 'badge-tarjeta', transferencia: 'badge-transferencia' }
  return map[metodo] ?? ''
}

async function handleDeleteVenta(id: number) {
  if (!confirm('Eliminar esta venta? Se restaurara el stock.')) return
  try {
    await api.ventas.delete(id)
    await store.loadVentas()
    toast.success('Venta eliminada y stock restaurado')
  } catch (e: any) {
    toast.error(e.message || 'Error al eliminar venta')
  }
}
</script>

<template>
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-history"></i> Historial de Ventas</h1>
    </div>

    <div v-if="loading" class="loading-state"><i class="fas fa-spinner fa-spin"></i> Cargando...</div>

    <div v-else-if="ventas.length > 0" class="table-container">
      <table>
        <thead>
          <tr><th>#</th><th>Cliente</th><th>Total</th><th>Metodo</th><th>Observaciones</th><th>Fecha</th><th></th></tr>
        </thead>
        <tbody>
          <tr v-for="v in ventas" :key="v.id">
            <td><strong>{{ v.id }}</strong></td>
            <td>{{ v.nombreUsuario || 'N/A' }}</td>
            <td class="price">${{ v.total.toFixed(2) }}</td>
            <td><span class="badge" :class="getBadgeClass(v.metodoPago)">{{ v.metodoPago.charAt(0).toUpperCase() + v.metodoPago.slice(1) }}</span></td>
            <td>{{ v.observaciones || '-' }}</td>
            <td>{{ new Date(v.fechaVenta).toLocaleDateString('es-ES') }}</td>
            <td><button class="btn-delete" @click="handleDeleteVenta(v.id)"><i class="fas fa-trash"></i></button></td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="empty-state"><i class="fas fa-receipt"></i><p>No hay ventas registradas aun</p></div>
  </div>
</template>

<style scoped>
.container { max-width: 1100px; margin: 30px auto; padding: 0 20px; }
.page-header h1 { font-size: 26px; color: #333; margin-bottom: 25px; }
.loading-state { text-align: center; padding: 40px; color: #888; }
.table-container { background: white; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06); }
table { width: 100%; border-collapse: collapse; }
thead th { background: #fafafa; padding: 14px 18px; text-align: left; font-size: 13px; font-weight: 600; color: #555; border-bottom: 2px solid #f0f0f0; }
tbody td { padding: 13px 18px; font-size: 14px; color: #444; border-bottom: 1px solid #f5f5f5; }
tbody tr:hover { background: #fdf2f8; }
.price { font-weight: 600; color: #2e7d32; }
.badge { display: inline-block; padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: 600; }
.badge-efectivo { background: #e8f5e9; color: #2e7d32; }
.badge-tarjeta { background: #e3f2fd; color: #1565c0; }
.badge-transferencia { background: #fff3e0; color: #e65100; }
.btn-delete { background: #fce4ec; color: #c62828; border: none; width: 30px; height: 30px; border-radius: 8px; cursor: pointer; font-size: 12px; }
.btn-delete:hover { background: #f8bbd0; }
.empty-state { text-align: center; padding: 50px 20px; color: #aaa; }
.empty-state i { font-size: 48px; margin-bottom: 15px; display: block; }
</style>
