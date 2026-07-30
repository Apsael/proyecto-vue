<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue'
import { api } from '@/services/api'
import { useToast } from '@/composables/useToast'
import L from 'leaflet'

const toast = useToast()
const mapContainer = ref<HTMLDivElement>()
const map = ref<L.Map | null>(null)
const marker = ref<L.Marker | null>(null)
const loading = ref(true)
const saving = ref(false)
const latitud = ref(-17.7853)
const longitud = ref(-63.1806)

const companyIcon = L.divIcon({
  html: '<div style="background:#e91e63;color:white;width:40px;height:40px;border-radius:50%;display:flex;align-items:center;justify-content:center;font-size:18px;box-shadow:0 2px 8px rgba(0,0,0,0.3);border:3px solid white;"><i class="fas fa-store"></i></div>',
  className: '',
  iconSize: [40, 40],
  iconAnchor: [20, 20]
})

onMounted(async () => {
  try {
    const cfg = await api.config.getEmpresa()
    latitud.value = cfg.latitud
    longitud.value = cfg.longitud
  } catch {
    toast.error('Error al cargar configuración')
  } finally {
    loading.value = false
    await nextTick()
  }

  if (mapContainer.value) {
    map.value = L.map(mapContainer.value).setView([latitud.value, longitud.value], 15)
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map.value)

    marker.value = L.marker([latitud.value, longitud.value], { icon: companyIcon, draggable: true })
      .addTo(map.value)
      .bindPopup('Ubicación de la empresa<br><small>Arrastra o haz clic en el mapa</small>')
      .openPopup()

    marker.value.on('dragend', () => {
      const pos = marker.value!.getLatLng()
      latitud.value = parseFloat(pos.lat.toFixed(6))
      longitud.value = parseFloat(pos.lng.toFixed(6))
    })

    map.value.on('click', (e: L.LeafletMouseEvent) => {
      if (marker.value) {
        marker.value.setLatLng(e.latlng)
      } else {
        marker.value = L.marker(e.latlng, { icon: companyIcon, draggable: true }).addTo(map.value!)
      }
      latitud.value = parseFloat(e.latlng.lat.toFixed(6))
      longitud.value = parseFloat(e.latlng.lng.toFixed(6))
    })
  }
})

async function save() {
  saving.value = true
  try {
    const cfg = await api.config.updateEmpresa({ latitud: latitud.value, longitud: longitud.value })
    latitud.value = cfg.latitud
    longitud.value = cfg.longitud
    toast.success('Ubicación guardada correctamente')
    localStorage.setItem('empresaConfig', JSON.stringify(cfg))
  } catch (e: any) {
    toast.error(e.message || 'Error al guardar')
  } finally {
    saving.value = false
  }
}

function centerMap() {
  if (map.value && marker.value) {
    const pos = marker.value.getLatLng()
    map.value.setView(pos, 15)
  }
}
</script>

<template>
  <div class="container">
    <div class="page-header">
      <h1><i class="fas fa-store"></i> Ubicación de la Empresa</h1>
    </div>

    <div v-if="loading" class="loading-state"><i class="fas fa-spinner fa-spin"></i> Cargando...</div>

    <template v-else>
      <div class="map-section">
        <div ref="mapContainer" class="map-container"></div>
      </div>

      <div class="controls">
        <div class="coords-display">
          <div class="coord-box">
            <label>Latitud</label>
            <input type="text" :value="latitud" readonly />
          </div>
          <div class="coord-box">
            <label>Longitud</label>
            <input type="text" :value="longitud" readonly />
          </div>
          <button class="btn-secondary" @click="centerMap"><i class="fas fa-crosshairs"></i> Centrar</button>
        </div>

        <div class="hint">
          <i class="fas fa-info-circle"></i> Haz clic en el mapa o arrastra el marcador para cambiar la ubicación
        </div>

        <button class="btn-save" :disabled="saving" @click="save">
          <i :class="saving ? 'fas fa-spinner fa-spin' : 'fas fa-save'"></i>
          {{ saving ? 'Guardando...' : 'Guardar Ubicación' }}
        </button>
      </div>
    </template>
  </div>
</template>

<style scoped>
.container { max-width: 900px; margin: 30px auto; padding: 0 20px; }

.page-header h1 { font-size: 26px; color: #333; margin-bottom: 25px; }

.loading-state { text-align: center; padding: 40px; color: #888; font-size: 16px; }

.map-section { background: white; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.06); }
.map-container { width: 100%; height: 450px; }

.controls { margin-top: 24px; background: white; border-radius: 16px; padding: 24px; box-shadow: 0 4px 20px rgba(0,0,0,0.06); }

.coords-display { display: flex; gap: 12px; align-items: flex-end; margin-bottom: 16px; }
.coord-box { flex: 1; }
.coord-box label { display: block; font-size: 12px; font-weight: 500; color: #888; margin-bottom: 4px; }
.coord-box input { width: 100%; padding: 10px 12px; border: 2px solid #eee; border-radius: 10px; font-size: 14px; font-family: 'Poppins', sans-serif; background: #fafafa; color: #555; }

.btn-secondary { padding: 10px 18px; background: #f5f5f5; color: #555; border: none; border-radius: 10px; font-size: 13px; font-family: 'Poppins', sans-serif; cursor: pointer; font-weight: 500; white-space: nowrap; transition: all 0.2s; }
.btn-secondary:hover { background: #eee; }

.hint { font-size: 13px; color: #888; margin-bottom: 16px; display: flex; align-items: center; gap: 6px; }
.hint i { color: #e91e63; }

.btn-save {
  width: 100%; padding: 14px; background: linear-gradient(135deg, #e91e63, #f06292);
  color: white; border: none; border-radius: 12px; font-size: 15px; font-weight: 600;
  font-family: 'Poppins', sans-serif; cursor: pointer; display: flex; align-items: center;
  justify-content: center; gap: 8px; transition: all 0.3s;
}
.btn-save:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(233,30,99,0.3); }
.btn-save:disabled { opacity: 0.7; cursor: not-allowed; }

@media (max-width: 768px) {
  .map-container { height: 350px; }
  .coords-display { flex-direction: column; }
}
</style>
