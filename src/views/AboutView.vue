<script setup lang="ts">
</script>

<template>
  <div class="about-page">
    <section class="about-hero">
      <div class="hero-content">
        <img src="/logo.png" alt="La Dolce Vita" class="hero-logo" />
        <h1>Sobre Nosotros</h1>
        <p class="subtitle">Conoce la historia de La Dolce Vita</p>
      </div>
    </section>

    <section class="about-content">
      <div class="about-card">
        <div class="about-icon"><i class="fas fa-heart"></i></div>
        <h2>Nuestra Historia</h2>
        <p>
          La Dolce Vita nació con la pasión de ofrecer helados artesanales de la más alta calidad.
          Desde nuestros inicios, nos hemos dedicado a crear sabores únicos utilizando ingredientes
          naturales y recetas tradicionales que han deleitado a nuestros clientes durante años.
        </p>
      </div>

      <div class="about-card">
        <div class="about-icon"><i class="fas fa-award"></i></div>
        <h2>Nuestra Misión</h2>
        <p>
          Brindar a cada cliente una experiencia gastronómica excepcional a través de helados
          artesanales elaborados con amor, dedicación y los mejores ingredientes. Queremos ser
          parte de tus momentos más dulces.
        </p>
      </div>

      <div class="about-card">
        <div class="about-icon"><i class="fas fa-gem"></i></div>
        <h2>Nuestros Valores</h2>
        <ul>
          <li><i class="fas fa-check"></i> Calidad sin compromisos</li>
          <li><i class="fas fa-check"></i> Ingredientes 100% naturales</li>
          <li><i class="fas fa-check"></i> Atención personalizada</li>
          <li><i class="fas fa-check"></i> Innovación constante en sabores</li>
          <li><i class="fas fa-check"></i> Compromiso con la comunidad</li>
        </ul>
      </div>

      <div class="about-card full-width">
        <div class="about-icon"><i class="fas fa-map-marker-alt"></i></div>
        <h2>Encuéntranos</h2>
        <p>
          Visítanos en nuestra tienda principal. Estamos abiertos de lunes a domingo
          de 10:00 AM a 10:00 PM. También puedes hacer tu pedido a través de nuestra
          página y recibirlo en la comodidad de tu hogar.
        </p>
        <div class="contact-info">
          <a href="https://wa.me/59169112579" target="_blank" rel="noopener" class="contact-link"><i class="fab fa-whatsapp"></i> +591 69112579</a>
          <div><i class="fas fa-envelope"></i> info@ladolcevita.com</div>
          <div><i class="fas fa-clock"></i> Lun - Dom: 10:00 AM - 10:00 PM</div>
        </div>
      </div>

      <div class="about-card full-width map-card">
        <div class="about-icon"><i class="fas fa-map"></i></div>
        <h2>Ubicación</h2>
        <div class="mapouter">
          <div class="gmap_canvas">
            <iframe class="gmap_iframe" width="100%" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="https://maps.google.com/maps?width=600&amp;height=400&amp;hl=en&amp;q=Delicenter Beni&amp;t=&amp;z=14&amp;ie=UTF8&amp;iwloc=B&amp;output=embed"></iframe>
          </div>
        </div>
      </div>
    </section>

    <footer class="footer">
      <p>Heladería La Dolce Vita &copy; 2026 &mdash; Todos los derechos reservados</p>
    </footer>
  </div>
</template>

<style scoped>
.about-page { min-height: 100vh; }

.about-hero {
  background: linear-gradient(135deg, #a18cd1 0%, #fbc2eb 100%);
  padding: 60px 30px;
  text-align: center;
}

.hero-content { max-width: 600px; margin: 0 auto; }
.hero-logo { height: 70px; margin-bottom: 15px; }
.about-hero h1 { font-size: 36px; color: #333; font-weight: 700; }
.subtitle { color: #555; font-size: 16px; margin-top: 8px; }

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
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.about-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 35px rgba(0, 0, 0, 0.1);
}

.about-card.full-width { grid-column: 1 / -1; }

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

.about-card h2 { font-size: 18px; color: #333; margin-bottom: 12px; }
.about-card p { font-size: 14px; color: #666; line-height: 1.7; }

.about-card ul { list-style: none; padding: 0; }
.about-card ul li {
  padding: 6px 0;
  font-size: 14px;
  color: #555;
  display: flex;
  align-items: center;
  gap: 8px;
}
.about-card ul li i { color: #4caf50; font-size: 12px; }

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
.contact-info i { color: #e91e63; }
.contact-link { color: #555; text-decoration: none; display: flex; align-items: center; gap: 8px; font-size: 14px; transition: color 0.2s; }
.contact-link:hover { color: #25d366; }
.contact-link i.fa-whatsapp { color: #25d366; font-size: 18px; }

.map-card { padding: 20px; }
.mapouter {
  position: relative;
  text-align: right;
  width: 100%;
  height: 400px;
  border-radius: 12px;
  overflow: hidden;
}
.gmap_canvas {
  overflow: hidden;
  background: none!important;
  width: 100%;
  height: 400px;
}
.gmap_iframe {
  height: 400px!important;
}

.footer { text-align: center; padding: 30px; color: #ccc; font-size: 12px; }

@media (max-width: 768px) {
  .about-content { grid-template-columns: 1fr; }
  .contact-info { flex-direction: column; gap: 10px; }
  .mapouter { height: 300px; }
  .gmap_canvas { height: 300px; }
  .gmap_iframe { height: 300px!important; }
}
</style>
