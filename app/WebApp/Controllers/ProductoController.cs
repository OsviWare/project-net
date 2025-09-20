using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ProductoController> _logger;

        public ProductoController(ApplicationDbContext db, ILogger<ProductoController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Mostrar todos los productos activos
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var productos = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Usuario)
                .Where(p => p.Activo)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
            return View(productos);
        }

        // Mostrar productos del usuario autenticado
        [Authorize]
        public async Task<IActionResult> Mine()
        {
            var uid = GetCurrentUserId();
            if (uid == null) return Forbid();

            var productos = await _db.Productos
                .Include(p => p.Categoria)
                .Where(p => p.UsuarioId == uid.Value)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
            return View(productos);
        }

        // GET: Crear producto
        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categorias = await _db.Categorias.ToListAsync();
            return View();
        }

        // POST: Crear producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Producto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido: {@ModelState}", ModelState);
                ViewBag.Categorias = await _db.Categorias.ToListAsync();
                return View(model);
            }

            var uid = GetCurrentUserId();
            if (uid == null) return Forbid();

            model.UsuarioId = uid.Value;
            model.FechaCreacion = DateTime.UtcNow;
            model.Activo = true;

            try
            {
                _db.Productos.Add(model);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Producto registrado: {@Producto}", model);
                return RedirectToAction(nameof(Mine));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar producto {@Producto}", model);
                ModelState.AddModelError("", "Ocurrió un error al registrar el producto");
                ViewBag.Categorias = await _db.Categorias.ToListAsync();
                return View(model);
            }
        }

        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (int.TryParse(idClaim, out int uid)) return uid;

            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var user = _db.Usuarios.FirstOrDefault(u => u.Email == email);
                if (user != null) return user.Id;
            }
            return null;
        }

        private bool CanEdit(Producto producto)
        {
            if (User.IsInRole("Administrador")) return true;
            var uid = GetCurrentUserId();
            return uid != null && producto.UsuarioId == uid.Value;
        }
    }
}
