<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'

const store = useStore()
const toast = useToast()
const selectedVenta = ref<any>(null)

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

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('es-ES', {
    year: 'numeric', month: 'long', day: 'numeric',
    hour: '2-digit', minute: '2-digit'
  })
}

function showReceipt(venta: any) {
  selectedVenta.value = venta
}

function closeReceipt() {
  selectedVenta.value = null
}

function printReceipt() {
  const content = document.getElementById('receipt-content')
  if (!content) return
  const win = window.open('', '_blank')
  if (!win) return
  win.document.write(`
    <html><head><title>Recibo #${selectedVenta.value.id}</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
    <style>
      body { font-family: Arial, sans-serif; padding: 40px; color: #333; }
      .receipt { max-width: 700px; margin: 0 auto; }
      .header { text-align: center; border-bottom: 2px solid #e91e63; padding-bottom: 20px; margin-bottom: 20px; }
      .header h1 { color: #e91e63; margin: 0; font-size: 24px; }
      .header p { color: #888; margin: 5px 0 0; }
      .info { display: flex; justify-content: space-between; margin-bottom: 20px; }
      .info-box { background: #f9f9f9; padding: 15px; border-radius: 8px; flex: 1; margin: 0 5px; }
      .info-box p { margin: 3px 0; font-size: 13px; }
      table { width: 100%; border-collapse: collapse; margin: 20px 0; }
      th { background: #fce4ec; color: #e91e63; padding: 10px; text-align: left; }
      td { padding: 10px; border-bottom: 1px solid #eee; }
      .total { text-align: right; font-size: 18px; font-weight: 700; color: #2e7d32; margin-top: 10px; }
      .footer { text-align: center; margin-top: 30px; color: #999; font-size: 12px; border-top: 1px solid #eee; padding-top: 20px; }
      @media print { body { padding: 20px; } .no-print { display: none; } }
    </style></head>
    <body>
      <div class="receipt">
        <div class="header">
          <h1 style="margin:0;">La Dolce Vita</h1>
          <p>Heladería Artesanal</p>
        </div>
        <div class="info">
          <div class="info-box">
            <p><strong>Recibo #${selectedVenta.value.id}</strong></p>
            <p>${formatDate(selectedVenta.value.fechaVenta)}</p>
          </div>
          <div class="info-box">
            <p><strong>Cliente:</strong> ${selectedVenta.value.nombreUsuario || 'N/A'}</p>
            <p><strong>Método:</strong> ${selectedVenta.value.metodoPago}</p>
          </div>
        </div>
        <table>
          <thead><tr><th>Producto</th><th>Cant.</th><th>P.Unit.</th><th>Subtotal</th></tr></thead>
          <tbody>
            ${selectedVenta.value.detalles.map((d: any) => `
              <tr><td>${d.nombreProducto}</td><td>${d.cantidad}</td><td>$${d.precioUnitario.toFixed(2)}</td><td>$${d.subtotal.toFixed(2)}</td></tr>
            `).join('')}
          </tbody>
        </table>
        <div class="total">Total: $${selectedVenta.value.total.toFixed(2)}</div>
        ${selectedVenta.value.observaciones ? `<p style="color:#888;"><i>Observaciones: ${selectedVenta.value.observaciones}</i></p>` : ''}
        <div class="footer">
          <p>Heladería La Dolce Vita &copy; 2026 - Calle Beni #123, Santa Cruz, Bolivia</p>
          <p>info@ladolcevita.com | Tel: +591 7000 1234</p>
        </div>
      </div>
      <div class="no-print" style="text-align:center;margin-top:20px;">
        <button onclick="window.print()" style="padding:10px 24px;background:#e91e63;color:white;border:none;border-radius:8px;cursor:pointer;font-size:14px;"><i class="fas fa-print"></i> Imprimir</button>
        <button onclick="window.close()" style="padding:10px 24px;background:#f5f5f5;color:#666;border:none;border-radius:8px;cursor:pointer;font-size:14px;margin-left:10px;">Cerrar</button>
      </div>
    </body></html>
  `)
  win.document.close()
}
</script>

