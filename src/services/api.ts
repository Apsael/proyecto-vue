const API_BASE = 'http://localhost:5057/api'
function getToken(): string | null {
  const session = sessionStorage.getItem('heladeria_session')
  if (session) {
    try {
      const parsed = JSON.parse(session)
      return parsed.token || null
    } catch {
      return null
    }
  }
  return null
}

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = getToken()
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string> || {}),
  }
  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }

  const response = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers,
  })

  if (!response.ok) {
    const body = await response.json().catch(() => null)
    const msg = body?.mensaje || body?.message || `Error ${response.status}`
    throw new Error(msg)
  }

  if (response.status === 204) return undefined as T
  return response.json()
}

export interface AuthResponse {
  id: number
  nombre: string
  email: string
  rol: string
  token: string
  verificado: boolean
  latitud?: number | null
  longitud?: number | null
}

export interface UsuarioResponse {
  id: number
  nombre: string
  email: string
  rol: string
  activo: boolean
  fechaCreacion: string
}

export interface Categoria {
  id: number
  nombre: string
  descripcion: string | null
}

export interface ProductoResponse {
  id: number
  nombre: string
  descripcion: string | null
  precio: number
  stock: number
  imagenUrl: string | null
  idCategoria: number
  nombreCategoria: string | null
  activo: boolean
  fechaCreacion: string
}

export interface DetalleVentaResponse {
  id: number
  nombreProducto: string | null
  cantidad: number
  precioUnitario: number
  subtotal: number
}

export interface VentaResponse {
  id: number
  total: number
  metodoPago: string
  observaciones: string | null
  direccionEnvio: string | null
  estado: string
  latitudEntrega: number | null
  longitudEntrega: number | null
  fechaVenta: string
  nombreUsuario: string | null
  emailUsuario: string | null
  detalles: DetalleVentaResponse[]
}

export interface EmpresaConfig {
  nombre: string
  direccion: string
  telefono: string
  email: string
  latitud: number
  longitud: number
  horario: string
}

export const api = {
  auth: {
    login: (email: string, password: string) =>
      request<AuthResponse>('/auth/login', {
        method: 'POST',
        body: JSON.stringify({ email, password }),
      }),
    register: (nombre: string, email: string, password: string, latitud?: number, longitud?: number) =>
      request<AuthResponse>('/auth/register', {
        method: 'POST',
        body: JSON.stringify({ nombre, email, password, latitud, longitud }),
      }),
    me: () => request<AuthResponse>('/auth/me'),
    updateProfile: (nombre: string, email: string) =>
      request<AuthResponse>('/auth/perfil', {
        method: 'PUT',
        body: JSON.stringify({ nombre, email }),
      }),
    changePassword: (currentPassword: string, newPassword: string) =>
      request<{ mensaje: string }>('/auth/password', {
        method: 'PUT',
        body: JSON.stringify({ currentPassword, newPassword }),
      }),
    updateUbicacion: (latitud: number, longitud: number) =>
      request<{ mensaje: string }>('/auth/ubicacion', {
        method: 'PUT',
        body: JSON.stringify({ latitud, longitud }),
      }),
    verificarEmail: (token: string) =>
      request<{ mensaje: string }>('/auth/verificar', {
        method: 'POST',
        body: JSON.stringify({ token }),
      }),
    reenviarVerificacion: (email: string) =>
      request<{ mensaje: string; token: string }>('/auth/reenviar-verificacion', {
        method: 'POST',
        body: JSON.stringify({ email }),
      }),
  },

  productos: {
    getAll: () => request<ProductoResponse[]>('/productos'),
    getAllAdmin: () => request<ProductoResponse[]>('/productos/all'),
    getById: (id: number) => request<ProductoResponse>(`/productos/${id}`),
    search: (nombre: string) =>
      request<ProductoResponse[]>(`/productos/buscar?nombre=${encodeURIComponent(nombre)}`),
    create: (data: { nombre: string; descripcion: string; precio: number; stock: number; idCategoria: number; imagenUrl?: string }) =>
      request<ProductoResponse>('/productos', {
        method: 'POST',
        body: JSON.stringify(data),
      }),
    update: (id: number, data: { nombre: string; descripcion: string; precio: number; stock: number; idCategoria: number; imagenUrl?: string }) =>
      request<void>(`/productos/${id}`, {
        method: 'PUT',
        body: JSON.stringify(data),
      }),
    delete: (id: number) =>
      request<void>(`/productos/${id}`, { method: 'DELETE' }),
  },

  categorias: {
    getAll: () => request<Categoria[]>('/categorias'),
    create: (data: { nombre: string; descripcion: string }) =>
      request<Categoria>('/categorias', {
        method: 'POST',
        body: JSON.stringify(data),
      }),
    update: (id: number, data: { nombre: string; descripcion: string }) =>
      request<void>(`/categorias/${id}`, {
        method: 'PUT',
        body: JSON.stringify(data),
      }),
    delete: (id: number) =>
      request<void>(`/categorias/${id}`, { method: 'DELETE' }),
  },

  usuarios: {
    getAll: () => request<UsuarioResponse[]>('/usuarios'),
    create: (data: { nombre: string; email: string; password: string; rol: string }) =>
      request<UsuarioResponse>('/usuarios', {
        method: 'POST',
        body: JSON.stringify(data),
      }),
    update: (id: number, data: { nombre: string; email: string; rol: string; activo: boolean }) =>
      request<void>(`/usuarios/${id}`, {
        method: 'PUT',
        body: JSON.stringify(data),
      }),
    delete: (id: number) =>
      request<void>(`/usuarios/${id}`, { method: 'DELETE' }),
  },

  ventas: {
    create: (data: {
      items: { productoId: number; cantidad: number }[]
      metodoPago: string
      observaciones: string
      direccionEnvio?: string
      latitudEntrega?: number
      longitudEntrega?: number
    }) =>
      request<VentaResponse>('/ventas', {
        method: 'POST',
        body: JSON.stringify(data),
      }),
    getAll: () => request<VentaResponse[]>('/ventas'),
    getMyPurchases: () => request<VentaResponse[]>('/ventas/mis-compras'),
    getById: (id: number) => request<VentaResponse>(`/ventas/${id}`),
    delete: (id: number) =>
      request<void>(`/ventas/${id}`, { method: 'DELETE' }),
    updateEstado: (id: number, estado: string) =>
      request<void>(`/ventas/${id}/estado`, {
        method: 'PATCH',
        body: JSON.stringify({ estado }),
      }),
  },

  config: {
    getEmpresa: () => request<EmpresaConfig>('/config/empresa'),
    updateEmpresa: (data: { latitud: number; longitud: number }) =>
      request<EmpresaConfig>('/config/empresa', {
        method: 'PUT',
        body: JSON.stringify(data),
      }),
  },

  mail: {
    send: (to: string, subject: string, body: string) =>
      request<{ success: boolean; message: string }>('/mail/send', {
        method: 'POST',
        body: JSON.stringify({ to, subject, body }),
      }),
  },
}
