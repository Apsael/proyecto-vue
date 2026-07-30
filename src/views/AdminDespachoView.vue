<script setup lang="ts">
import { ref, onMounted, computed, nextTick } from 'vue'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'
import { api } from '@/services/api'
import L from 'leaflet'

const store = useStore()
const toast = useToast()

const mapContainer = ref<HTMLDivElement>()
const map = ref<any>(null)
const companyLat = ref(-17.7853)
const companyLng = ref(-63.1806)
const selectedVenta = ref<any>(null)
const showRoutes = ref(true)
const loading = ref(true)
const routeDistance = ref<number | null>(null)
const routeDuration = ref<number | null>(null)
const calculatingRoute = ref(false)

const routingUrl = 'https://router.project-osrm.org/route/v1/driving/'
let routeLayer: any = null
let midpointMarker: any = null
let clientMarkers: Map<number, any> = new Map()
let prevSelectedId: number | null = null
let companyMarker: any = null

const companyIcon = L.divIcon({
  html: '<div style="background:#e91e63;color:white;width:36px;height:36px;border-radius:50%;display:flex;align-items:center;justify-content:center;font-size:16px;box-shadow:0 2px 8px rgba(0,0,0,0.3);"><i class="fas fa-store"></i></div>',
  className: '',
  iconSize: [36, 36],
  iconAnchor: [18, 18]
})

function getClientIcon(venta: any, isSelected: boolean) {
  const color = getStatusColor(venta.estado)
  const size = isSelected ? 40 : 30
  return L.divIcon({
    html: `<div style="background:${color};color:white;width:${size}px;height:${size}px;border-radius:50%;display:flex;align-items:center;justify-content:center;font-size:${isSelected ? 16 : 12}px;box-shadow:0 2px 8px rgba(0,0,0,0.3);border:${isSelected ? '3px solid #e91e63' : 'none'};"><i class="fas fa-user"></i></div>`,
    className: '',
    iconSize: [size, size],
    iconAnchor: [size / 2, size / 2]
  })
}

function getStatusColor(estado: string): string {
  const map: Record<string, string> = {
    pendiente: '#ff9800',
    confirmado: '#2196f3',
    despachado: '#4caf50',
    entregado: '#9e9e9e',
    cancelado: '#f44336'
  }
  return map[estado] ?? '#ff9800'
}

onMounted(async () => {
  try {
    const empresaConfig = await api.config.getEmpresa()
    companyLat.value = empresaConfig.latitud
    companyLng.value = empresaConfig.longitud
  } catch {
    const saved = localStorage.getItem('empresaConfig')
    if (saved) {
      const cfg = JSON.parse(saved)
      companyLat.value = cfg.latitud
      companyLng.value = cfg.longitud
    }
  }

  try {
    await store.loadAllVentas()
  } catch {
    toast.error('Error al cargar ventas')
  } finally {
    loading.value = false
    await nextTick()
  }

  initMap()
})

const ventas = computed(() => store.getAllVentas().filter((v: any) => v.latitudEntrega && v.longitudEntrega))

function initMap() {
  if (!mapContainer.value) return
  map.value = L.map(mapContainer.value).setView([companyLat.value, companyLng.value], 12)
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; OpenStreetMap contributors'
  }).addTo(map.value)

  companyMarker = L.marker([companyLat.value, companyLng.value], { icon: companyIcon })
    .addTo(map.value)
    .bindPopup('<strong>La Dolce Vita</strong><br>Heladería Artesanal')

  drawAllMarkers()
}

function drawAllMarkers() {
  if (!map.value) return
  clientMarkers.forEach(m => { if (m) map.value!.removeLayer(m) })
  clientMarkers.clear()

  ventas.value.forEach(venta => {
    const isSelected = selectedVenta.value?.id === venta.id
    const marker = L.marker([venta.latitudEntrega, venta.longitudEntrega], { icon: getClientIcon(venta, isSelected) })
      .addTo(map.value)
      .bindPopup(popupContent(venta))

    marker.on('click', () => selectVenta(venta))
    clientMarkers.set(venta.id, marker)
  })
}

function updateMarkerIcons() {
  const selId = selectedVenta.value?.id
  if (prevSelectedId !== null && clientMarkers.has(prevSelectedId)) {
    const prevVenta = ventas.value.find(v => v.id === prevSelectedId)
    if (prevVenta) {
      clientMarkers.get(prevSelectedId)!.setIcon(getClientIcon(prevVenta, false))
    }
  }
  if (selId !== null && clientMarkers.has(selId)) {
    clientMarkers.get(selId)!.setIcon(getClientIcon(selectedVenta.value, true))
  }
  prevSelectedId = selId
}

