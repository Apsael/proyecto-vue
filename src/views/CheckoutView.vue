<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'
import { api } from '@/services/api'

const router = useRouter()
const store = useStore()
const toast = useToast()

const metodoPago = ref('efectivo')
const observaciones = ref('')
const direccionEnvio = ref('')
const processing = ref(false)
const showLoader = ref(false)
const latitudEntrega = ref<number | null>(null)
const longitudEntrega = ref<number | null>(null)
const cardNumber = ref('')
const cardExpiry = ref('')
const cardCvc = ref('')
const cardName = ref('')
const paymentConfirmed = ref(false)
const lastVentaId = ref<number | null>(null)

const tipoEntrega = ref('local')
const showMap = ref(false)
const mapInitialized = ref(false)
let mapInstance: any = null
let markerInstance: any = null

const cart = computed(() => store.getCart())
const total = computed(() => store.getCartTotal())

if (cart.value.length === 0) {
  router.push('/carrito')
}

onMounted(() => {
  const user = store.getSession()
  if (user?.latitud && user?.longitud) {
    latitudEntrega.value = user.latitud
    longitudEntrega.value = user.longitud
  }
})

function formatCardNumber(value: string) {
  const v = value.replace(/\D/g, '').substring(0, 16)
  const parts = v.match(/.{1,4}/g)
  cardNumber.value = parts ? parts.join(' ') : v
}

function formatExpiry(value: string) {
  const v = value.replace(/\D/g, '').substring(0, 4)
  if (v.length > 2) {
    cardExpiry.value = v.substring(0, 2) + '/' + v.substring(2)
  } else {
    cardExpiry.value = v
  }
}

function toggleTipoEntrega() {
  if (tipoEntrega.value === 'local') {
    tipoEntrega.value = 'envio'
    setTimeout(initMap, 300)
  } else {
    tipoEntrega.value = 'local'
    removeMap()
  }
}

