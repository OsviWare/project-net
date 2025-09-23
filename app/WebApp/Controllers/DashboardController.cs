using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApp.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    [Authorize] // Requiere autenticación para todo el controlador
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Ruta segura 1: Dashboard principal - Accesible para todos los roles autenticados
        public IActionResult Index()
        {
            return View();
        }

        // Ruta segura 2: Panel de administración - Solo para administradores
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> AdminPanel()
        {
            // Cargar estadísticas para el panel admin
            ViewBag.TotalUsers = await _context.Usuarios.CountAsync();
            ViewBag.TotalProducts = await _context.Productos.CountAsync();
            ViewBag.ActiveProducts = await _context.Productos.CountAsync(p => p.Activo);
            ViewBag.TotalCategories = await _context.Categorias.CountAsync();
            
            return View();
        }

        // Ruta segura 3: Mis productos - Solo para usuarios que pueden vender
        [Authorize(Roles = "administrador,usuario_sistema")]
        public IActionResult MyProducts()
        {
            return View();
        }

        // Ruta segura 4: Perfil de usuario - Accesible para todos los autenticados
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize(Roles = "usuario_sistema,administrador")]
        public IActionResult UserArea()
        {
            return Content("¡Área para usuarios registrados!");
        }

        [Authorize(Roles = "administrador")]
        public IActionResult AdminOnly()
        {
            return Content("¡Esta es una página solo para administradores!");
        }
    }
}