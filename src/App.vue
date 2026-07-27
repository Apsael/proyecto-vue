<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import Toast from '@/components/Toast.vue'
import TopBar from '@/components/TopBar.vue'

const route = useRoute()

const showTopBar = computed(() => {
  const noTopBar = ['login', 'register']
  return !noTopBar.includes(route.name as string)
})
</script>

<template>
  <Toast />
  <TopBar v-if="showTopBar" />
  <RouterView />
</template>