<template>
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-history"></i> Mis Compras</h1>
    </div>

    <div v-if="ventas.length === 0" class="empty-state">
      <i class="fas fa-shopping-bag"></i>
      <p>Aún no has realizado ninguna compra</p>
      <router-link to="/" class="btn-back"><i class="fas fa-arrow-left"></i> Explorar productos</router-link>
    </div>

    <div v-else class="purchases-list">
      <div v-for="venta in ventas" :key="venta.id" class="purchase-card">
        <div class="purchase-header">
          <div>
            <span class="purchase-id">#{{ venta.id }}</span>
            <span class="purchase-date">{{ formatDate(venta.fechaVenta) }}</span>
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
        <div class="purchase-footer">
          <div v-if="venta.observaciones" class="purchase-notes">
            <i class="fas fa-sticky-note"></i> {{ venta.observaciones }}
          </div>
          <button class="btn-receipt" @click="showReceipt(venta)">
            <i class="fas fa-file-invoice"></i> Ver Recibo
          </button>
        </div>
      </div>
    </div>

    <Teleport to="body">
      <Transition name="modal">
        <div v-if="selectedVenta" class="receipt-overlay" @click.self="closeReceipt">
          <div class="receipt-modal">
            <div class="receipt-header">
              <h2><i class="fas fa-file-invoice"></i> Recibo #{{ selectedVenta.id }}</h2>
              <button class="btn-close" @click="closeReceipt"><i class="fas fa-times"></i></button>
            </div>
            <div id="receipt-content" class="receipt-content">
              <div class="receipt-brand">
                <img src="/logo.png" alt="La Dolce Vita" class="receipt-logo" />
                <h3>La Dolce Vita</h3>
                <p>Heladería Artesanal</p>
              </div>

              <div class="receipt-info">
                <div class="receipt-info-box">
                  <p><strong>Recibo #{{ selectedVenta.id }}</strong></p>
                  <p><i class="far fa-calendar"></i> {{ formatDate(selectedVenta.fechaVenta) }}</p>
                </div>
                <div class="receipt-info-box">
                  <p><strong>Cliente:</strong> {{ selectedVenta.nombreUsuario || 'N/A' }}</p>
                  <p><strong>Método de pago:</strong> {{ selectedVenta.metodoPago }}</p>
                </div>
              </div>

              <table class="receipt-table">
                <thead>
                  <tr><th>Producto</th><th>Cant.</th><th>P. Unit.</th><th>Subtotal</th></tr>
                </thead>
                <tbody>
                  <tr v-for="d in selectedVenta.detalles" :key="d.id">
                    <td>{{ d.nombreProducto }}</td>
                    <td>{{ d.cantidad }}</td>
                    <td>${{ d.precioUnitario.toFixed(2) }}</td>
                    <td class="subtotal">${{ d.subtotal.toFixed(2) }}</td>
                  </tr>
                </tbody>
              </table>

              <div class="receipt-total">
                <span>Total</span>
                <span class="total-amount">${{ selectedVenta.total.toFixed(2) }}</span>
              </div>

              <div v-if="selectedVenta.observaciones" class="receipt-notes">
                <i class="fas fa-sticky-note"></i> {{ selectedVenta.observaciones }}
              </div>

              <div class="receipt-footer-text">
                <p>Heladería La Dolce Vita &copy; 2026</p>
                <p>Calle Beni #123, Santa Cruz, Bolivia</p>
              </div>
            </div>
            <div class="receipt-actions">
              <button class="btn-download" @click="printReceipt">
                <i class="fas fa-print"></i> Imprimir / Descargar PDF
              </button>
              <button class="btn-close-modal" @click="closeReceipt">Cerrar</button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<style scoped>