function popupContent(venta: any) {
  const isSelected = selectedVenta.value?.id === venta.id
  let extra = ''
  if (isSelected && routeDistance.value !== null && routeDuration.value !== null) {
    extra = `<br><strong style="color:#2e7d32;"><i class="fas fa-road"></i> ${routeDistance.value.toFixed(1)} km</strong><br><strong style="color:#1565c0;"><i class="fas fa-clock"></i> ${Math.round(routeDuration.value / 60)} min</strong>`
  }
  return `
    <strong>#${venta.id}</strong><br>
    ${venta.nombreUsuario}<br>
    $${venta.total.toFixed(2)}<br>
    <span style="display:inline-block;padding:2px 8px;border-radius:12px;font-size:11px;background:${getStatusColor(venta.estado)};color:white;text-transform:capitalize;">${venta.estado}</span>
    ${extra}
  `
}

async function selectVenta(venta: any) {
  if (routeLayer && map.value) {
    map.value.removeLayer(routeLayer)
    routeLayer = null
  }
  if (midpointMarker && map.value) {
    map.value.removeLayer(midpointMarker)
    midpointMarker = null
  }
  selectedVenta.value = venta
  routeDistance.value = null
  routeDuration.value = null
  calculatingRoute.value = true

  updateMarkerIcons()

  if (companyMarker) {
    companyMarker.setLatLng([companyLat.value, companyLng.value])
  }

  if (map.value) {
    const bounds = L.latLngBounds([
      [companyLat.value, companyLng.value],
      [venta.latitudEntrega, venta.longitudEntrega]
    ])
    map.value.fitBounds(bounds, { padding: [50, 50] })
  }

  const url = `${routingUrl}${companyLng.value},${companyLat.value};${venta.longitudEntrega},${venta.latitudEntrega}?geometries=geojson`
  try {
    const res = await fetch(url)
    const data = await res.json()
    if (data.routes && data.routes.length > 0) {
      const route = data.routes[0]
      routeDistance.value = route.distance / 1000
      routeDuration.value = route.duration

      const coords = route.geometry.coordinates.map((c: number[]) => [c[1], c[0]])
      routeLayer = L.polyline(coords, {
        color: '#e91e63',
        weight: 4,
        opacity: 0.8
      }).addTo(map.value!)

      const midpoint = coords[Math.floor(coords.length / 2)]
      const midLatLng = L.latLng(midpoint[0], midpoint[1])
      const distanceLabel = routeDistance.value.toFixed(1)
      const durationLabel = Math.round(routeDuration.value / 60)
      midpointMarker = L.marker(midLatLng, {
        icon: L.divIcon({
          html: `<div style="background:white;color:#333;padding:6px 14px;border-radius:20px;box-shadow:0 2px 12px rgba(0,0,0,0.15);font-size:12px;font-weight:600;border:2px solid #e91e63;white-space:nowrap;"><i class="fas fa-road" style="color:#2e7d32"></i> ${distanceLabel} km &nbsp; <i class="fas fa-clock" style="color:#1565c0"></i> ${durationLabel} min</div>`,
          className: '',
          iconSize: [0, 0],
          iconAnchor: [0, 0]
        })
      }).addTo(map.value!)
    }
  } catch {
    toast.error('Error al calcular la ruta')
  } finally {
    calculatingRoute.value = false
  }
}

async function completarEntrega(venta: any) {
  try {
    await api.ventas.updateEstado(venta.id, 'entregado')
    await store.loadAllVentas()
    if (selectedVenta.value?.id === venta.id) {
      selectedVenta.value = null
      if (routeLayer && map.value) {
        map.value.removeLayer(routeLayer)
        routeLayer = null
      }
      if (midpointMarker && map.value) {
        map.value.removeLayer(midpointMarker)
        midpointMarker = null
      }
    }
    toast.success('Entrega completada')
  } catch (e: any) {
    toast.error(e.message || 'Error al completar entrega')
  }
}

function formatDuration(minutes: number): string {
  if (minutes < 60) return `${minutes} min`
  const h = Math.floor(minutes / 60)
  const m = minutes % 60
  return `${h}h ${m}min`
}
</script>

