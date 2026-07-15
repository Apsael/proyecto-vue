export interface Usuario {
  id: number
  nombre: string
  email: string
  password: string
  rol: 'admin' | 'empleado'
}

export interface Categoria {
  id: number
  nombre: string
  descripcion: string
}

export interface Producto {
  id: number
  nombre: string
  descripcion: string
  precio: number
  stock: number
  id_categoria: number
  activo: boolean
}

export type MetodoPago = 'efectivo' | 'tarjeta' | 'transferencia'

export interface Venta {
  id: number
  id_usuario: number
  total: number
  metodo_pago: MetodoPago
  observaciones: string
  fecha_venta: string
}

export interface DetalleVenta {
  id: number
  id_venta: number
  id_producto: number
  cantidad: number
  precio_unitario: number
  subtotal: number
}

export interface AppData {
  usuarios: Usuario[]
  categorias: Categoria[]
  productos: Producto[]
  ventas: Venta[]
  detalleVenta: DetalleVenta[]
}
