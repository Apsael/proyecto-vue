<script setup lang="ts">
import TopBar from '@/components/TopBar.vue'
</script>

<template>
  <TopBar />
  <div class="main-content">
    <h1 class="page-title">Panel de Control</h1>
    <p class="page-subtitle">Selecciona una opcion para comenzar</p>

    <div class="menu-grid">
      <router-link to="/productos" class="menu-card">
        <div class="card-icon icon-1"><i class="fas fa-ice-cream"></i></div>
        <h3>Productos</h3>
        <p>Administra el catalogo de helados, paletas y mas</p>
      </router-link>

      <router-link to="/ventas" class="menu-card">
        <div class="card-icon icon-2"><i class="fas fa-cash-register"></i></div>
        <h3>Registro de Ventas</h3>
        <p>Registra nuevas ventas y gestiona pedidos</p>
      </router-link>

      <router-link to="/reportes" class="menu-card">
        <div class="card-icon icon-3"><i class="fas fa-chart-bar"></i></div>
        <h3>Reportes</h3>
        <p>Consulta estadisticas e informes de ventas</p>
      </router-link>
    </div>
  </div>
  <div class="footer-bar">Heladeria La Dolce Vita &copy; 2026</div>
</template>

<style scoped>
.main-content {
  max-width: 1100px;
  margin: 0 auto;
  padding: 50px 30px;
  text-align: center;
}

.page-title {
  color: #333;
  font-size: 32px;
  font-weight: 700;
  margin-bottom: 8px;
}

.page-subtitle {
  color: #888;
  font-size: 16px;
  margin-bottom: 50px;
  font-weight: 300;
}

.menu-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 28px;
  max-width: 900px;
  margin: 0 auto;
}

.menu-card {
  background: white;
  border-radius: 20px;
  padding: 40px 25px;
  text-decoration: none;
  color: #333;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
}

.menu-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  border-radius: 20px 20px 0 0;
}

.menu-card:nth-child(1)::before { background: linear-gradient(90deg, #e91e63, #f06292); }
.menu-card:nth-child(2)::before { background: linear-gradient(90deg, #ff9800, #ffb74d); }
.menu-card:nth-child(3)::before { background: linear-gradient(90deg, #4caf50, #81c784); }

.menu-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.15);
}

.card-icon {
  width: 70px;
  height: 70px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 18px;
  font-size: 28px;
  color: white;
}

.icon-1 { background: linear-gradient(135deg, #e91e63, #f06292); }
.icon-2 { background: linear-gradient(135deg, #ff9800, #ffb74d); }
.icon-3 { background: linear-gradient(135deg, #4caf50, #81c784); }

.menu-card h3 {
  font-size: 17px;
  font-weight: 600;
  margin-bottom: 8px;
}

.menu-card p {
  font-size: 13px;
  color: #888;
  line-height: 1.5;
}

.footer-bar {
  text-align: center;
  padding: 30px;
  color: #ccc;
  font-size: 12px;
}
</style>
