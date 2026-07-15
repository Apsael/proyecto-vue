<script setup lang="ts">
import { computed } from 'vue'
import TopBar from '@/components/TopBar.vue'
import { useStore } from '@/composables/useStore'

const store = useStore()

const ventas = computed(() => store.getVentas())
const productos = computed(() => store.getAllProductos().filter(p => p.activo))

const totalVentas = computed(() => ventas.value.length)
const totalIngresos = computed(() => ventas.value.reduce((sum, v) => sum + v.total, 0))

const hoy: string = new Date().toISOString().split('T')[0] ?? ''
const ventasHoy = computed(() => ventas.value.filter(v => v.fecha_venta.startsWith(hoy)))
const ingresosHoy = computed(() => ventasHoy.value.reduce((sum, v) => sum + v.total, 0))

const topProductos = computed(() => {
  const map = new Map<number, { nombre: string; totalVendido: number; ingresos: number }>()
  const detalles = store.state.detalleVenta

  for (const d of detalles) {
    const existing = map.get(d.id_producto)
    if (existing) {
      existing.totalVendido += d.cantidad
      existing.ingresos += d.subtotal
    } else {
      const prod = store.getProductoById(d.id_producto)
      map.set(d.id_producto, {
        nombre: prod?.nombre ?? 'Eliminado',
        totalVendido: d.cantidad,
        ingresos: d.subtotal,
      })
    }
  }

  return [...map.values()].sort((a, b) => b.totalVendido - a.totalVendido).slice(0, 10)
})

const ventasPorMetodo = computed(() => {
  const map = new Map<string, { cantidad: number; monto: number }>()
  for (const v of ventas.value) {
    const existing = map.get(v.metodo_pago)
    if (existing) {
      existing.cantidad++
      existing.monto += v.total
    } else {
      map.set(v.metodo_pago, { cantidad: 1, monto: v.total })
    }
  }
  return [...map.entries()].sort((a, b) => b[1].monto - a[1].monto)
})

const maxMetodo = computed(() => {
  if (ventasPorMetodo.value.length === 0) return 1
  return Math.max(...ventasPorMetodo.value.map(([, v]) => v.monto), 1)
})

const stockBajo = computed(() =>
  productos.value.filter(p => p.stock < 20).sort((a, b) => a.stock - b.stock).slice(0, 8)
)

const ventasRecientes = computed(() => ventas.value.slice(0, 8))

function getVendedor(id: number): string {
  return store.getUsuarioById(id)?.nombre ?? 'Desconocido'
}

function getBadgeClass(metodo: string): string {
  const map: Record<string, string> = {
    efectivo: 'badge-efectivo',
    tarjeta: 'badge-tarjeta',
    transferencia: 'badge-transferencia',
  }
  return map[metodo] ?? ''
}

const barColors = ['c1', 'c2', 'c3']
</script>

<template>
  <TopBar />
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-chart-line"></i> Centro de Reportes</h1>
    </div>

    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon icon-1"><i class="fas fa-receipt"></i></div>
        <div class="stat-value">{{ totalVentas }}</div>
        <div class="stat-label">Ventas Totales</div>
      </div>
      <div class="stat-card">
        <div class="stat-icon icon-2"><i class="fas fa-dollar-sign"></i></div>
        <div class="stat-value">${{ totalIngresos.toFixed(2) }}</div>
        <div class="stat-label">Ingresos Totales</div>
      </div>
      <div class="stat-card">
        <div class="stat-icon icon-3"><i class="fas fa-shopping-bag"></i></div>
        <div class="stat-value">${{ ingresosHoy.toFixed(2) }}</div>
        <div class="stat-label">Ventas Hoy ({{ ventasHoy.length }})</div>
      </div>
      <div class="stat-card">
        <div class="stat-icon icon-4"><i class="fas fa-box"></i></div>
        <div class="stat-value">{{ productos.length }}</div>
        <div class="stat-label">Productos Activos</div>
      </div>
    </div>

    <div class="reports-grid">
      <div class="card">
        <h2><i class="fas fa-trophy" style="color:#ff9800"></i> Top Productos Vendidos</h2>
        <table v-if="topProductos.length > 0">
          <thead>
            <tr><th>Producto</th><th>Unidades</th><th>Ingresos</th></tr>
          </thead>
          <tbody>
            <tr v-for="(tp, idx) in topProductos" :key="idx">
              <td><strong>{{ tp.nombre }}</strong></td>
              <td>{{ tp.totalVendido }}</td>
              <td class="price">${{ tp.ingresos.toFixed(2) }}</td>
            </tr>
          </tbody>
        </table>
        <div v-else class="empty-state">
          <i class="fas fa-chart-bar"></i>
          <p>Aun no hay datos de ventas</p>
        </div>
      </div>

      <div class="card">
        <h2><i class="fas fa-credit-card" style="color:#2196f3"></i> Ventas por Metodo de Pago</h2>
        <div v-if="ventasPorMetodo.length > 0">
          <div v-for="([metodo, data], idx) in ventasPorMetodo" :key="metodo" class="bar-container">
            <div class="bar-label">{{ metodo.charAt(0).toUpperCase() + metodo.slice(1) }}</div>
            <div class="bar-track">
              <div
                class="bar-fill"
                :class="barColors[idx % 3]"
                :style="{ width: (data.monto / maxMetodo) * 100 + '%' }"
              >
                ${{ data.monto.toFixed(2) }}
              </div>
            </div>
          </div>
        </div>
        <div v-else class="empty-state">
          <i class="fas fa-credit-card"></i>
          <p>Sin datos aun</p>
        </div>
      </div>

      <div class="card full-width">
        <h2><i class="fas fa-history" style="color:#4caf50"></i> Historial Reciente de Ventas</h2>
        <table v-if="ventasRecientes.length > 0">
          <thead>
            <tr>
              <th>ID</th>
              <th>Vendedor</th>
              <th>Total</th>
              <th>Metodo</th>
              <th>Observaciones</th>
              <th>Fecha</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="v in ventasRecientes" :key="v.id">
              <td><strong>#{{ v.id }}</strong></td>
              <td>{{ getVendedor(v.id_usuario) }}</td>
              <td class="price">${{ v.total.toFixed(2) }}</td>
              <td>
                <span class="badge" :class="getBadgeClass(v.metodo_pago)">
                  {{ v.metodo_pago.charAt(0).toUpperCase() + v.metodo_pago.slice(1) }}
                </span>
              </td>
              <td>{{ v.observaciones || '-' }}</td>
              <td>{{ new Date(v.fecha_venta).toLocaleDateString('es-ES') }}</td>
            </tr>
          </tbody>
        </table>
        <div v-else class="empty-state">
          <i class="fas fa-receipt"></i>
          <p>No hay ventas registradas</p>
        </div>
      </div>

      <div class="card full-width">
        <h2><i class="fas fa-exclamation-triangle" style="color:#e65100"></i> Stock Bajo</h2>
        <div v-if="stockBajo.length > 0">
          <div v-for="p in stockBajo" :key="p.id" class="stock-alert">
            <span class="name">{{ p.nombre }}</span>
            <span class="qty">{{ p.stock }} uds</span>
          </div>
        </div>
        <div v-else class="empty-state">
          <i class="fas fa-check-circle"></i>
          <p>Todos los productos tienen stock suficiente</p>
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
  margin-bottom: 30px;
}

