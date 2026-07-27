import { reactive } from 'vue'
import { api } from '@/services/api'
import type { ProductoResponse, Categoria, VentaResponse } from '@/services/api'

const SESSION_KEY = 'heladeria_session'
const CART_KEY = 'heladeria_cart'

interface SessionUser {
  id: number
  nombre: string
  email: string
  rol: string
  token: string
}

interface CartItem {
  producto: ProductoResponse
  cantidad: number
}

interface AppState {
  user: SessionUser | null
  cart: CartItem[]
  productos: ProductoResponse[]
  categorias: Categoria[]
  ventas: VentaResponse[]
}

function loadSession(): SessionUser | null {
  const s = sessionStorage.getItem(SESSION_KEY)
  if (s) {
    try { return JSON.parse(s) } catch { return null }
  }
  return null
}

function loadCart(): CartItem[] {
  const c = localStorage.getItem(CART_KEY)
  if (c) {
    try { return JSON.parse(c) } catch { return [] }
  }
  return []
}

const state = reactive<AppState>({
  user: loadSession(),
  cart: loadCart(),
  productos: [],
  categorias: [],
  ventas: [],
})

function saveCart() {
  localStorage.setItem(CART_KEY, JSON.stringify(state.cart))
}

export function useStore() {

  function getSession(): SessionUser | null {
    return state.user
  }

  function setSession(user: SessionUser | null) {
    state.user = user
    if (user) sessionStorage.setItem(SESSION_KEY, JSON.stringify(user))
    else sessionStorage.removeItem(SESSION_KEY)
  }

  async function login(email: string, password: string): Promise<string | null> {
    try {
      const res = await api.auth.login(email, password)
      setSession(res)
      return null
    } catch (e: any) {
      return e.message || 'Error al iniciar sesion'
    }
  }

  async function register(nombre: string, email: string, password: string): Promise<string | null> {
    try {
      const res = await api.auth.register(nombre, email, password)
      setSession(res)
      return null
    } catch (e: any) {
      return e.message || 'Error al registrarse'
    }
  }

  function logout() {
    setSession(null)
    state.cart = []
    localStorage.removeItem(CART_KEY)
  }

  function isAdmin(): boolean {
    return state.user?.rol === 'admin'
  }

  async function loadProductos(): Promise<void> {
    state.productos = await api.productos.getAll()
  }

  async function loadAllProductos(): Promise<void> {
    state.productos = await api.productos.getAllAdmin()
  }

  async function loadCategorias(): Promise<void> {
    state.categorias = await api.categorias.getAll()
  }

  async function loadVentas(): Promise<void> {
    state.ventas = await api.ventas.getAll()
  }

  async function loadMisCompras(): Promise<void> {
    state.ventas = await api.ventas.getMyPurchases()
  }

  function getProductos(): ProductoResponse[] {
    return state.productos.filter(p => p.activo)
  }

  function getAllProductos(): ProductoResponse[] {
    return state.productos
  }

  function getCategorias(): Categoria[] {
    return state.categorias
  }

  function getVentas(): VentaResponse[] {
    return [...state.ventas].sort((a, b) => new Date(b.fechaVenta).getTime() - new Date(a.fechaVenta).getTime())
  }

  function addToCart(producto: ProductoResponse, cantidad: number = 1) {
    const existing = state.cart.find(item => item.producto.id === producto.id)
    if (existing) {
      existing.cantidad += cantidad
    } else {
      state.cart.push({ producto, cantidad })
    }
    saveCart()
  }

  function updateCartItem(productoId: number, cantidad: number) {
    const item = state.cart.find(i => i.producto.id === productoId)
    if (item) {
      if (cantidad <= 0) {
        removeFromCart(productoId)
      } else {
        item.cantidad = cantidad
        saveCart()
      }
    }
  }

  function removeFromCart(productoId: number) {
    state.cart = state.cart.filter(i => i.producto.id !== productoId)
    saveCart()
  }

  function getCart(): CartItem[] {
    return state.cart
  }

  function getCartTotal(): number {
    return state.cart.reduce((sum, item) => sum + item.producto.precio * item.cantidad, 0)
  }

  function getCartCount(): number {
    return state.cart.reduce((sum, item) => sum + item.cantidad, 0)
  }

  function clearCart() {
    state.cart = []
    saveCart()
  }

  return {
    state,
    getSession,
    setSession,
    login,
    register,
    logout,
    isAdmin,
    loadProductos,
    loadAllProductos,
    loadCategorias,
    loadVentas,
    loadMisCompras,
    getProductos,
    getAllProductos,
    getCategorias,
    getVentas,
    addToCart,
    updateCartItem,
    removeFromCart,
    getCart,
    getCartTotal,
    getCartCount,
    clearCart,
  }
}
