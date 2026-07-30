<script setup lang="ts">
import { useToast } from '@/composables/useToast'

const { toasts } = useToast()
</script>

<template>
  <Teleport to="body">
    <div class="toast-container toast-left">
      <TransitionGroup name="toast">
        <div
          v-for="toast in toasts"
          :key="toast.id"
          class="toast"
          :class="[toast.type, toast.position === 'right' ? 'toast-right' : '']"
        >
          <div class="toast-icon">
            <i :class="
              toast.type === 'success' ? 'fas fa-check-circle' :
              toast.type === 'error' ? 'fas fa-exclamation-circle' :
              'fas fa-info-circle'
            "></i>
          </div>
          <div class="toast-content">
            <span class="toast-title">{{ toast.type === 'success' ? 'Éxito' : toast.type === 'error' ? 'Error' : 'Información' }}</span>
            <span class="toast-message">{{ toast.message }}</span>
          </div>
          <button class="toast-close" @click="toasts = toasts.filter(t => t.id !== toast.id)">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<style scoped>
.toast-container {
  position: fixed;
  top: 20px;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 10px;
  pointer-events: none;
  max-width: 420px;
  width: calc(100% - 40px);
}

.toast-left {
  left: 20px;
  align-items: flex-start;
}

.toast-right {
  align-self: flex-end;
}

.toast {
  pointer-events: auto;
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 16px 18px;
  border-radius: 14px;
  font-size: 14px;
  font-weight: 500;
  font-family: 'Poppins', sans-serif;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.12);
  min-width: 300px;
  max-width: 420px;
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  position: relative;
  animation: toast-slide-in 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

.toast.success {
  background: linear-gradient(135deg, #f0fff4, #e8f5e9);
  color: #1b5e20;
  border-left: 4px solid #4caf50;
}

.toast.error {
  background: linear-gradient(135deg, #fff0f3, #fce4ec);
  color: #b71c1c;
  border-left: 4px solid #ef5350;
}

.toast.info {
  background: linear-gradient(135deg, #e3f2fd, #e8eaf6);
  color: #0d47a1;
  border-left: 4px solid #2196f3;
}

.toast-icon {
  font-size: 20px;
  flex-shrink: 0;
  margin-top: 1px;
}

.toast.success .toast-icon { color: #4caf50; }
.toast.error .toast-icon { color: #ef5350; }
.toast.info .toast-icon { color: #2196f3; }

.toast-content {
  display: flex;
  flex-direction: column;
  gap: 2px;
  flex: 1;
}

.toast-title {
  font-size: 13px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.3px;
}

.toast-message {
  font-size: 13px;
  font-weight: 400;
  opacity: 0.85;
  line-height: 1.4;
}

.toast-close {
  background: none;
  border: none;
  color: inherit;
  opacity: 0.4;
  cursor: pointer;
  font-size: 12px;
  padding: 2px;
  flex-shrink: 0;
  transition: opacity 0.2s;
}

.toast-close:hover {
  opacity: 0.8;
}

@keyframes toast-slide-in {
  from {
    opacity: 0;
    transform: translateX(-30px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateX(0) scale(1);
  }
}

.toast-enter-active {
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(-30px) scale(0.95);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(-30px) scale(0.9);
}

@media (max-width: 768px) {
  .toast-container {
    left: 10px;
    right: 10px;
    max-width: none;
    width: auto;
  }
  .toast {
    min-width: auto;
  }
}
</style>