function initMap() {
  if (mapInitialized.value) return
  showMap.value = true
  setTimeout(() => {
    const L = (window as any).L
    if (!L) return
    mapInitialized.value = true
    const lat = latitudEntrega.value || -17.7853
    const lng = longitudEntrega.value || -63.1806
    mapInstance = L.map('checkout-map').setView([lat, lng], 14)
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(mapInstance)
    markerInstance = L.marker([lat, lng], { draggable: true }).addTo(mapInstance)
    markerInstance.on('dragend', () => {
      const pos = markerInstance.getLatLng()
      latitudEntrega.value = pos.lat
      longitudEntrega.value = pos.lng
    })
    mapInstance.on('click', (e: any) => {
      markerInstance.setLatLng(e.latlng)
      latitudEntrega.value = e.latlng.lat
      longitudEntrega.value = e.latlng.lng
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

async function handleCheckout() {
  if (cart.value.length === 0) return

  if (metodoPago.value === 'stripe') {
    if (!cardNumber.value || !cardExpiry.value || !cardCvc.value || !cardName.value) {
      toast.error('Complete todos los datos de la tarjeta')
      return
    }
    if (cardNumber.value.replace(/\s/g, '').length < 16) {
      toast.error('Número de tarjeta inválido')
      return
    }
  }

  showLoader.value = true
  processing.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 2000))

    const items = cart.value.map(item => ({
      productoId: item.producto.id,
      cantidad: item.cantidad,
    }))

    const ventaData: any = {
      items,
      metodoPago: metodoPago.value === 'stripe' ? 'tarjeta' : metodoPago.value,
      observaciones: observaciones.value,
    }

    if (tipoEntrega.value === 'envio') {
      ventaData.direccionEnvio = direccionEnvio.value || 'Sin dirección'
      ventaData.latitudEntrega = latitudEntrega.value
      ventaData.longitudEntrega = longitudEntrega.value
    }

    const ventaRes = await api.ventas.create(ventaData)

    lastVentaId.value = ventaRes.id
    paymentConfirmed.value = true
    store.clearCart()

    const user = store.getSession()
    if (user?.email) {
      try {
        const itemsHtml = ventaRes.detalles.map((d: any) => `
          <tr>
            <td style="padding:10px;border-bottom:1px solid #eee;">${d.nombreProducto} x${d.cantidad}</td>
            <td style="padding:10px;border-bottom:1px solid #eee;text-align:right;">$${d.subtotal.toFixed(2)}</td>
          </tr>
        `).join('')

        await api.mail.send(
          user.email,
          `Recibo de Compra #${ventaRes.id} - La Dolce Vita`,
          `
          <!DOCTYPE html>
          <html><head><meta charset="UTF-8"></head>
          <body style="margin:0;padding:0;font-family:Arial,sans-serif;background:#f5f6fa;">
            <table width="100%" cellpadding="0" cellspacing="0" style="padding:20px;">
              <tr><td align="center">
                <table width="600" cellpadding="0" cellspacing="0" style="background:#fff;border-radius:16px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);">
                  <tr><td style="background:linear-gradient(135deg,#e91e63,#f06292);padding:30px;text-align:center;">
                    <h1 style="color:#fff;margin:0;font-size:24px;">La Dolce Vita</h1>
                    <p style="color:rgba(255,255,255,0.85);margin:5px 0 0;font-size:14px;">¡Compra Confirmada!</p>
                  </td></tr>
                  <tr><td style="padding:30px;">
                    <p style="color:#666;">Hola <strong>${user.nombre}</strong>, gracias por tu compra.</p>
                    <div style="background:#f8f9fa;border-radius:12px;padding:20px;margin:20px 0;">
                      <p style="margin:5px 0;"><strong>Recibo #${ventaRes.id}</strong></p>
                      <p style="margin:5px 0;">Fecha: ${new Date(ventaRes.fechaVenta).toLocaleDateString('es-ES', { year:'numeric',month:'long',day:'numeric',hour:'2-digit',minute:'2-digit' })}</p>
                      <p style="margin:5px 0;">Método de pago: ${ventaRes.metodoPago}</p>
                      <p style="margin:5px 0;">Tipo: ${tipoEntrega.value === 'envio' ? 'Envío a domicilio' : 'Recoger en tienda'}</p>
                    </div>
                    <table width="100%" style="border-collapse:collapse;">
                      <thead><tr><th style="padding:10px;text-align:left;border-bottom:2px solid #e91e63;color:#e91e63;">Producto</th><th style="padding:10px;text-align:right;border-bottom:2px solid #e91e63;color:#e91e63;">Total</th></tr></thead>
                      <tbody>${itemsHtml}</tbody>
                      <tfoot>
                        <tr><td style="padding:12px;text-align:right;font-weight:700;font-size:18px;">TOTAL</td><td style="padding:12px;text-align:right;font-weight:700;font-size:20px;color:#2e7d32;">$${ventaRes.total.toFixed(2)}</td></tr>
                      </tfoot>
                    </table>
                    <div style="text-align:center;margin:30px 0;">
                      <a href="${window.location.origin}/mis-compras" style="background:linear-gradient(135deg,#e91e63,#f06292);color:#fff;padding:12px 28px;text-decoration:none;border-radius:12px;display:inline-block;font-weight:600;">Ver Mis Compras</a>
                    </div>
                  </td></tr>
                  <tr><td style="background:#f9fafb;padding:20px;text-align:center;border-top:1px solid #eee;">
                    <p style="margin:0;color:#999;font-size:12px;">&copy; 2026 Heladería La Dolce Vita</p>
                  </td></tr>
                </table>
              </td></tr>
            </table>
          </body></html>
          `
        )
      } catch {}
    }

    showLoader.value = false
    processing.value = false
  } catch (e: any) {
    showLoader.value = false
    processing.value = false
    toast.error(e.message || 'Error al procesar la compra')
  }
}

function viewPurchases() {
  router.push('/mis-compras')
}
</script>

<template>
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-credit-card"></i> Checkout</h1>
    </div>

    <div v-if="paymentConfirmed" class="confirmation-overlay">
      <div class="confirmation-card">
        <div class="confirmation-icon"><i class="fas fa-check-circle"></i></div>
        <h2>¡Compra Exitosa!</h2>
        <p>Tu pedido #{{ lastVentaId }} ha sido procesado correctamente.</p>
        <p class="confirmation-email">Recibirás el recibo en tu correo electrónico.</p>
        <button class="btn-view-purchases" @click="viewPurchases">
          <i class="fas fa-shopping-bag"></i> Ver Mis Compras
        </button>
      </div>
    </div>

    <div v-if="showLoader && !paymentConfirmed" class="loader-overlay">
      <div class="cart-loader">
        <div class="items-container">
          <div id="item-mobile" class="item"></div>
          <div id="item-laptop" class="item"></div>
          <div id="item-tab" class="item"></div>
          <div id="item-headphone" class="item"></div>
          <div id="item-mixer" class="item"></div>
        </div>
        <div id="cart-icon"></div>
        <div class="loading-text">
          Procesando compra<span class="dot">.</span><span class="dot">.</span><span class="dot">.</span>
        </div>
      </div>
    </div>

    <div v-if="!paymentConfirmed && !showLoader" class="checkout-layout">
      <div class="checkout-form">
        <div class="card">
          <h2><i class="fas fa-truck" style="color:#e91e63"></i> Tipo de Entrega</h2>
          <div class="toggle-container">
            <button class="toggle-btn" :class="{ active: tipoEntrega === 'local' }" @click="tipoEntrega = 'local'; removeMap()">
              <i class="fas fa-store"></i> Local
            </button>
            <button class="toggle-btn" :class="{ active: tipoEntrega === 'envio' }" @click="toggleTipoEntrega">
              <i class="fas fa-truck"></i> Envío
            </button>
          </div>

          <div v-if="tipoEntrega === 'envio'" class="delivery-section">
            <div class="form-group">
              <label>Dirección de envío</label>
              <input type="text" v-model="direccionEnvio" placeholder="Calle, número, zona..." />
            </div>
            <div class="form-group">
              <label>Ubicación en el mapa</label>
              <div v-if="showMap" class="map-container">
                <div id="checkout-map" class="checkout-leaflet-map"></div>
              </div>
              <p class="location-coords" v-if="latitudEntrega && longitudEntrega">
                Lat: {{ latitudEntrega.toFixed(6) }}, Lng: {{ longitudEntrega.toFixed(6) }}
              </p>
            </div>
          </div>
        </div>

        <div class="card">
          <h2><i class="fas fa-receipt" style="color:#e91e63"></i> Método de Pago</h2>
          <div class="payment-options">
            <label class="payment-option" :class="{ active: metodoPago === 'stripe' }">
              <input type="radio" v-model="metodoPago" value="stripe" />
              <i class="fas fa-credit-card"></i>
              <span>Tarjeta Visa/MC</span>
            </label>
            <label class="payment-option" :class="{ active: metodoPago === 'efectivo' }">
              <input type="radio" v-model="metodoPago" value="efectivo" />
              <i class="fas fa-money-bill-wave"></i>
              <span>Efectivo</span>
            </label>
            <label class="payment-option" :class="{ active: metodoPago === 'transferencia' }">
              <input type="radio" v-model="metodoPago" value="transferencia" />
              <i class="fas fa-university"></i>
              <span>Transferencia</span>
            </label>
          </div>

          <div v-if="metodoPago === 'stripe'" class="card-form">
            <p class="card-form-hint"><i class="fas fa-lock"></i> Pago simulado - cualquier número de tarjeta es aceptado</p>
            <div class="form-group">
              <label>Nombre en la tarjeta</label>
              <input type="text" v-model="cardName" placeholder="Como aparece en la tarjeta" />
            </div>
            <div class="form-group">
              <label>Número de tarjeta</label>
              <input type="text" v-model="cardNumber" @input="formatCardNumber(($event.target as HTMLInputElement).value)" placeholder="4242 4242 4242 4242" maxlength="19" />
            </div>
            <div class="form-row-fields">
              <div class="form-group">
                <label>Vencimiento</label>
                <input type="text" v-model="cardExpiry" @input="formatExpiry(($event.target as HTMLInputElement).value)" placeholder="MM/AA" maxlength="5" />
              </div>
              <div class="form-group">
                <label>CVC</label>
                <input type="text" v-model="cardCvc" placeholder="123" maxlength="4" />
              </div>
            </div>
            <div class="card-preview">
              <div class="card-preview-inner">
                <div class="card-chip"><i class="fas fa-microchip"></i></div>
                <div class="card-number-display">{{ cardNumber || '**** **** **** ****' }}</div>
                <div class="card-bottom">
                  <span>{{ cardName || 'TITULAR' }}</span>
                  <span>{{ cardExpiry || 'MM/AA' }}</span>
                </div>
              </div>
            </div>
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
.container { max-width: 1000px; margin: 30px auto; padding: 0 20px; }
.page-header h1 { font-size: 26px; color: #333; margin-bottom: 25px; }

.checkout-layout { display: grid; grid-template-columns: 1fr 380px; gap: 24px; align-items: start; }

.card { background: white; border-radius: 16px; padding: 28px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06); margin-bottom: 20px; }
.card h2 { font-size: 17px; color: #333; margin-bottom: 20px; display: flex; align-items: center; gap: 10px; }

.toggle-container { display: flex; gap: 12px; margin-bottom: 20px; }
.toggle-btn {
  flex: 1; padding: 14px; border: 2px solid #eee; border-radius: 12px;
  background: white; cursor: pointer; font-size: 14px; font-weight: 600;
  font-family: 'Poppins', sans-serif; color: #666; transition: all 0.3s;
  display: flex; align-items: center; justify-content: center; gap: 8px;
}
.toggle-btn.active { border-color: #e91e63; background: #fce4ec; color: #e91e63; }
.toggle-btn:hover { border-color: #e91e63; }

.delivery-section { margin-bottom: 10px; }

.map-container { margin-bottom: 10px; }
.checkout-leaflet-map { height: 220px; border-radius: 12px; border: 2px solid #e91e63; z-index: 1; }
.location-coords { font-size: 11px; color: #888; margin-top: 4px; }

.payment-options { display: flex; gap: 12px; margin-bottom: 24px; }
.payment-option {
  flex: 1; display: flex; flex-direction: column; align-items: center; gap: 8px;
  padding: 18px 12px; border: 2px solid #eee; border-radius: 12px;
  cursor: pointer; transition: all 0.3s; font-size: 13px; color: #666;
}
.payment-option input { display: none; }
.payment-option i { font-size: 22px; }
.payment-option.active { border-color: #e91e63; background: #fce4ec; color: #e91e63; }

.card-form { margin-bottom: 20px; }
.card-form-hint { font-size: 12px; color: #888; margin-bottom: 16px; display: flex; align-items: center; gap: 6px; }
.form-row-fields { display: flex; gap: 12px; }
.form-row-fields .form-group { flex: 1; }

.form-group { margin-bottom: 14px; }
.form-group label { display: block; font-size: 13px; font-weight: 500; color: #555; margin-bottom: 5px; }
.form-group input, .form-group textarea {
  width: 100%; padding: 11px 14px; border: 2px solid #eee; border-radius: 10px;
  font-size: 14px; font-family: 'Poppins', sans-serif; background: #fafafa; transition: all 0.3s;
}
.form-group input:focus, .form-group textarea:focus { outline: none; border-color: #e91e63; background: #fff; }
.form-group textarea { resize: vertical; }

.card-preview {
  background: linear-gradient(135deg, #667eea, #764ba2); border-radius: 14px; padding: 24px;
  margin-top: 16px; color: white; min-height: 160px; display: flex; flex-direction: column; justify-content: space-between;
}
.card-preview-inner { display: flex; flex-direction: column; gap: 16px; }
.card-chip { font-size: 24px; }
.card-number-display { font-size: 20px; font-weight: 600; letter-spacing: 2px; font-family: 'Courier New', monospace; }
.card-bottom { display: flex; justify-content: space-between; font-size: 13px; text-transform: uppercase; letter-spacing: 1px; opacity: 0.9; }

.order-items { max-height: 300px; overflow-y: auto; margin-bottom: 16px; }
.order-item { display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #f5f5f5; font-size: 14px; color: #444; }
.order-price { font-weight: 600; color: #2e7d32; }

.order-total { display: flex; justify-content: space-between; align-items: center; padding: 16px 0; border-top: 2px solid #f0f0f0; font-size: 14px; color: #666; }
.total-value { font-size: 24px; font-weight: 700; color: #333; }

.btn-pay {
  width: 100%; padding: 14px; background: linear-gradient(135deg, #2e7d32, #66bb6a);
  color: white; border: none; border-radius: 12px; font-size: 15px; font-weight: 600;
  font-family: 'Poppins', sans-serif; cursor: pointer; display: flex; align-items: center;
  justify-content: center; gap: 8px; margin-top: 16px; transition: all 0.3s;
}
.btn-pay:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(46, 125, 50, 0.3); }
.btn-pay:disabled { opacity: 0.7; cursor: not-allowed; }

.btn-back { display: block; text-align: center; margin-top: 12px; color: #888; font-size: 13px; text-decoration: none; }
.btn-back:hover { color: #e91e63; }

.loader-overlay {
  position: fixed; top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(255, 255, 255, 0.95); display: flex; align-items: center;
  justify-content: center; z-index: 1000; backdrop-filter: blur(8px);
}

.cart-loader {
  --loader-scale: 1; position: relative; width: 160px; height: 200px;
  display: flex; flex-direction: column; align-items: center; justify-content: flex-end;
  transform: scale(var(--loader-scale)); transform-origin: center center;
}
@media (max-width: 768px) { .cart-loader { --loader-scale: 0.85; } }
@media (max-width: 480px) { .cart-loader { --loader-scale: 0.7; } }
@media (min-width: 1400px) { .cart-loader { --loader-scale: 1.2; } }

.items-container { position: absolute; top: 20px; left: 0; width: 100%; height: 100px; z-index: 1; }
.items-container { position: absolute; top: 20px; left: 0; width: 100%; height: 100px; z-index: 1; }
.item { position: absolute; opacity: 0; background-size: contain; background-repeat: no-repeat; background-position: center; animation: drop-item 2s cubic-bezier(0.3, 0, 0.5, 1) infinite; }

#item-mobile { top:-15px; left:58px; width:20px; height:32px; --end-rot:-15deg; animation-delay:0.05s; background-image: url("data:image/svg+xml,%3Csvg xmlns='://www.w3.org/2000/svg' viewBox='0 0 46 56'%3E%3Ccircle cx='15' cy='16' r='8' fill='%23F9A8D4'/%3E%3Ccircle cx='23' cy='11' r='8' fill='%23FDE68A'/%3E%3Ccircle cx='31' cy='16' r='8' fill='%2393C5FD'/%3E%3Cpath d='M12 22H34L30 40H16Z' fill='%23F8FAFC' stroke='%23CBD5E1' stroke-width='1.5'/%3E%3Crect x='21' y='40' width='4' height='8' fill='%23CBD5E1'/%3E%3Crect x='16' y='48' width='14' height='3' rx='1.5' fill='%23CBD5E1'/%3E%3C/svg%3E"); }
#item-laptop { top:-10px; left:70px; width:35px; height:26px; --end-rot:10deg; animation-delay:0.4s; background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 36 56'%3E%3Cpath d='M10 14H26L23 42H13Z' fill='%23FBCFE8' stroke='%23EC4899' stroke-width='1.5'/%3E%3Cpath d='M13 10C13 5 23 5 23 10' fill='%23FFF4BF'/%3E%3Cline x1='24' y1='2' x2='30' y2='16' stroke='%230EA5E9' stroke-width='2'/%3E%3Ccircle cx='18' cy='8' r='2' fill='%23DC2626'/%3E%3C/svg%3E"); }
#item-tab { top:-20px; left:85px; width:24px; height:32px; --end-rot:25deg; animation-delay:0.8s; background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 26 48'%3E%3Crect x='4' y='2' width='18' height='30' rx='8' fill='%23FB7185'/%3E%3Crect x='11' y='32' width='4' height='12' rx='2' fill='%23D6A15A'/%3E%3Cpath d='M8 8L18 20' stroke='%23FECDD3' stroke-width='1.5'/%3E%3C/svg%3E"); }
#item-headphone { top:-15px; left:58px; width:28px; height:28px; --end-rot:-5deg; animation-delay:1.2s; background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 40 56'%3E%3Ccircle cx='20' cy='14' r='11' fill='%23704B34'/%3E%3Cpolygon points='20,54 10,24 30,24' fill='%23D6A15A'/%3E%3Cpath d='M13 31L27 46M27 31L13 46' stroke='%23B97A3D' stroke-width='1.5'/%3E%3C/svg%3E"); }
#item-mixer { top:-25px; left:75px; width:26px; height:34px; --end-rot:5deg; animation-delay:1.6s; background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 40 56'%3E%3Ccircle cx='20' cy='14' r='11' fill='%23FFF4BF'/%3E%3Cpolygon points='20,54 10,24 30,24' fill='%23D6A15A'/%3E%3Cpath d='M13 31L27 46M27 31L13 46' stroke='%23B97A3D' stroke-width='1.5'/%3E%3C/svg%3E"); }

#cart-icon { position: relative; z-index: 2; width: 140px; height: 120px; background-size: contain; background-repeat: no-repeat; background-position: center; background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 140 120' width='140' height='120' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' stroke='%23334155' stroke-width='5' stroke-linecap='round' stroke-linejoin='round'%3E%3Cline x1='35' y1='90' x2='110' y2='90' /%3E%3Cline x1='40' y1='90' x2='50' y2='70' /%3E%3Cpolyline points='10,15 25,15 40,30' /%3E%3Cline x1='40' y1='30' x2='50' y2='70' /%3E%3Cline x1='68' y1='30' x2='71' y2='70' /%3E%3Cline x1='96' y1='30' x2='93' y2='70' /%3E%3Cline x1='125' y1='30' x2='115' y2='70' /%3E%3Cline x1='40' y1='30' x2='125' y2='30' /%3E%3Cline x1='43' y1='43' x2='122' y2='43' /%3E%3Cline x1='47' y1='57' x2='118' y2='57' /%3E%3Cline x1='50' y1='70' x2='115' y2='70' /%3E%3Ccircle cx='45' cy='105' r='8' /%3E%3Ccircle cx='105' cy='105' r='8' /%3E%3C/g%3E%3C/svg%3E"); animation: cart-bounce 0.4s ease-in-out infinite; }

.loading-text { margin-top: 10px; font-size: 16px; font-weight: 700; color: #1e293b; letter-spacing: 0.5px; white-space: nowrap; }

.dot { display: inline-block; animation: wave 0.8s infinite; }
.dot:nth-child(1) { animation-delay: 0s; }
.dot:nth-child(2) { animation-delay: 0.1s; }
.dot:nth-child(3) { animation-delay: 0.2s; }

@keyframes drop-item {
  0% { transform: translateY(-20px) scale(0.8) rotate(0deg); opacity: 0; }
  10% { opacity: 1; transform: translateY(20px) scale(1) rotate(calc(var(--end-rot) / 2)); }
  35% { transform: translateY(55px) scale(1) rotate(var(--end-rot)); opacity: 1; }
  50%, 100% { transform: translateY(75px) scale(0.9) rotate(var(--end-rot)); opacity: 0; }
}
@keyframes cart-bounce { 0%,100% { transform: translateY(0); } 40% { transform: translateY(3px); } 60% { transform: translateY(0); } }
@keyframes wave { 0%,60%,100% { transform: translateY(0); } 30% { transform: translateY(-4px); } }

.confirmation-overlay {
  position: fixed; top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.4); display: flex; align-items: center; justify-content: center; z-index: 1000; animation: fadeIn 0.3s ease;
}
.confirmation-card { background: white; border-radius: 24px; padding: 50px 40px; text-align: center; max-width: 440px; width: 90%; box-shadow: 0 25px 80px rgba(0,0,0,0.15); animation: scaleIn 0.4s cubic-bezier(0.16, 1, 0.3, 1); }
.confirmation-icon { font-size: 72px; color: #4caf50; margin-bottom: 16px; }
.confirmation-card h2 { font-size: 24px; color: #333; margin-bottom: 10px; }
.confirmation-card p { color: #666; margin-bottom: 6px; }
.confirmation-email { font-size: 13px; color: #888; }
.btn-view-purchases { margin-top: 24px; padding: 14px 32px; background: linear-gradient(135deg, #e91e63, #f06292); color: white; border: none; border-radius: 12px; font-size: 15px; font-weight: 600; font-family: 'Poppins', sans-serif; cursor: pointer; transition: all 0.3s; }
.btn-view-purchases:hover { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(233, 30, 99, 0.3); }

@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
@keyframes scaleIn { from { opacity: 0; transform: scale(0.9); } to { opacity: 1; transform: scale(1); } }

@media (max-width: 768px) {
  .checkout-layout { grid-template-columns: 1fr; }
  .payment-options { flex-direction: column; }
}
</style>
