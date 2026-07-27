using Microsoft.EntityFrameworkCore;
using BackendApi.Models;

namespace BackendApi.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.EnsureCreatedAsync();

            if (await context.Usuarios.AnyAsync())
                return;

            var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");

            var admin = new Usuario
            {
                Nombre = "Administrador",
                Email = "admin@heladeria.com",
                PasswordHash = adminPasswordHash,
                Rol = "admin",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            context.Usuarios.Add(admin);
            await context.SaveChangesAsync();

            var categorias = new List<Categoria>
            {
                new() { Nombre = "Helados", Descripcion = "Helados artesanales en cono, pocillo y tubo" },
                new() { Nombre = "Paletas", Descripcion = "Paletas de fruta y crema" },
                new() { Nombre = "Postres", Descripcion = "Postres helados especiales" },
                new() { Nombre = "Bebidas", Descripcion = "Batidos, malteadas y refrescos" },
                new() { Nombre = "Acompanamientos", Descripcion = "Conos, toppings y adicionales" }
            };

            context.Categorias.AddRange(categorias);
            await context.SaveChangesAsync();

            var productos = new List<Producto>
            {
                new() { Nombre = "Helado de Vainilla", Descripcion = "Helado cremoso de vainilla natural", Precio = 2.50m, Stock = 100, IdCategoria = categorias[0].Id, Activo = true },
                new() { Nombre = "Helado de Chocolate", Descripcion = "Helado intenso de cacao premium", Precio = 2.50m, Stock = 100, IdCategoria = categorias[0].Id, Activo = true },
                new() { Nombre = "Helado de Fresa", Descripcion = "Helado de fresa fresca", Precio = 2.50m, Stock = 80, IdCategoria = categorias[0].Id, Activo = true },
                new() { Nombre = "Helado de Menta", Descripcion = "Helado refrescante de menta con chispas de chocolate", Precio = 3.00m, Stock = 60, IdCategoria = categorias[0].Id, Activo = true },
                new() { Nombre = "Helado de Mango", Descripcion = "Helado tropical de mango", Precio = 2.75m, Stock = 70, IdCategoria = categorias[0].Id, Activo = true },
                new() { Nombre = "Paleta de Limon", Descripcion = "Paleta natural de limon", Precio = 1.50m, Stock = 120, IdCategoria = categorias[1].Id, Activo = true },
                new() { Nombre = "Paleta de Sandia", Descripcion = "Paleta refrescante de sandia", Precio = 1.50m, Stock = 90, IdCategoria = categorias[1].Id, Activo = true },
                new() { Nombre = "Paleta de Mora", Descripcion = "Paleta de mora artesanal", Precio = 1.75m, Stock = 85, IdCategoria = categorias[1].Id, Activo = true },
                new() { Nombre = "Sundae de Chocolate", Descripcion = "Sundae con salsa de chocolate y nata", Precio = 4.50m, Stock = 50, IdCategoria = categorias[2].Id, Activo = true },
                new() { Nombre = "Banana Split", Descripcion = "Banana split clasico con tres sabores", Precio = 5.00m, Stock = 40, IdCategoria = categorias[2].Id, Activo = true },
                new() { Nombre = "Batido de Fresa", Descripcion = "Batido cremoso de fresa", Precio = 3.50m, Stock = 60, IdCategoria = categorias[3].Id, Activo = true },
                new() { Nombre = "Malteada de Vainilla", Descripcion = "Malteada de vainilla con crema batida", Precio = 4.00m, Stock = 55, IdCategoria = categorias[3].Id, Activo = true },
                new() { Nombre = "Cono Clasico", Descripcion = "Cono crujiente para helado", Precio = 0.50m, Stock = 200, IdCategoria = categorias[4].Id, Activo = true },
                new() { Nombre = "Sprinkles de Colores", Descripcion = "Grageas de colores para decorar", Precio = 0.30m, Stock = 300, IdCategoria = categorias[4].Id, Activo = true },
                new() { Nombre = "Salsa de Chocolate", Descripcion = "Salsa de chocolate para topping", Precio = 0.75m, Stock = 150, IdCategoria = categorias[4].Id, Activo = true }
            };

            context.Productos.AddRange(productos);
            await context.SaveChangesAsync();
        }
    }
}
