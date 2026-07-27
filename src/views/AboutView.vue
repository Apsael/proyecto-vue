<script setup lang="ts">
</script>

<template>
  <div class="about-page">
    <section class="about-hero">
      <h1>Sobre Nosotros</h1>
      <p class="subtitle">Conoce la historia de La Dolce Vita</p>
    </section>

    <section class="about-content">
      <div class="about-card">
        <div class="about-icon"><i class="fas fa-heart"></i></div>
        <h2>Nuestra Historia</h2>
        <p>
          La Dolce Vita nacio con la pasion de ofrecer helados artesanales de la mas alta calidad.
          Desde nuestros inicios, nos hemos dedicado a crear sabores unicos utilizando ingredientes
          naturales y recetas tradicionales que han deleitado a nuestros clientes durante anos.
        </p>
      </div>

      <div class="about-card">
        <div class="about-icon"><i class="fas fa-award"></i></div>
        <h2>Nuestra Mision</h2>
        <p>
          Brindar a cada cliente una experiencia gastronomica excepcional a traves de helados
          artesanales elaborados con amor, dedicacion y los mejores ingredientes. Queremos ser
          parte de tus momentos mas dulces.
        </p>
      </div>

      <div class="about-card">
        <div class="about-icon"><i class="fas fa-gem"></i></div>
        <h2>Nuestros Valores</h2>
        <ul>
          <li><i class="fas fa-check"></i> Calidad sin compromisos</li>
          <li><i class="fas fa-check"></i> Ingredientes 100% naturales</li>
          <li><i class="fas fa-check"></i> Atencion personalizada</li>
          <li><i class="fas fa-check"></i> Innovacion constante en sabores</li>
          <li><i class="fas fa-check"></i> Compromiso con la comunidad</li>
        </ul>
      </div>

      <div class="about-card full-width">
        <div class="about-icon"><i class="fas fa-map-marker-alt"></i></div>
        <h2>Encuentranos</h2>
        <p>
          Visitanos en nuestra tienda principal. Estamos abiertos de lunes a domingo
          de 10:00 AM a 10:00 PM. Tambien puedes hacer tu pedido a traves de nuestra
          pagina y recibirlo en la comodidad de tu hogar.
        </p>
        <div class="contact-info">
          <div><i class="fas fa-phone"></i> +591 7000 1234</div>
          <div><i class="fas fa-envelope"></i> info@ladolcevita.com</div>
          <div><i class="fas fa-clock"></i> Lun - Dom: 10:00 AM - 10:00 PM</div>
        </div>
      </div>
    </section>

    <footer class="footer">
      <p>Heladeria La Dolce Vita &copy; 2026 &mdash; Todos los derechos reservados</p>
    </footer>
  </div>
</template>

<style scoped>
.about-page {
  min-height: 100vh;
}

.about-hero {
  background: linear-gradient(135deg, #a18cd1 0%, #fbc2eb 100%);
  padding: 60px 30px;
  text-align: center;
}

.about-hero h1 {
  font-size: 36px;
  color: #333;
  font-weight: 700;
}

.subtitle {
  color: #555;
  font-size: 16px;
  margin-top: 8px;
}

.about-content {
  max-width: 900px;
  margin: 40px auto;
  padding: 0 30px;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
}

.about-card {
  background: white;
  border-radius: 16px;
  padding: 30px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
}

.about-card.full-width {
  grid-column: 1 / -1;
}

.about-icon {
  width: 56px;
  height: 56px;
  border-radius: 14px;
  background: linear-gradient(135deg, #fce4ec, #f8bbd0);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 22px;
  color: #e91e63;
  margin-bottom: 16px;
}

.about-card h2 {
  font-size: 18px;
  color: #333;
  margin-bottom: 12px;
}

.about-card p {
  font-size: 14px;
  color: #666;
  line-height: 1.7;
}

.about-card ul {
  list-style: none;
  padding: 0;
}

.about-card ul li {
  padding: 6px 0;
  font-size: 14px;
  color: #555;
  display: flex;
  align-items: center;
  gap: 8px;
}

.about-card ul li i {
  color: #4caf50;
  font-size: 12px;
}

.contact-info {
  margin-top: 16px;
  display: flex;
  gap: 30px;
  flex-wrap: wrap;
}

.contact-info div {
  font-size: 14px;
  color: #555;
  display: flex;
  align-items: center;
  gap: 8px;
}

.contact-info i {
  color: #e91e63;
}

.footer {
  text-align: center;
  padding: 30px;
  color: #ccc;
  font-size: 12px;
}

@media (max-width: 768px) {
  .about-content { grid-template-columns: 1fr; }
  .contact-info { flex-direction: column; gap: 10px; }
}
</style>
