using Microsoft.EntityFrameworkCore;
using BackendApi.Models;

namespace BackendApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(e =>
            {
                e.ToTable("Usuarios");
                e.HasKey(u => u.Id);
                e.Property(u => u.Id).UseIdentityColumn();
                e.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                e.Property(u => u.Email).IsRequired().HasMaxLength(150);
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.PasswordHash).IsRequired();
                e.Property(u => u.Rol).IsRequired().HasMaxLength(20);
            });

            modelBuilder.Entity<Categoria>(e =>
            {
                e.ToTable("Categorias");
                e.HasKey(c => c.Id);
                e.Property(c => c.Id).UseIdentityColumn();
                e.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
                e.Property(c => c.Descripcion).HasMaxLength(500);
            });

            modelBuilder.Entity<Producto>(e =>
            {
                e.ToTable("Productos");
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).UseIdentityColumn();
                e.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
                e.Property(p => p.Descripcion).HasMaxLength(500);
                e.Property(p => p.Precio).HasColumnType("decimal(18,2)");
                e.HasOne(p => p.Categoria)
                    .WithMany()
                    .HasForeignKey(p => p.IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Venta>(e =>
            {
                e.ToTable("Ventas");
                e.HasKey(v => v.Id);
                e.Property(v => v.Id).UseIdentityColumn();
                e.Property(v => v.Total).HasColumnType("decimal(18,2)");
                e.Property(v => v.MetodoPago).IsRequired().HasMaxLength(50);
                e.Property(v => v.Observaciones).HasMaxLength(500);
                e.HasOne(v => v.Usuario)
                    .WithMany()
                    .HasForeignKey(v => v.IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DetalleVenta>(e =>
            {
                e.ToTable("DetalleVenta");
                e.HasKey(d => d.Id);
                e.Property(d => d.Id).UseIdentityColumn();
                e.Property(d => d.PrecioUnitario).HasColumnType("decimal(18,2)");
                e.Ignore(d => d.Subtotal);
                e.HasOne(d => d.Venta)
                    .WithMany(v => v.Detalles)
                    .HasForeignKey(d => d.IdVenta)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(d => d.Producto)
                    .WithMany()
                    .HasForeignKey(d => d.IdProducto)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
