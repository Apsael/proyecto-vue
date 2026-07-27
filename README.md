# La Dolce Vita - Heladeria Artesanal

Sistema completo de gestion para heladeria con **Vue 3 + TypeScript** (frontend) y **ASP.NET Core 10.0 + SQL Server** (backend). Incluye vitrina de productos, carrito de compras, autenticacion JWT, panel de administracion y reportes.

---

## Funcionalidades

### Publico (sin sesion)
- **Vitrina de productos** con busqueda por nombre/categoria
- **Pagina "Nosotros"** con informacion de la heladeria
- Navegacion libre por el catalogo

### Cliente (sesion iniciada)
- **Carrito de compras** con cantidad, subtotal y total
- **Checkout** con seleccion de metodo de pago (efectivo/tarjeta/transferencia)
- **Historial de compras** con detalle de cada pedido
- **Perfil** para actualizar nombre, correo y contrasena

### Administrador
- **Panel de control** con acceso rapido a todas las secciones
- **Gestion de productos**: crear, editar, desactivar (CRUD completo)
- **Gestion de usuarios**: crear, editar roles, activar/desactivar
- **Historial de ventas**: ver todas las ventas del sistema, eliminar con restauracion de stock
- **Reportes**: estadisticas, top productos, ventas por metodo de pago, alertas de stock bajo

### Seguridad
- Autenticacion **JWT** (tokens con expiracion de 8 horas)
- Contrasenas hasheadas con **BCrypt**
- Autorizacion por roles (admin/cliente)
- **CORS** configurado solo para el frontend
- Notificaciones **flotantes** (toast) que no desplazan la UI

---

## Credenciales iniciales

| Campo      | Valor                 |
|------------|-----------------------|
| Correo     | `admin@heladeria.com` |
| Contrasena | `admin123`            |

> Cambia la contrasena despues del primer inicio de sesion desde "Mi Perfil".

---

## Estructura del proyecto

```
proyecto-vue/
├── BackendApi/                          # Backend ASP.NET Core 10.0
│   ├── Controllers/
│   │   ├── AuthController.cs            # Login, registro, perfil, password
│   │   ├── ProductosController.cs       # CRUD productos (publico + admin)
│   │   ├── CategoriasController.cs      # CRUD categorias
│   │   ├── UsuariosController.cs        # Gestion de usuarios (admin)
│   │   └── VentasController.cs          # Ventas y historial de compras
│   ├── Data/
│   │   ├── ApplicationDbContext.cs       # DbContext y configuracion EF Core
│   │   └── SeedData.cs                  # Seeding de admin, categorias y productos
│   ├── Models/
│   │   ├── Usuario.cs
│   │   ├── Categoria.cs
│   │   ├── Producto.cs
│   │   ├── Venta.cs
│   │   ├── DetalleVenta.cs
│   │   └── Dtos/
│   │       ├── AuthDtos.cs              # Login, Register, Profile, Password
│   │       ├── ProductoDtos.cs          # Request/Response de productos
│   │       └── VentaDtos.cs             # Request/Response de ventas
│   ├── BackendApi.csproj
│   ├── Program.cs                       # Registro de servicios, JWT, CORS, seeder
│   ├── appsettings.json                 # Cadena de conexion y configuracion JWT
│   └── script.sql                       # Script SQL Server (esquema + datos)
│
├── src/                                 # Frontend Vue 3
│   ├── assets/
│   │   └── main.css                     # Estilos globales (Poppins, Font Awesome)
│   ├── components/
│   │   ├── Modal.vue                    # Modal reutilizable
│   │   ├── Toast.vue                    # Notificaciones flotantes
│   │   └── TopBar.vue                   # Barra de navegacion superior
│   ├── composables/
│   │   ├── useStore.ts                  # Estado central + llamadas API
│   │   └── useToast.ts                  # Sistema de notificaciones toast
│   ├── router/
│   │   └── index.ts                     # Rutas con guard de auth y roles
│   ├── services/
│   │   └── api.ts                       # Cliente HTTP para el backend
│   ├── types/
│   │   └── index.ts                     # Definiciones TypeScript
│   ├── views/
│   │   ├── HomeView.vue                 # Vitrina publica de productos
│   │   ├── AboutView.vue                # Pagina "Nosotros"
│   │   ├── LoginView.vue                # Inicio de sesion
│   │   ├── RegisterView.vue             # Registro de cliente
│   │   ├── CarritoView.vue              # Carrito de compras
│   │   ├── CheckoutView.vue             # Pago y confirmacion
│   │   ├── MisComprasView.vue           # Historial de compras del cliente
│   │   ├── MiPerfilView.vue             # Editar perfil y contrasena
│   │   ├── AdminDashboardView.vue       # Panel de control admin
│   │   ├── AdminProductosView.vue       # CRUD productos (admin)
│   │   ├── AdminUsuariosView.vue        # Gestion de usuarios (admin)
│   │   ├── AdminVentasView.vue          # Historial de ventas (admin)
│   │   └── AdminReportesView.vue        # Reportes y estadisticas
│   ├── App.vue                          # Componente raiz
│   └── main.ts                          # Punto de entrada
│
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
└── README.md
```

