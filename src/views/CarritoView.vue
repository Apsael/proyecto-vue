<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'

const router = useRouter()
const store = useStore()
const toast = useToast()

const cart = computed(() => store.getCart())
const total = computed(() => store.getCartTotal())

function updateQty(productoId: number, qty: number) {
  store.updateCartItem(productoId, qty)
}

function removeItem(productoId: number) {
  store.removeFromCart(productoId)
  toast.success('Producto eliminado del carrito')
}

function goToCheckout() {
  if (!store.getSession()) {
    toast.info('Inicia sesion para continuar con la compra')
    router.push('/login')
    return
  }
  router.push('/checkout')
}
</script>

<template>
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-shopping-cart"></i> Mi Carrito</h1>
    </div>

    <div v-if="cart.length === 0" class="empty-state">
      <i class="fas fa-shopping-basket"></i>
      <p>Tu carrito esta vacio</p>
      <router-link to="/" class="btn-back"><i class="fas fa-arrow-left"></i> Explorar productos</router-link>
    </div>

    <div v-else class="cart-layout">
      <div class="cart-items">
        <div v-for="item in cart" :key="item.producto.id" class="cart-item">
          <div class="item-icon">
            <img v-if="item.producto.imagenUrl" :src="item.producto.imagenUrl" :alt="item.producto.nombre" class="cart-thumb" />
            <i v-else class="fas fa-ice-cream"></i>
          </div>
          <div class="item-info">
            <h3>{{ item.producto.nombre }}</h3>
            <p class="item-cat">{{ item.producto.nombreCategoria }}</p>
            <p class="item-price">${{ item.producto.precio.toFixed(2) }} c/u</p>
          </div>
          <div class="item-qty">
            <button class="qty-btn" @click="updateQty(item.producto.id, item.cantidad - 1)">
              <i class="fas fa-minus"></i>
            </button>
            <span class="qty-value">{{ item.cantidad }}</span>
            <button class="qty-btn" @click="updateQty(item.producto.id, item.cantidad + 1)" :disabled="item.cantidad >= item.producto.stock">
              <i class="fas fa-plus"></i>
            </button>
          </div>
          <div class="item-subtotal">
            ${{ (item.producto.precio * item.cantidad).toFixed(2) }}
          </div>
          <button class="btn-remove" @click="removeItem(item.producto.id)">
            <i class="fas fa-trash"></i>
          </button>
        </div>
      </div>

      <div class="cart-summary">
        <h2>Resumen</h2>
        <div class="summary-row">
          <span>Productos</span>
          <span>{{ cart.length }}</span>
        </div>
        <div class="summary-row total">
          <span>Total</span>
          <span>${{ total.toFixed(2) }}</span>
        </div>
        <button class="btn-checkout" @click="goToCheckout">
          <i class="fas fa-credit-card"></i> Proceder al Pago
        </button>
        <router-link to="/" class="btn-continue">
          <i class="fas fa-arrow-left"></i> Seguir comprando
        </router-link>
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
  font-size: 16px;
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

.cart-layout {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 24px;
  align-items: start;
}

.cart-items {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.cart-item {
  background: white;
  border-radius: 14px;
  padding: 18px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.05);
}

.item-icon {
  width: 60px;
  height: 60px;
  border-radius: 12px;
  background: linear-gradient(135deg, #fce4ec, #f8bbd0);
  display: flex;
  align-items: center;
  justify-content: center;
  color: #e91e63;
  font-size: 20px;
  flex-shrink: 0;
  overflow: hidden;
}

.cart-thumb { width: 100%; height: 100%; object-fit: cover; }

.item-info { flex: 1; }

.item-info h3 {
  font-size: 15px;
  color: #333;
  margin-bottom: 2px;
}

.item-cat {
  font-size: 12px;
  color: #aaa;
}

.item-price {
  font-size: 13px;
  color: #888;
  margin-top: 2px;
}

.item-qty {
  display: flex;
  align-items: center;
  gap: 10px;
}

.qty-btn {
  width: 30px;
  height: 30px;
  border-radius: 8px;
  border: 2px solid #eee;
  background: white;
  cursor: pointer;
  font-size: 11px;
  color: #555;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.qty-btn:hover:not(:disabled) {
  border-color: #e91e63;
  color: #e91e63;
}

.qty-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.qty-value {
  font-size: 15px;
  font-weight: 600;
  min-width: 20px;
  text-align: center;
}

.item-subtotal {
  font-size: 16px;
  font-weight: 700;
  color: #2e7d32;
  min-width: 80px;
  text-align: right;
}

.btn-remove {
  background: #fce4ec;
  color: #c62828;
  border: none;
  width: 34px;
  height: 34px;
  border-radius: 8px;
  cursor: pointer;
  font-size: 13px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.btn-remove:hover {
  background: #f8bbd0;
}

.cart-summary {
  background: white;
  border-radius: 16px;
  padding: 28px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
  position: sticky;
  top: 90px;
}

.cart-summary h2 {
  font-size: 18px;
  color: #333;
  margin-bottom: 20px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
  font-size: 14px;
  color: #666;
  border-bottom: 1px solid #f0f0f0;
}

.summary-row.total {
  font-size: 20px;
  font-weight: 700;
  color: #333;
  border-bottom: none;
  padding-top: 14px;
}

.btn-checkout {
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
  margin-top: 20px;
  transition: all 0.3s;
}

.btn-checkout:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(46, 125, 50, 0.3);
}

.btn-continue {
  display: block;
  text-align: center;
  margin-top: 12px;
  color: #888;
  font-size: 13px;
  text-decoration: none;
}

.btn-continue:hover {
  color: #e91e63;
}

@media (max-width: 768px) {
  .cart-layout { grid-template-columns: 1fr; }
  .cart-item { flex-wrap: wrap; }
}
</style>
