<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from '@/composables/useStore'
import { useToast } from '@/composables/useToast'

const router = useRouter()
const store = useStore()
const toast = useToast()

const search = ref('')
const loading = ref(true)

onMounted(async () => {
  try {
    await Promise.all([store.loadProductos(), store.loadCategorias()])
  } catch {
    toast.error('Error al cargar los productos')
  } finally {
    loading.value = false
  }
})

const productosFiltrados = computed(() => {
  const prods = store.getProductos()
  if (!search.value) return prods
  const q = search.value.toLowerCase()
  return prods.filter(p =>
    p.nombre.toLowerCase().includes(q) ||
    (p.descripcion && p.descripcion.toLowerCase().includes(q)) ||
    (p.nombreCategoria && p.nombreCategoria.toLowerCase().includes(q))
  )
})

const categorias = computed(() => store.getCategorias())

function getCategoriaNombre(id: number): string {
  return categorias.value.find(c => c.id === id)?.nombre ?? ''
}

function addToCart(producto: any) {
  if (!store.getSession()) {
    toast.info('Inicia sesion para agregar productos al carrito')
    router.push('/login')
    return
  }
  store.addToCart(producto)
  toast.success(`${producto.nombre} agregado al carrito`)
}

const featuredProducts = computed(() => store.getProductos().slice(0, 6))
</script>

<template>
  <div class="home-page">
    <section class="hero">
      <div class="hero-content">
        <h1>La Dolce Vita</h1>
        <p class="hero-subtitle">Heladeria Artesanal</p>
        <p class="hero-desc">Descubre nuestros helados artesanales hechos con los mejores ingredientes naturales. Cada bocado es una experiencia unica de sabor.</p>
        <div class="hero-actions">
          <a href="#catalogo" class="btn-hero-primary"><i class="fas fa-ice-cream"></i> Ver Catalogo</a>
          <router-link to="/about" class="btn-hero-secondary"><i class="fas fa-info-circle"></i> Conocenos</router-link>
        </div>
      </div>
      <div class="hero-visual">
        <div class="hero-icon"><i class="fas fa-ice-cream"></i></div>
      </div>
    </section>

    <section class="features">
      <div class="feature">
        <div class="feature-icon"><i class="fas fa-leaf"></i></div>
        <h3>100% Natural</h3>
        <p>Ingredientes frescos y naturales sin conservantes artificiales</p>
      </div>
      <div class="feature">
        <div class="feature-icon"><i class="fas fa-hand-holding-heart"></i></div>
        <h3>Artesanal</h3>
        <p>Elaborados artesanalmente con recetas unicas y tradicionales</p>
      </div>
      <div class="feature">
        <div class="feature-icon"><i class="fas fa-truck"></i></div>
        <h3>Delivery</h3>
        <p>Recibe tus helados favoritos en la comodidad de tu hogar</p>
      </div>
    </section>

    <section id="catalogo" class="catalog-section">
      <div class="section-header">
        <h2><i class="fas fa-star"></i> Nuestros Productos</h2>
        <div class="search-bar">
          <i class="fas fa-search"></i>
          <input v-model="search" type="text" placeholder="Buscar productos..." />
        </div>
      </div>

      <div v-if="loading" class="loading-state">
        <i class="fas fa-spinner fa-spin"></i>
        <p>Cargando productos...</p>
      </div>

      <div v-else-if="productosFiltrados.length > 0" class="products-grid">
        <div v-for="p in productosFiltrados" :key="p.id" class="product-card">
          <div class="product-badge" v-if="p.stock < 20">Stock bajo</div>
          <div class="product-icon">
            <i class="fas fa-ice-cream"></i>
          </div>
          <div class="product-info">
            <span class="product-category">{{ p.nombreCategoria || 'Sin categoria' }}</span>
            <h3>{{ p.nombre }}</h3>
            <p class="product-desc">{{ p.descripcion }}</p>
            <div class="product-footer">
              <span class="product-price">${{ p.precio.toFixed(2) }}</span>
              <button class="btn-add-cart" @click="addToCart(p)" :disabled="p.stock === 0">
                <i class="fas fa-cart-plus"></i>
                {{ p.stock === 0 ? 'Sin stock' : 'Agregar' }}
              </button>
            </div>
          </div>
        </div>
      </div>

      <div v-else class="empty-state">
        <i class="fas fa-search"></i>
        <p>No se encontraron productos</p>
      </div>
    </section>

    <section class="cta">
      <h2>¿Listo para probar?</h2>
      <p>Registrate y empieza a disfrutar de los mejores helados artesanales</p>
      <router-link to="/register" class="btn-cta">
        <i class="fas fa-user-plus"></i> Crear Cuenta
      </router-link>
    </section>

    <footer class="footer">
      <p>Heladeria La Dolce Vita &copy; 2026 &mdash; Todos los derechos reservados</p>
    </footer>
  </div>
</template>

<style scoped>
.home-page {
  min-height: 100vh;
}

