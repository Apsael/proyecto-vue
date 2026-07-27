<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'

const router = useRouter()
const store = useStore()
const toast = useToast()

const metodoPago = ref('efectivo')
const observaciones = ref('')
const processing = ref(false)

const cart = computed(() => store.getCart())
const total = computed(() => store.getCartTotal())

if (cart.value.length === 0) {
  router.push('/carrito')
}

async function handleCheckout() {
  if (cart.value.length === 0) return

  processing.value = true
  try {
    const items = cart.value.map(item => ({
      productoId: item.producto.id,
      cantidad: item.cantidad,
    }))

    await api.ventas.create({
      items,
      metodoPago: metodoPago.value,
      observaciones: observaciones.value,
    })

    store.clearCart()
    toast.success('Compra realizada exitosamente!')
    router.push('/mis-compras')
  } catch (e: any) {
    toast.error(e.message || 'Error al procesar la compra')
  } finally {
    processing.value = false
  }
}

import { api } from '@/services/api'
</script>

<template>
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-credit-card"></i> Checkout</h1>
    </div>

    <div class="checkout-layout">
      <div class="checkout-form">
        <div class="card">
          <h2><i class="fas fa-receipt" style="color:#e91e63"></i> Metodo de Pago</h2>
          <div class="payment-options">
            <label class="payment-option" :class="{ active: metodoPago === 'efectivo' }">
              <input type="radio" v-model="metodoPago" value="efectivo" />
              <i class="fas fa-money-bill-wave"></i>
              <span>Efectivo</span>
            </label>
            <label class="payment-option" :class="{ active: metodoPago === 'tarjeta' }">
              <input type="radio" v-model="metodoPago" value="tarjeta" />
              <i class="fas fa-credit-card"></i>
              <span>Tarjeta</span>
            </label>
            <label class="payment-option" :class="{ active: metodoPago === 'transferencia' }">
              <input type="radio" v-model="metodoPago" value="transferencia" />
              <i class="fas fa-university"></i>
              <span>Transferencia</span>
            </label>
          </div>

          <div class="form-group">
            <label>Observaciones (opcional)</label>
            <textarea v-model="observaciones" placeholder="Instrucciones adicionales para tu pedido..." rows="3"></textarea>
          </div>
        </div>
      </div>

      <div class="checkout-summary">
        <div class="card">
          <h2><i class="fas fa-shopping-bag" style="color:#ff9800"></i> Tu Pedido</h2>
          <div class="order-items">
            <div v-for="item in cart" :key="item.producto.id" class="order-item">
              <span class="order-name">{{ item.producto.nombre }} x{{ item.cantidad }}</span>
              <span class="order-price">${{ (item.producto.precio * item.cantidad).toFixed(2) }}</span>
            </div>
          </div>
          <div class="order-total">
            <span>Total a pagar</span>
            <span class="total-value">${{ total.toFixed(2) }}</span>
          </div>
          <button class="btn-pay" @click="handleCheckout" :disabled="processing">
            <i class="fas" :class="processing ? 'fa-spinner fa-spin' : 'fa-check-circle'"></i>
            {{ processing ? 'Procesando...' : 'Confirmar Compra' }}
          </button>
          <router-link to="/carrito" class="btn-back">
            <i class="fas fa-arrow-left"></i> Volver al carrito
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container {
  max-width: 1000px;
  margin: 30px auto;
  padding: 0 20px;
}

.page-header h1 {
  font-size: 26px;
  color: #333;
  margin-bottom: 25px;
}

.checkout-layout {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: 24px;
  align-items: start;
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

.payment-options {
  display: flex;
  gap: 12px;
  margin-bottom: 24px;
}

.payment-option {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 18px 12px;
  border: 2px solid #eee;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 13px;
  color: #666;
}

.payment-option input { display: none; }

.payment-option i { font-size: 22px; }

.payment-option.active {
  border-color: #e91e63;
  background: #fce4ec;
  color: #e91e63;
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

.form-group textarea {
  width: 100%;
  padding: 11px 14px;
  border: 2px solid #eee;
  border-radius: 10px;
  font-size: 14px;
  font-family: 'Poppins', sans-serif;
  resize: vertical;
  background: #fafafa;
  transition: all 0.3s;
}

.form-group textarea:focus {
  outline: none;
  border-color: #e91e63;
  background: #fff;
}

.order-items {
  max-height: 300px;
  overflow-y: auto;
  margin-bottom: 16px;
}

.order-item {
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
  border-bottom: 1px solid #f5f5f5;
  font-size: 14px;
  color: #444;
}

.order-price {
  font-weight: 600;
  color: #2e7d32;
}

.order-total {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 0;
  border-top: 2px solid #f0f0f0;
  font-size: 14px;
  color: #666;
}

.total-value {
  font-size: 24px;
  font-weight: 700;
  color: #333;
}

.btn-pay {
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
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  margin-top: 16px;
  transition: all 0.3s;
}

.btn-pay:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(46, 125, 50, 0.3);
}

.btn-pay:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.btn-back {
  display: block;
  text-align: center;
  margin-top: 12px;
  color: #888;
  font-size: 13px;
  text-decoration: none;
}

.btn-back:hover { color: #e91e63; }

@media (max-width: 768px) {
  .checkout-layout { grid-template-columns: 1fr; }
  .payment-options { flex-direction: column; }
}
</style>