.container { max-width: 900px; margin: 30px auto; padding: 0 20px; }
.page-header h1 { font-size: 26px; color: #333; margin-bottom: 25px; }

.empty-state { text-align: center; padding: 80px 20px; background: white; border-radius: 16px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06); }
.empty-state i { font-size: 56px; color: #ddd; margin-bottom: 16px; display: block; }
.empty-state p { color: #888; font-size: 16px; margin-bottom: 20px; }

.btn-back {
  background: linear-gradient(135deg, #e91e63, #f06292); color: white;
  padding: 12px 24px; border-radius: 12px; text-decoration: none;
  font-size: 14px; font-weight: 600; display: inline-flex; align-items: center; gap: 8px;
}

.purchases-list { display: flex; flex-direction: column; gap: 16px; }

.purchase-card {
  background: white; border-radius: 14px; padding: 22px; box-shadow: 0 2px 12px rgba(0, 0, 0, 0.05);
  transition: transform 0.2s ease;
}
.purchase-card:hover { transform: translateY(-2px); }

.purchase-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 14px; padding-bottom: 12px; border-bottom: 1px solid #f0f0f0; }
.purchase-id { font-weight: 700; color: #333; font-size: 16px; margin-right: 12px; }
.purchase-date { font-size: 13px; color: #888; }
.purchase-meta { display: flex; align-items: center; gap: 14px; }
.purchase-total { font-size: 20px; font-weight: 700; color: #2e7d32; }

.badge { padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: 600; }
.badge-efectivo { background: #e8f5e9; color: #2e7d32; }
.badge-tarjeta { background: #e3f2fd; color: #1565c0; }
.badge-transferencia { background: #fff3e0; color: #e65100; }

.purchase-details { margin-bottom: 8px; }
.detail-row { display: flex; justify-content: space-between; padding: 6px 0; font-size: 14px; color: #555; }

.purchase-footer { display: flex; justify-content: space-between; align-items: center; margin-top: 10px; }
.purchase-notes { padding: 8px 12px; background: #fafafa; border-radius: 8px; font-size: 13px; color: #888; flex: 1; margin-right: 12px; }
.purchase-notes i { margin-right: 6px; }

.btn-receipt {
  background: #fce4ec; color: #e91e63; border: none; padding: 8px 16px; border-radius: 8px;
  font-size: 12px; font-weight: 600; font-family: 'Poppins', sans-serif; cursor: pointer;
  display: inline-flex; align-items: center; gap: 6px; transition: all 0.2s; white-space: nowrap;
}
.btn-receipt:hover { background: #f8bbd0; }

.receipt-overlay {
  position: fixed; top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.5); z-index: 200;
  display: flex; align-items: center; justify-content: center; padding: 20px;
}

.receipt-modal {
  background: white; border-radius: 20px; padding: 35px;
  width: 100%; max-width: 700px; max-height: 90vh; overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0,0,0,0.2);
}

.receipt-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.receipt-header h2 { font-size: 20px; color: #333; display: flex; align-items: center; gap: 10px; }
.btn-close { background: #f5f5f5; border: none; width: 36px; height: 36px; border-radius: 10px; cursor: pointer; font-size: 14px; color: #666; display: flex; align-items: center; justify-content: center; }
.btn-close:hover { background: #fce4ec; color: #c62828; }

.receipt-content { padding: 20px 0; }
.receipt-brand { text-align: center; margin-bottom: 24px; }
.receipt-logo { height: 50px; margin-bottom: 8px; }
.receipt-brand h3 { color: #e91e63; font-size: 20px; margin: 0; }
.receipt-brand p { color: #888; font-size: 13px; }

.receipt-info { display: flex; gap: 16px; margin-bottom: 20px; }
.receipt-info-box { flex: 1; background: #f9f9f9; padding: 14px; border-radius: 10px; }
.receipt-info-box p { margin: 3px 0; font-size: 13px; color: #555; }
.receipt-info-box i { margin-right: 4px; color: #e91e63; }

.receipt-table { width: 100%; border-collapse: collapse; margin: 20px 0; }
.receipt-table th { background: #fce4ec; color: #e91e63; padding: 10px 14px; text-align: left; font-size: 13px; }
.receipt-table td { padding: 10px 14px; border-bottom: 1px solid #f0f0f0; font-size: 14px; color: #444; }
.receipt-table .subtotal { font-weight: 600; color: #2e7d32; }

.receipt-total { display: flex; justify-content: space-between; align-items: center; padding: 16px 0; border-top: 2px solid #e91e63; font-size: 16px; font-weight: 600; color: #333; }
.total-amount { font-size: 24px; font-weight: 700; color: #2e7d32; }

.receipt-notes { margin-top: 16px; padding: 10px 14px; background: #fafafa; border-radius: 8px; font-size: 13px; color: #888; }
.receipt-notes i { margin-right: 6px; }

.receipt-footer-text { text-align: center; margin-top: 24px; color: #ccc; font-size: 12px; }
.receipt-footer-text p { margin: 2px 0; }

.receipt-actions { display: flex; gap: 10px; margin-top: 20px; border-top: 1px solid #f0f0f0; padding-top: 20px; }
.btn-download {
  flex: 1; padding: 12px; background: linear-gradient(135deg, #e91e63, #f06292); color: white;
  border: none; border-radius: 10px; font-size: 14px; font-weight: 600; font-family: 'Poppins', sans-serif;
  cursor: pointer; display: flex; align-items: center; justify-content: center; gap: 8px;
}
.btn-download:hover { box-shadow: 0 4px 15px rgba(233,30,99,0.3); }
.btn-close-modal {
  padding: 12px 24px; background: #f5f5f5; color: #666; border: none; border-radius: 10px;
  font-size: 14px; font-family: 'Poppins', sans-serif; cursor: pointer; font-weight: 500;
}

.modal-enter-active, .modal-leave-active { transition: opacity 0.3s ease; }
.modal-enter-from, .modal-leave-to { opacity: 0; }
.modal-enter-active .receipt-modal, .modal-leave-active .receipt-modal { transition: transform 0.3s ease; }
.modal-enter-from .receipt-modal, .modal-leave-to .receipt-modal { transform: scale(0.95) translateY(-10px); }

@media (max-width: 768px) {
  .purchase-header { flex-direction: column; align-items: flex-start; gap: 8px; }
  .purchase-footer { flex-direction: column; gap: 10px; align-items: flex-start; }
  .purchase-notes { margin-right: 0; width: 100%; }
  .receipt-info { flex-direction: column; }
}
</style>