.hero {
  background: linear-gradient(135deg, #ff9a9e 0%, #fecfef 50%, #fdfcfb 100%);
  padding: 80px 40px 60px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  max-width: 1200px;
  margin: 0 auto;
  gap: 40px;
}

.hero-content { flex: 1; }

.hero-content h1 {
  font-size: 48px;
  font-weight: 700;
  color: #e91e63;
  margin-bottom: 4px;
}

.hero-subtitle {
  font-size: 18px;
  color: #888;
  margin-bottom: 20px;
  font-weight: 300;
}

.hero-desc {
  font-size: 16px;
  color: #555;
  line-height: 1.7;
  margin-bottom: 30px;
  max-width: 500px;
}

.hero-actions {
  display: flex;
  gap: 14px;
}

.btn-hero-primary {
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  padding: 14px 28px;
  border-radius: 14px;
  font-size: 15px;
  font-weight: 600;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s;
}

.btn-hero-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(233, 30, 99, 0.3);
}

.btn-hero-secondary {
  background: white;
  color: #e91e63;
  padding: 14px 28px;
  border-radius: 14px;
  font-size: 15px;
  font-weight: 600;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  border: 2px solid #e91e63;
  transition: all 0.3s;
}

.btn-hero-secondary:hover {
  background: #fce4ec;
}

.hero-visual {
  flex-shrink: 0;
}

.hero-icon {
  width: 180px;
  height: 180px;
  border-radius: 50%;
  background: linear-gradient(135deg, #e91e63, #f06292);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 72px;
  color: white;
  box-shadow: 0 20px 50px rgba(233, 30, 99, 0.3);
}

.features {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 30px;
  max-width: 1000px;
  margin: 50px auto;
  padding: 0 30px;
}

.feature {
  text-align: center;
  padding: 30px 20px;
  background: white;
  border-radius: 16px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05);
}

.feature-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: linear-gradient(135deg, #fce4ec, #f8bbd0);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 14px;
  font-size: 24px;
  color: #e91e63;
}

.feature h3 {
  font-size: 16px;
  color: #333;
  margin-bottom: 8px;
}

.feature p {
  font-size: 13px;
  color: #888;
  line-height: 1.5;
}

.catalog-section {
  max-width: 1200px;
  margin: 40px auto;
  padding: 0 30px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.section-header h2 {
  font-size: 26px;
  color: #333;
  display: flex;
  align-items: center;
  gap: 10px;
}

.search-bar {
  position: relative;
}

.search-bar i {
  position: absolute;
  left: 14px;
  top: 50%;
  transform: translateY(-50%);
  color: #bbb;
}

.search-bar input {
  padding: 10px 16px 10px 40px;
  border: 2px solid #eee;
  border-radius: 12px;
  font-size: 14px;
  font-family: 'Poppins', sans-serif;
  width: 280px;
  transition: all 0.3s;
  background: white;
}

.search-bar input:focus {
  outline: none;
  border-color: #e91e63;
  box-shadow: 0 0 0 3px rgba(233, 30, 99, 0.1);
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 24px;
}

.product-card {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
  transition: all 0.3s;
  position: relative;
}

.product-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 35px rgba(0, 0, 0, 0.12);
}

.product-badge {
  position: absolute;
  top: 12px;
  right: 12px;
  background: #fff3e0;
  color: #e65100;
  font-size: 11px;
  font-weight: 600;
  padding: 4px 10px;
  border-radius: 20px;
}

.product-icon {
  height: 140px;
  background: linear-gradient(135deg, #fce4ec, #f8bbd0);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 48px;
  color: #e91e63;
}

.product-info {
  padding: 20px;
}

.product-category {
  font-size: 11px;
  font-weight: 600;
  color: #e91e63;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.product-info h3 {
  font-size: 16px;
  color: #333;
  margin: 6px 0;
}

.product-desc {
  font-size: 13px;
  color: #888;
  line-height: 1.5;
  margin-bottom: 16px;
}

.product-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.product-price {
  font-size: 20px;
  font-weight: 700;
  color: #2e7d32;
}

.btn-add-cart {
  background: linear-gradient(135deg, #e91e63, #f06292);
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 10px;
  font-size: 12px;
  font-weight: 600;
  font-family: 'Poppins', sans-serif;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  transition: all 0.3s;
}

.btn-add-cart:hover:not(:disabled) {
  transform: scale(1.05);
  box-shadow: 0 4px 15px rgba(233, 30, 99, 0.3);
}

.btn-add-cart:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.loading-state, .empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #aaa;
}

.loading-state i, .empty-state i {
  font-size: 42px;
  margin-bottom: 14px;
  display: block;
}

.cta {
  text-align: center;
  padding: 60px 30px;
  background: linear-gradient(135deg, #fce4ec, #f3e5f5);
  margin-top: 50px;
}

.cta h2 {
  font-size: 28px;
  color: #333;
  margin-bottom: 10px;
}

.cta p {
  color: #666;
  margin-bottom: 24px;
}

.btn-cta {
  background: linear-gradient(135deg, #9c27b0, #ba68c8);
  color: white;
  padding: 14px 32px;
  border-radius: 14px;
  font-size: 16px;
  font-weight: 600;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s;
}

.btn-cta:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(156, 39, 176, 0.4);
}

.footer {
  text-align: center;
  padding: 30px;
  color: #ccc;
  font-size: 12px;
}

@media (max-width: 768px) {
  .hero { flex-direction: column; text-align: center; padding: 50px 20px; }
  .hero-desc { margin: 0 auto 30px; }
  .hero-actions { justify-content: center; }
  .hero-icon { width: 120px; height: 120px; font-size: 50px; }
  .features { grid-template-columns: 1fr; }
  .section-header { flex-direction: column; gap: 14px; align-items: flex-start; }
  .search-bar input { width: 100%; }
}
</style>