---

## Rutas

| Ruta                  | Descripcion                     | Auth    | Rol     |
|-----------------------|---------------------------------|---------|---------|
| `/`                   | Vitrina de productos (inicio)   | No      | -       |
| `/about`              | Nosotros                        | No      | -       |
| `/login`              | Inicio de sesion                | No      | -       |
| `/register`           | Registro de cliente             | No      | -       |
| `/carrito`            | Carrito de compras              | Si      | Cualquiera |
| `/checkout`           | Pago y confirmacion             | Si      | Cualquiera |
| `/mis-compras`        | Historial de compras            | Si      | Cualquiera |
| `/mi-perfil`          | Editar perfil                   | Si      | Cualquiera |
| `/admin/dashboard`    | Panel de control                | Si      | admin   |
| `/admin/productos`    | CRUD productos                  | Si      | admin   |
| `/admin/usuarios`     | Gestion de usuarios             | Si      | admin   |
| `/admin/ventas`       | Historial de ventas             | Si      | admin   |
| `/admin/reportes`     | Reportes y estadisticas         | Si      | admin   |

---

## Stack tecnologico

### Frontend
- **Vue 3.5** (Composition API + `<script setup>`)
- **TypeScript 6**
- **Vite 8** (con proxy a backend)
- **Vue Router 4**
- CSS puro con gradientes y diseno responsive
- Font Awesome 6 (iconos)
- Poppins (Google Fonts)

### Backend
- **ASP.NET Core 10.0** (Web API)
- **Entity Framework Core 10.0** (ORM)
- **SQL Server** (base de datos)
- **JWT Bearer** (autenticacion)
- **BCrypt.Net** (hashing de contrasenas)

---

## API Endpoints

### Auth
| Metodo | Ruta                  | Descripcion            | Auth  |
|--------|-----------------------|------------------------|-------|
| POST   | `/api/auth/register`  | Registro de cliente    | No    |
| POST   | `/api/auth/login`     | Inicio de sesion       | No    |
| GET    | `/api/auth/me`        | Usuario actual         | Si    |
| PUT    | `/api/auth/perfil`    | Actualizar perfil      | Si    |
| PUT    | `/api/auth/password`  | Cambiar contrasena     | Si    |

### Productos
| Metodo | Ruta                          | Descripcion               | Auth  |
|--------|-------------------------------|---------------------------|-------|
| GET    | `/api/productos`              | Productos activos         | No    |
| GET    | `/api/productos/all`          | Todos los productos       | Admin |
| GET    | `/api/productos/{id}`         | Producto por ID           | No    |
| GET    | `/api/productos/buscar?q=`    | Buscar por nombre         | No    |
| POST   | `/api/productos`              | Crear producto            | Admin |
| PUT    | `/api/productos/{id}`         | Actualizar producto       | Admin |
| DELETE | `/api/productos/{id}`         | Desactivar producto       | Admin |

