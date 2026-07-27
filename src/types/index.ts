export interface Usuario {
  id: number
  nombre: string
  email: string
  rol: 'admin' | 'cliente'
  activo: boolean
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
  nombre_categoria: string | null
  activo: boolean
}
