using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using BCrypt.Net;

namespace WebApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> Authenticate(string email, string password)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email && u.Activo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
                return null;

            return usuario;
        }

        public async Task<Usuario?> Register(Usuario usuario, string password)
        {
            try
            {
                if (await EmailExists(usuario.Email!))
                    throw new Exception("El email ya está registrado");

                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                usuario.FechaRegistro = DateTime.UtcNow;
                usuario.Activo = true;
                
                // Asegurar RolId para usuario normal
                if (usuario.RolId == 0)
                {
                    usuario.RolId = 2; // usuario_sistema
                }

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Cargar la relación Rol
                await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();

                return usuario;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.InnerException?.Message}");
                throw new Exception("Error de base de datos al registrar usuario");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw;
            }
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }
    }
}