.page-header h1 {
  font-size: 26px;
  color: #333;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
  margin-bottom: 30px;
}

.stat-card {
  background: white;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
  position: relative;
  overflow: hidden;
}

.stat-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
}

.stat-card:nth-child(1)::before { background: linear-gradient(90deg, #e91e63, #f06292); }
.stat-card:nth-child(2)::before { background: linear-gradient(90deg, #ff9800, #ffb74d); }
.stat-card:nth-child(3)::before { background: linear-gradient(90deg, #4caf50, #81c784); }
.stat-card:nth-child(4)::before { background: linear-gradient(90deg, #2196f3, #64b5f6); }

.stat-icon {
  width: 50px;
  height: 50px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 22px;
  color: white;
  margin-bottom: 14px;
}

.icon-1 { background: linear-gradient(135deg, #e91e63, #f06292); }
.icon-2 { background: linear-gradient(135deg, #ff9800, #ffb74d); }
.icon-3 { background: linear-gradient(135deg, #4caf50, #81c784); }
.icon-4 { background: linear-gradient(135deg, #2196f3, #64b5f6); }

.stat-value {
  font-size: 28px;
  font-weight: 700;
  color: #333;
}

.stat-label {
  font-size: 13px;
  color: #888;
  margin-top: 4px;
}

.reports-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 25px;
  margin-bottom: 25px;
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
  margin-bottom: 18px;
  display: flex;
  align-items: center;
  gap: 10px;
}

table {
  width: 100%;
  border-collapse: collapse;
}

thead th {
  background: #fafafa;
  padding: 12px 14px;
  text-align: left;
  font-size: 12px;
  font-weight: 600;
  color: #555;
  border-bottom: 2px solid #f0f0f0;
}

tbody td {
  padding: 11px 14px;
  font-size: 13px;
  color: #444;
  border-bottom: 1px solid #f5f5f5;
}

tbody tr:hover { background: #fdf2f8; }

.price { font-weight: 600; color: #2e7d32; }

.badge {
  display: inline-block;
  padding: 3px 10px;
  border-radius: 20px;
  font-size: 11px;
  font-weight: 600;
}

.badge-efectivo      { background: #e8f5e9; color: #2e7d32; }
.badge-tarjeta       { background: #e3f2fd; color: #1565c0; }
.badge-transferencia { background: #fff3e0; color: #e65100; }

.bar-container {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 12px;
}

.bar-label {
  font-size: 13px;
  color: #555;
  width: 100px;
  flex-shrink: 0;
}

.bar-track {
  flex: 1;
  height: 24px;
  background: #f0f0f0;
  border-radius: 12px;
  overflow: hidden;
}

.bar-fill {
  height: 100%;
  border-radius: 12px;
  transition: width 0.5s ease;
  display: flex;
  align-items: center;
  padding-left: 10px;
  font-size: 11px;
  font-weight: 600;
  color: white;
}

.bar-fill.c1 { background: linear-gradient(90deg, #e91e63, #f06292); }
.bar-fill.c2 { background: linear-gradient(90deg, #2196f3, #64b5f6); }
.bar-fill.c3 { background: linear-gradient(90deg, #ff9800, #ffb74d); }

.stock-alert {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px 0;
  border-bottom: 1px solid #f5f5f5;
}

.stock-alert:last-child { border-bottom: none; }

.stock-alert .name {
  font-size: 13px;
  color: #444;
}

.stock-alert .qty {
  font-size: 13px;
  font-weight: 600;
  color: #e65100;
  background: #fff3e0;
  padding: 3px 10px;
  border-radius: 10px;
}

.empty-state {
  text-align: center;
  padding: 35px 20px;
  color: #aaa;
}

.empty-state i { font-size: 36px; margin-bottom: 10px; display: block; }

.full-width { grid-column: 1 / -1; }

@media (max-width: 768px) {
  .stats-grid { grid-template-columns: repeat(2, 1fr); }
  .reports-grid { grid-template-columns: 1fr; }
}
</style>
