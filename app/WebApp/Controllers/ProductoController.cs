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
        private readonly IWebHostEnvironment _env;

        public ProductoController(ApplicationDbContext db, ILogger<ProductoController> logger, IWebHostEnvironment env)
        {
            _db = db;
            _logger = logger;
            _env = env;
        }

        // Mostrar todos los productos activos
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? categoriaId)
        {
            var query = _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Usuario)
                .Where(p => p.Activo)
                .AsQueryable();

            if (categoriaId.HasValue)
            {
                query = query.Where(p => p.CategoriaId == categoriaId.Value);
                ViewBag.SelectedCategoriaId = categoriaId.Value;
            }

            var productos = await query
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

                // Si se subió un archivo de imagen, guardarlo y asignar la ruta
                if (model.ImagenFile != null && model.ImagenFile.Length > 0)
                {
                    var relativePath = await SaveImageFile(model.ImagenFile);
                    if (!string.IsNullOrEmpty(relativePath)) producto.ImagenUrl = relativePath;
                }

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
            if (User.IsInRole("administrador")) return true;
            var uid = GetCurrentUserId();
            return uid != null && producto.UsuarioId == uid.Value;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

            if (producto == null) return NotFound();
            return View(producto);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            if (!CanEdit(producto)) return Forbid();

            ViewBag.Categorias = await _db.Categorias.ToListAsync();
            var model = new WebApp.Models.ProductoCreateViewModel
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                ImagenUrl = producto.ImagenUrl,
                CategoriaId = producto.CategoriaId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, WebApp.Models.ProductoCreateViewModel model)
        {
            if (id != model.Id) return BadRequest();

            var producto = await _db.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            if (!CanEdit(producto)) return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = await _db.Categorias.ToListAsync();
                return View(model);
            }

            producto.Nombre = model.Nombre;
            producto.Descripcion = model.Descripcion;
            producto.Precio = model.Precio;
            producto.Stock = model.Stock;
            producto.CategoriaId = model.CategoriaId;

            // Si se sube una nueva imagen durante la edición, guardarla y actualizar ImagenUrl
            if (model.ImagenFile != null && model.ImagenFile.Length > 0)
            {
                var relativePath = await SaveImageFile(model.ImagenFile);
                if (!string.IsNullOrEmpty(relativePath)) producto.ImagenUrl = relativePath;
            }
            else
            {
                // mantener ImagenUrl existente o la proporcionada en el ViewModel
                producto.ImagenUrl = model.ImagenUrl;
            }

            try
            {
                _db.Productos.Update(producto);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Mine));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar producto {Id}", id);
                ModelState.AddModelError("", "Ocurrió un error al actualizar el producto");
                ViewBag.Categorias = await _db.Categorias.ToListAsync();
                return View(model);
            }
        }

        // Guarda el archivo en wwwroot/uploads y devuelve la ruta relativa (p.ej. /uploads/archivo.jpg)
        private async Task<string?> SaveImageFile(IFormFile file)
        {
            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueName);

                using var stream = System.IO.File.Create(filePath);
                await file.CopyToAsync(stream);

                // devolver ruta relativa usable en las vistas
                return "/uploads/" + uniqueName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar archivo de imagen");
                return null;
            }
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();
            if (!CanEdit(producto)) return Forbid();

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            if (!CanEdit(producto)) return Forbid();

            // Soft delete
            producto.Activo = false;
            _db.Productos.Update(producto);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Mine));
        }
    }
}
