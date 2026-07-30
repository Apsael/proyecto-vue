import { ref } from 'vue'

export interface Toast {
  id: number
  message: string
  type: 'success' | 'error' | 'info'
  position?: 'left' | 'right'
}

const toasts = ref<Toast[]>([])
let nextId = 0

export function useToast() {
  function show(message: string, type: Toast['type'] = 'info', duration = 3500, position: 'left' | 'right' = 'left') {
    const id = nextId++
    toasts.value.push({ id, message, type, position })
    setTimeout(() => {
      toasts.value = toasts.value.filter(t => t.id !== id)
    }, duration)
  }

  function success(message: string) {
    show(message, 'success')
  }

  function error(message: string) {
    show(message, 'error', 4500)
  }

  function info(message: string) {
    show(message, 'info')
  }

  return { toasts, show, success, error, info }
}
