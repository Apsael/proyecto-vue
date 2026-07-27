<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'

const store = useStore()
const toast = useToast()

onMounted(async () => {
  try {
    await store.loadMisCompras()
  } catch {
    toast.error('Error al cargar el historial de compras')
  }
})

const ventas = computed(() => store.getVentas())

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
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-history"></i> Mis Compras</h1>
    </div>

    <div v-if="ventas.length === 0" class="empty-state">
      <i class="fas fa-shopping-bag"></i>
      <p>Aun no has realizado ninguna compra</p>
      <router-link to="/" class="btn-back"><i class="fas fa-arrow-left"></i> Explorar productos</router-link>
    </div>

    <div v-else class="purchases-list">
      <div v-for="venta in ventas" :key="venta.id" class="purchase-card">
        <div class="purchase-header">
          <div>
            <span class="purchase-id">#{{ venta.id }}</span>
            <span class="purchase-date">{{ new Date(venta.fechaVenta).toLocaleDateString('es-ES', { year: 'numeric', month: 'long', day: 'numeric' }) }}</span>
          </div>
          <div class="purchase-meta">
            <span class="badge" :class="getBadgeClass(venta.metodoPago)">
              {{ venta.metodoPago.charAt(0).toUpperCase() + venta.metodoPago.slice(1) }}
            </span>
            <span class="purchase-total">${{ venta.total.toFixed(2) }}</span>
          </div>
        </div>
        <div class="purchase-details">
          <div v-for="d in venta.detalles" :key="d.id" class="detail-row">
            <span>{{ d.nombreProducto }} x{{ d.cantidad }}</span>
            <span>${{ d.subtotal.toFixed(2) }}</span>
          </div>
        </div>
        <div v-if="venta.observaciones" class="purchase-notes">
          <i class="fas fa-sticky-note"></i> {{ venta.observaciones }}
        </div>
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

.empty-state {
  text-align: center;
  padding: 80px 20px;
  background: white;
  border-radius: 16px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
}

.empty-state i {
  font-size: 56px;
  color: #ddd;
  margin-bottom: 16px;
  display: block;
}

.empty-state p {
  color: #888;
  margin-bottom: 20px;
}

.btn-back {
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  padding: 12px 24px;
  border-radius: 12px;
  text-decoration: none;
  font-size: 14px;
  font-weight: 600;
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.purchases-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.purchase-card {
  background: white;
  border-radius: 14px;
  padding: 22px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.05);
}

.purchase-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 14px;
  padding-bottom: 12px;
  border-bottom: 1px solid #f0f0f0;
}

.purchase-id {
  font-weight: 700;
  color: #333;
  font-size: 16px;
  margin-right: 12px;
}

.purchase-date {
  font-size: 13px;
  color: #888;
}

.purchase-meta {
  display: flex;
  align-items: center;
  gap: 14px;
}

.purchase-total {
  font-size: 20px;
  font-weight: 700;
  color: #2e7d32;
}

.badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}

.badge-efectivo { background: #e8f5e9; color: #2e7d32; }
.badge-tarjeta { background: #e3f2fd; color: #1565c0; }
.badge-transferencia { background: #fff3e0; color: #e65100; }

.purchase-details {
  margin-bottom: 8px;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  padding: 6px 0;
  font-size: 14px;
  color: #555;
}

.purchase-notes {
  margin-top: 10px;
  padding: 10px 14px;
  background: #fafafa;
  border-radius: 8px;
  font-size: 13px;
  color: #888;
}

.purchase-notes i {
  margin-right: 6px;
}
</style>
