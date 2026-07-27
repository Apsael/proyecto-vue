-- ============================================
-- Heladeria La Dolce Vita - Script SQL Server
-- ============================================
-- Base de datos: HeladeriaDb
-- Nota: El usuario admin se crea automaticamente
--       al iniciar la aplicacion backend.
-- ============================================

CREATE DATABASE HeladeriaDb;
GO

USE HeladeriaDb;
GO

-- ============================================
-- Tabla: Usuarios
-- ============================================
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Rol NVARCHAR(20) NOT NULL DEFAULT 'cliente',
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UQ_Usuarios_Email UNIQUE (Email)
);
GO

-- ============================================
-- Tabla: Categorias
-- ============================================
CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500) NULL
);
GO

-- ============================================
-- Tabla: Productos
-- ============================================
CREATE TABLE Productos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    Precio DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    IdCategoria INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (IdCategoria) REFERENCES Categorias(Id)
);
GO

-- ============================================
-- Tabla: Ventas
-- ============================================
CREATE TABLE Ventas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    MetodoPago NVARCHAR(50) NOT NULL DEFAULT 'efectivo',
    Observaciones NVARCHAR(500) NULL,
    FechaVenta DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Ventas_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(Id)
);
GO

-- ============================================
-- Tabla: DetalleVenta
-- ============================================
CREATE TABLE DetalleVenta (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_DetalleVenta_Ventas FOREIGN KEY (IdVenta) REFERENCES Ventas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_DetalleVenta_Productos FOREIGN KEY (IdProducto) REFERENCES Productos(Id)
);
GO

-- ============================================
-- Datos iniciales: Categorias
-- ============================================
INSERT INTO Categorias (Nombre, Descripcion) VALUES
(N'Helados', N'Helados artesanales en cono, pocillo y tubo'),
(N'Paletas', N'Paletas de fruta y crema'),
(N'Postres', N'Postres helados especiales'),
(N'Bebidas', N'Batidos, malteadas y refrescos'),
(N'Acompanamientos', N'Conos, toppings y adicionales');
GO

-- ============================================
-- Datos iniciales: Productos
-- ============================================
INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, IdCategoria, Activo) VALUES
(N'Helado de Vainilla', N'Helado cremoso de vainilla natural', 2.50, 100, 1, 1),
(N'Helado de Chocolate', N'Helado intenso de cacao premium', 2.50, 100, 1, 1),
(N'Helado de Fresa', N'Helado de fresa fresca', 2.50, 80, 1, 1),
(N'Helado de Menta', N'Helado refrescante de menta con chispas de chocolate', 3.00, 60, 1, 1),
(N'Helado de Mango', N'Helado tropical de mango', 2.75, 70, 1, 1),
(N'Paleta de Limon', N'Paleta natural de limon', 1.50, 120, 2, 1),
(N'Paleta de Sandia', N'Paleta refrescante de sandia', 1.50, 90, 2, 1),
(N'Paleta de Mora', N'Paleta de mora artesanal', 1.75, 85, 2, 1),
(N'Sundae de Chocolate', N'Sundae con salsa de chocolate y nata', 4.50, 50, 3, 1),
(N'Banana Split', N'Banana split clasico con tres sabores', 5.00, 40, 3, 1),
(N'Batido de Fresa', N'Batido cremoso de fresa', 3.50, 60, 4, 1),
(N'Malteada de Vainilla', N'Malteada de vainilla con crema batida', 4.00, 55, 4, 1),
(N'Cono Clasico', N'Cono crujiente para helado', 0.50, 200, 5, 1),
(N'Sprinkles de Colores', N'Grageas de colores para decorar', 0.30, 300, 5, 1),
(N'Salsa de Chocolate', N'Salsa de chocolate para topping', 0.75, 150, 5, 1);
GO

PRINT 'Base de datos HeladeriaDb creada exitosamente.';
PRINT 'Categorias y productos iniciales insertados.';
PRINT 'El usuario admin sera creado automaticamente al iniciar el backend.';
GO
