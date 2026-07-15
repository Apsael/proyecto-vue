<script setup lang="ts">
import EncabezadoApp from './components/EncabezadoApp.vue';
</script>


<template>
  <EncabezadoApp
    titulo="Mi Aplicación Vue 3"
    subtitulo="Bienvenido a mi aplicación"
  />
<main>
  <h1 class="hello-world">Hello World</h1>
  <p>Bienvenido a mi aplicación Vue 3</p>
</main>
</template>

<style scoped>
main {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
}

.hello-world {
  font-size: 4rem;
  font-weight: 700;
  text-transform: lowercase;
}
</style>