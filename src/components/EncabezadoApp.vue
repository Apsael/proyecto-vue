<script setup>
defineProps({
  titulo: {
    type: String,
    required: true
  },

  subtitulo: {
    type: String,
    default: ''
  },

  parrafo: {
    type: String,
    default: ''
  }
})
</script>

<template>
  <header class="encabezado-app">
    <h1>{{ titulo }}</h1>
    <h2 v-if="subtitulo">{{ subtitulo }}</h2>
    <p>{{ parrafo }}</p>
  </header>
</template>

<style scoped>
.encabezado-app {
  text-align: center;
  margin-bottom: 24px;
  background: #1a1717;
  color: white;
}
</style>