<template>
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-truck"></i> Panel de Despacho</h1>
    </div>

    <div v-if="loading" class="loading-state"><i class="fas fa-spinner fa-spin"></i> Cargando...</div>

    <div v-else class="dashboard">
      <div class="map-section">
        <div ref="mapContainer" class="map-container"></div>
      </div>

      <div class="sidebar">
        <h3>Pedidos con ubicación</h3>
        <div v-if="calculatingRoute" class="route-calc">
          <i class="fas fa-spinner fa-spin"></i> Calculando ruta...
        </div>
        <div v-if="ventas.length === 0" class="empty-list">
          <i class="fas fa-map-marker-alt"></i>
          <p>No hay pedidos con ubicación registrada</p>
        </div>
        <div v-else class="orders-list">
          <div
            v-for="venta in ventas"
            :key="venta.id"
            class="order-card"
            :class="{ active: selectedVenta?.id === venta.id, [venta.estado]: true }"
            @click="selectVenta(venta)"
          >
            <div class="order-header">
              <span class="order-id">#{{ venta.id }}</span>
              <span class="order-status" :class="venta.estado">{{ venta.estado }}</span>
            </div>
            <p class="order-client"><i class="fas fa-user"></i> {{ venta.nombreUsuario }}</p>
            <p class="order-amount">${{ venta.total.toFixed(2) }}</p>
            <p class="order-coords" v-if="venta.latitudEntrega && venta.longitudEntrega">
              <i class="fas fa-map-pin"></i> {{ venta.latitudEntrega.toFixed(4) }}, {{ venta.longitudEntrega.toFixed(4) }}
            </p>
            <div v-if="selectedVenta?.id === venta.id && routeDistance !== null && routeDuration !== null" class="route-info">
              <span class="route-distance"><i class="fas fa-road"></i> {{ routeDistance.toFixed(1) }} km</span>
              <span class="route-time"><i class="fas fa-clock"></i> {{ formatDuration(Math.round(routeDuration / 60)) }}</span>
            </div>
            <button v-if="venta.estado !== 'entregado' && venta.estado !== 'cancelado'" class="btn-complete" @click.stop="completarEntrega(venta)">
              <i class="fas fa-check"></i> Completado
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container { max-width: 1400px; margin: 0 auto; padding: 20px; height: calc(100vh - 90px); display: flex; flex-direction: column; }

.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.page-header h1 { font-size: 24px; color: #333; }

.loading-state { text-align: center; padding: 40px; color: #888; }

.dashboard { flex: 1; display: flex; gap: 20px; min-height: 0; }

.map-section { flex: 1; background: white; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.06); }
.map-container { width: 100%; height: 100%; min-height: 500px; }

.sidebar { width: 340px; min-width: 340px; background: white; border-radius: 16px; padding: 20px; box-shadow: 0 4px 20px rgba(0,0,0,0.06); display: flex; flex-direction: column; }
.sidebar h3 { font-size: 16px; color: #333; margin-bottom: 16px; }

.route-calc { text-align: center; padding: 12px; color: #e91e63; font-size: 13px; }

.orders-list { display: flex; flex-direction: column; gap: 10px; overflow-y: auto; flex: 1; }

.order-card {
  padding: 14px; border-radius: 12px; cursor: pointer; transition: all 0.2s;
  border: 2px solid transparent; background: #fafafa;
}
.order-card:hover { border-color: #e91e63; background: #fdf2f8; }
.order-card.active { border-color: #e91e63; background: #fce4ec; }

.order-card.pendiente { border-left: 4px solid #ff9800; }
.order-card.confirmado { border-left: 4px solid #2196f3; }
.order-card.despachado { border-left: 4px solid #4caf50; }
.order-card.entregado { border-left: 4px solid #9e9e9e; }
.order-card.cancelado { border-left: 4px solid #f44336; }

.order-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 6px; }
.order-id { font-weight: 700; color: #333; font-size: 14px; }
.order-status { padding: 2px 10px; border-radius: 12px; font-size: 11px; font-weight: 600; text-transform: capitalize; }
.order-status.pendiente { background: #fff3e0; color: #e65100; }
.order-status.confirmado { background: #e3f2fd; color: #1565c0; }
.order-status.despachado { background: #e8f5e9; color: #2e7d32; }
.order-status.entregado { background: #f5f5f5; color: #616161; }
.order-status.cancelado { background: #fce4ec; color: #c62828; }

.order-client { font-size: 13px; color: #555; margin-bottom: 4px; }
.order-amount { font-size: 15px; font-weight: 600; color: #2e7d32; margin-bottom: 4px; }
.order-coords { font-size: 12px; color: #888; }

.route-info { display: flex; gap: 12px; margin-top: 8px; padding-top: 8px; border-top: 1px solid #f0f0f0; font-size: 12px; font-weight: 600; }
.route-distance { color: #2e7d32; display: flex; align-items: center; gap: 4px; }
.route-time { color: #1565c0; display: flex; align-items: center; gap: 4px; }
.btn-complete {
  width: 100%; margin-top: 8px; padding: 8px; border: none; border-radius: 8px;
  background: linear-gradient(135deg, #2e7d32, #66bb6a); color: white;
  font-size: 13px; font-weight: 600; font-family: 'Poppins', sans-serif;
  cursor: pointer; display: flex; align-items: center; justify-content: center; gap: 6px;
  transition: all 0.2s;
}
.btn-complete:hover { transform: translateY(-1px); box-shadow: 0 4px 12px rgba(46,125,50,0.3); }

.empty-list { text-align: center; padding: 30px; color: #aaa; }
.empty-list i { font-size: 36px; margin-bottom: 10px; display: block; }
.empty-list p { font-size: 14px; }

@media (max-width: 900px) {
  .dashboard { flex-direction: column; }
  .sidebar { width: 100%; min-width: unset; max-height: 300px; }
  .map-container { min-height: 400px; }
}
</style>