### Categorias
| Metodo | Ruta                  | Descripcion            | Auth  |
|--------|-----------------------|------------------------|-------|
| GET    | `/api/categorias`     | Listar categorias      | No    |
| POST   | `/api/categorias`     | Crear categoria        | Admin |
| PUT    | `/api/categorias/{id}`| Actualizar categoria   | Admin |
| DELETE | `/api/categorias/{id}`| Eliminar categoria     | Admin |

### Usuarios (admin)
| Metodo | Ruta                  | Descripcion            | Auth  |
|--------|-----------------------|------------------------|-------|
| GET    | `/api/usuarios`       | Listar usuarios        | Admin |
| POST   | `/api/usuarios`       | Crear usuario          | Admin |
| PUT    | `/api/usuarios/{id}`  | Actualizar usuario     | Admin |
| DELETE | `/api/usuarios/{id}`  | Desactivar usuario     | Admin |

### Ventas
| Metodo | Ruta                      | Descripcion                   | Auth  |
|--------|---------------------------|-------------------------------|-------|
| POST   | `/api/ventas`             | Crear venta (checkout)        | Si    |
| GET    | `/api/ventas`             | Todas las ventas              | Admin |
| GET    | `/api/ventas/mis-compras` | Compras del usuario actual    | Si    |
| GET    | `/api/ventas/{id}`        | Detalle de venta              | Si    |
| DELETE | `/api/ventas/{id}`        | Eliminar venta + restaurar stock | Admin |

---

## Como levantar el proyecto (paso a paso)

### Requisitos previos
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 22+](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) (local o Docker)

### Paso 1: Crear la base de datos

Abre **SQL Server Management Studio** (o tu herramienta favorita) y ejecuta el archivo:

```
BackendApi/script.sql
```

Esto creara la base de datos `HeladeriaDb` con las tablas y datos iniciales (categorias y productos).

### Paso 2: Configurar la conexion

Edita `BackendApi/appsettings.json` y ajusta la cadena de conexion si es necesario:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HeladeriaDb;Trusted_Connection=True;..."
  }
}
```

> Si usas autenticacion SQL Server, cambia `Trusted_Connection=True` por `User Id=sa;Password=tu_password;`.

### Paso 3: Ejecutar el backend

```sh
cd BackendApi
dotnet restore
dotnet run
```

El backend estara disponible en:
- **API**: `http://localhost:5057`

> Al iniciar por primera vez, el seeder creara el usuario admin automaticamente.

### Paso 4: Ejecutar el frontend

En otra terminal:

```sh
# Instalar dependencias (solo la primera vez)
npm install

# Ejecutar en desarrollo
npm run dev
```

El frontend estara disponible en: `http://localhost:5173`

> El frontend esta configurado con un proxy en `vite.config.ts` que redirige las llamadas `/api/*` al backend en el puerto 5057.

### Paso 5: Usar la aplicacion

1. Abre `http://localhost:5173` en tu navegador
2. Explora los productos en la pagina de inicio
3. Crea una cuenta o inicia sesion con `admin@heladeria.com` / `admin123`
4. Si inicias sesion como admin, seras redirigido al panel de administracion
5. Si te registras como cliente, podras agregar productos al carrito y comprar

---

## Notas

- Las contrasenas se almacenan hasheadas con BCrypt (nunca en texto plano).
- Los tokens JWT expiran en 8 horas por defecto (configurable en `appsettings.json`).
- El carrito se persiste en `localStorage` del navegador.
- Las ventas en la base de datos incluyen historial completo y detalle de productos.
- Las notificaciones son flotantes (toast) y no desplazan elementos de la UI.
- El stock se actualiza automaticamente al realizar una venta y se restaura al eliminarla.

---

*Proyecto realizado por el grupo 404*
