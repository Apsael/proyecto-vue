<script setup lang="ts">
import { useToast } from '@/composables/useToast'

const { toasts } = useToast()
</script>

<template>
  <Teleport to="body">
    <div class="toast-container">
      <TransitionGroup name="toast">
        <div
          v-for="toast in toasts"
          :key="toast.id"
          class="toast"
          :class="toast.type"
        >
          <i :class="
            toast.type === 'success' ? 'fas fa-check-circle' :
            toast.type === 'error' ? 'fas fa-exclamation-circle' :
            'fas fa-info-circle'
          "></i>
          <span>{{ toast.message }}</span>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<style scoped>
.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 10px;
  pointer-events: none;
}

.toast {
  pointer-events: auto;
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 14px 20px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 500;
  font-family: 'Poppins', sans-serif;
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
  min-width: 280px;
  max-width: 420px;
  backdrop-filter: blur(10px);
}

.toast.success {
  background: #f0fff4;
  color: #2e7d32;
  border-left: 4px solid #4caf50;
}

.toast.error {
  background: #fff0f3;
  color: #c62828;
  border-left: 4px solid #ef5350;
}

.toast.info {
  background: #e3f2fd;
  color: #1565c0;
  border-left: 4px solid #2196f3;
}

.toast-enter-active {
  transition: all 0.35s ease;
}

.toast-leave-active {
  transition: all 0.25s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(80px);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(80px) scale(0.95);
}
</style>
