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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(ProductoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido: {@ModelState}", ModelState);
                ViewBag.Categorias = await _db.Categorias.ToListAsync();
                return View(model);
            }

            var uid = GetCurrentUserId();
            if (uid == null) return Forbid();

            try
            {
                // Mapear del ViewModel al Modelo Producto
                var producto = new Producto
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Precio = model.Precio,
                    Stock = model.Stock,
                    ImagenUrl = model.ImagenUrl,
                    CategoriaId = model.CategoriaId,
                    UsuarioId = uid.Value, // Asignar el usuario actual
                    FechaCreacion = DateTime.UtcNow,
                    Activo = true
                };

                _db.Productos.Add(producto);
                await _db.SaveChangesAsync();
                
                _logger.LogInformation("Producto registrado: {@Producto}", producto);
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
            // Primero busca el claim personalizado "usuarioId"
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;
            if (!string.IsNullOrEmpty(usuarioIdClaim) && int.TryParse(usuarioIdClaim, out int uid))
            {
                return uid;
            }

            // Luego intenta con los claims estándar
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(idClaim) && int.TryParse(idClaim, out int standardUid))
            {
                return standardUid;
            }

            // Luego con claim "sub"
            var subClaim = User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(subClaim) && int.TryParse(subClaim, out int subUid))
            {
                return subUid;
            }

            // Finalmente por email
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var user = _db.Usuarios.FirstOrDefault(u => u.Email == email);
                if (user != null) return user.Id;
            }

            _logger.LogWarning("No se pudo obtener el ID del usuario");
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
