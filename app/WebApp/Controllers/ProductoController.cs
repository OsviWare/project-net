using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ¡Falta este using!
using WebApp.Models;
using WebApp.Data; // Para el ApplicationDbContext

namespace WebApp.Controllers // Namespace corregido
{
    public class ProductoController : Controller
    {
        private readonly ILogger<ProductoController> _logger;
        private readonly ApplicationDbContext _context; // DbContext

        // Inyectar ambos servicios
        public ProductoController(
            ILogger<ProductoController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Método corregido
        public async Task<IActionResult> Index()
        {
            // Usar _context en lugar de _logger
            var productos = await _context.Productos.ToListAsync();
            return View(productos); // Pasar los productos a la vista
        }
    }
}