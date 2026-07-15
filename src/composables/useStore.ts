import { reactive, watch } from 'vue'
import type { Usuario, Categoria, Producto, Venta, DetalleVenta, MetodoPago, AppData } from '@/types'
import seedData from '@/data/data.json'

const STORAGE_KEY = 'heladeria_data'
const SESSION_KEY = 'heladeria_session'

function loadData(): AppData {
  const stored = localStorage.getItem(STORAGE_KEY)
  if (stored) {
    try {
      return JSON.parse(stored)
    } catch {
      return JSON.parse(JSON.stringify(seedData))
    }
  }
  return JSON.parse(JSON.stringify(seedData))
}

const state = reactive<AppData>(loadData())

watch(state, () => {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state))
}, { deep: true })

export function useStore() {

  function getSession(): Usuario | null {
    const s = sessionStorage.getItem(SESSION_KEY)
    if (s) {
      try { return JSON.parse(s) } catch { return null }
    }
    return null
  }

  function setSession(user: Usuario | null) {
    if (user) sessionStorage.setItem(SESSION_KEY, JSON.stringify(user))
    else sessionStorage.removeItem(SESSION_KEY)
  }

  function login(email: string, password: string): Usuario | null {
    const user = state.usuarios.find(u => u.email === email && u.password === password)
    if (user) {
      setSession(user)
      return user
    }
    return null
  }

  function logout() {
    setSession(null)
  }

  function register(nombre: string, email: string, password: string): string | null {
    if (state.usuarios.some(u => u.email === email)) {
      return 'Ya existe una cuenta con ese correo.'
    }
    if (password.length < 6) {
      return 'La contrasena debe tener al menos 6 caracteres.'
    }
    const newUser: Usuario = {
      id: Math.max(0, ...state.usuarios.map(u => u.id)) + 1,
      nombre,
      email,
      password,
      rol: 'empleado'
    }
    state.usuarios.push(newUser)
    return null
  }

  function getCategorias(): Categoria[] {
    return state.categorias
  }

  function getProductos(): Producto[] {
    return state.productos.filter(p => p.activo)
  }

  function getAllProductos(): Producto[] {
    return state.productos
  }

  function addProducto(data: { nombre: string; descripcion: string; precio: number; stock: number; id_categoria: number }) {
    const newProduct: Producto = {
      id: Math.max(0, ...state.productos.map(p => p.id)) + 1,
      ...data,
      activo: true
    }
    state.productos.push(newProduct)
  }

  function updateProducto(id: number, data: Partial<Producto>) {
    const prod = state.productos.find(p => p.id === id)
    if (prod) {
      Object.assign(prod, data)
    }
  }

  function deleteProducto(id: number) {
    const prod = state.productos.find(p => p.id === id)
    if (prod) {
      prod.activo = false
    }
  }

  function getVentas(): Venta[] {
    return [...state.ventas].sort((a, b) => new Date(b.fecha_venta).getTime() - new Date(a.fecha_venta).getTime())
  }

  function addVenta(usuarioId: number, items: { productoId: number; cantidad: number }[], metodo: MetodoPago, observaciones: string): string | null {
    let total = 0
    const resolvedItems: { producto: Producto; cantidad: number; precio: number; subtotal: number }[] = []

    for (const item of items) {
      const prod = state.productos.find(p => p.id === item.productoId && p.activo)
      if (!prod) return `Producto ID ${item.productoId} no encontrado.`
      if (prod.stock < item.cantidad) return `Stock insuficiente para "${prod.nombre}".`
      const subtotal = prod.precio * item.cantidad
      total += subtotal
      resolvedItems.push({ producto: prod, cantidad: item.cantidad, precio: prod.precio, subtotal })
    }

    if (total <= 0) return 'La venta debe tener un total mayor a cero.'

    const ventaId = Math.max(0, ...state.ventas.map(v => v.id)) + 1
    const venta: Venta = {
      id: ventaId,
      id_usuario: usuarioId,
      total,
      metodo_pago: metodo,
      observaciones,
      fecha_venta: new Date().toISOString()
    }
    state.ventas.push(venta)

    const detId = state.detalleVenta.length > 0
      ? Math.max(0, ...state.detalleVenta.map(d => d.id)) + 1
      : 1

    resolvedItems.forEach((ri, i) => {
      state.detalleVenta.push({
        id: detId + i,
        id_venta: ventaId,
        id_producto: ri.producto.id,
        cantidad: ri.cantidad,
        precio_unitario: ri.precio,
        subtotal: ri.subtotal
      })
      const prod = state.productos.find(p => p.id === ri.producto.id)
      if (prod) prod.stock -= ri.cantidad
    })

    return null
  }

  function deleteVenta(id: number) {
    state.detalleVenta = state.detalleVenta.filter(d => d.id_venta !== id)
    state.ventas = state.ventas.filter(v => v.id !== id)
  }

  function getDetalleVenta(ventaId: number): DetalleVenta[] {
    return state.detalleVenta.filter(d => d.id_venta === ventaId)
  }

  function getUsuarioById(id: number): Usuario | undefined {
    return state.usuarios.find(u => u.id === id)
  }

  function getProductoById(id: number): Producto | undefined {
    return state.productos.find(p => p.id === id)
  }

  return {
    state,
    getSession,
    login,
    logout,
    register,
    getCategorias,
    getProductos,
    getAllProductos,
    addProducto,
    updateProducto,
    deleteProducto,
    getVentas,
    addVenta,
    deleteVenta,
    getDetalleVenta,
    getUsuarioById,
    getProductoById
  }
}
