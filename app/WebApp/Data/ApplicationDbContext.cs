using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Producto> Productos { get; set; } // ← AÑADIR ESTA LÍNEA
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<DetalleOrden> DetalleOrden { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar nombres de tablas exactos
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Rol>().ToTable("roles");
            modelBuilder.Entity<Producto>().ToTable("productos");
            modelBuilder.Entity<Categoria>().ToTable("categorias");
            modelBuilder.Entity<Orden>().ToTable("ordenes");
            modelBuilder.Entity<DetalleOrden>().ToTable("detalle_orden");

            // Configurar índice único para email
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Datos iniciales para roles
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "administrador", Descripcion = "Administrador completo del sistema" },
                new Rol { Id = 2, Nombre = "usuario_sistema", Descripcion = "Usuario que puede vender y comprar" },
                new Rol { Id = 3, Nombre = "externo", Descripcion = "Usuario que solo puede comprar" }
            );

            // Datos iniciales para categorías
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Ropa y Accesorios", Descripcion = "Prendas de vestir y complementos" },
                new Categoria { Id = 2, Nombre = "Tecnología", Descripcion = "Dispositivos electrónicos y tecnología" },
                new Categoria { Id = 3, Nombre = "Hogar y Muebles", Descripcion = "Artículos para el hogar y muebles" },
                new Categoria { Id = 4, Nombre = "Deportes", Descripcion = "Equipamiento y ropa deportiva" },
                new Categoria { Id = 5, Nombre = "Libros", Descripcion = "Libros de todo tipo" }
            );

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("productos");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Precio).HasColumnName("precio");
                entity.Property(e => e.Stock).HasColumnName("stock");
                entity.Property(e => e.ImagenUrl).HasColumnName("imagen_url");
                entity.Property(e => e.CategoriaId).HasColumnName("CategoriaId");
                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioId");
                entity.Property(e => e.FechaCreacion).HasColumnName("FechaCreacion");
                entity.Property(e => e.Activo).HasColumnName("activo");
            });
                }
    }
}