using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using BCrypt.Net;

namespace WebApp.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedData(ApplicationDbContext context)
        {
            // Crear usuario administrador por defecto si no existe
            if (!await context.Usuarios.AnyAsync())
            {
                var adminUser = new Usuario
                {
                    Nombre = "Administrador",
                    Apellido = "Sistema",
                    Email = "admin@marketplace.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Telefono = "+1234567890",
                    Direccion = "123 Calle Principal",
                    Ciudad = "Ciudad Principal",
                    RolId = 1, // Administrador
                    FechaRegistro = DateTime.UtcNow,
                    Activo = true
                };

                context.Usuarios.Add(adminUser);
                await context.SaveChangesAsync();
            }

            // Crear productos de muestra si no existen
            if (!await context.Productos.AnyAsync())
            {
                var adminUser = await context.Usuarios.FirstAsync(u => u.RolId == 1);

                var productos = new List<Producto>
                {
                    new Producto
                    {
                        Nombre = "iPhone 15 Pro Max",
                        Descripcion = "El último iPhone de Apple con tecnología de vanguardia",
                        Precio = 1299.99m,
                        Stock = 50,
                        ImagenUrl = "https://via.placeholder.com/300x200?text=iPhone+15",
                        CategoriaId = 2, // Tecnología
                        UsuarioId = adminUser.Id,
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    },
                    new Producto
                    {
                        Nombre = "Camiseta Nike Dri-FIT",
                        Descripcion = "Camiseta deportiva de alta calidad para entrenar",
                        Precio = 29.99m,
                        Stock = 100,
                        ImagenUrl = "https://via.placeholder.com/300x200?text=Nike+Shirt",
                        CategoriaId = 4, // Deportes
                        UsuarioId = adminUser.Id,
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    },
                    new Producto
                    {
                        Nombre = "MacBook Pro M3",
                        Descripcion = "Laptop profesional con chip M3 de Apple",
                        Precio = 1999.99m,
                        Stock = 25,
                        ImagenUrl = "https://via.placeholder.com/300x200?text=MacBook+Pro",
                        CategoriaId = 2, // Tecnología
                        UsuarioId = adminUser.Id,
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    },
                    new Producto
                    {
                        Nombre = "Sofá Moderno 3 Plazas",
                        Descripcion = "Sofá cómodo y elegante para sala de estar",
                        Precio = 799.99m,
                        Stock = 10,
                        ImagenUrl = "https://via.placeholder.com/300x200?text=Sofa+Moderno",
                        CategoriaId = 3, // Hogar y Muebles
                        UsuarioId = adminUser.Id,
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    },
                    new Producto
                    {
                        Nombre = "El Quijote - Edición Especial",
                        Descripcion = "Clásico de la literatura española en edición de lujo",
                        Precio = 39.99m,
                        Stock = 75,
                        ImagenUrl = "https://via.placeholder.com/300x200?text=El+Quijote",
                        CategoriaId = 5, // Libros
                        UsuarioId = adminUser.Id,
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    },
                    new Producto
                    {
                        Nombre = "Chaqueta de Cuero",
                        Descripcion = "Chaqueta de cuero genuino, estilo clásico",
                        Precio = 199.99m,
                        Stock = 30,
                        ImagenUrl = "https://via.placeholder.com/300x200?text=Leather+Jacket",
                        CategoriaId = 1, // Ropa y Accesorios
                        UsuarioId = adminUser.Id,
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    }
                };

                context.Productos.AddRange(productos);
                await context.SaveChangesAsync();
            }
        }
    }
}