using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Data;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering; // ← AÑADE ESTE USING

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
            var categorias = await _db.Categorias.ToListAsync();
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre"); // ← CAMBIA ESTA LÍNEA
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
                var categorias = await _db.Categorias.ToListAsync();
                ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre"); // ← CAMBIA ESTA LÍNEA
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


        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null || !producto.Activo)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Producto/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _db.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            // Verificar que el usuario puede editar este producto
            if (!CanEdit(producto))
            {
                return Forbid();
            }

            var categorias = await _db.Categorias.ToListAsync();
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre"); // ← CAMBIA ESTA LÍNEA
            
            // Mapear a ViewModel para edición
            var model = new ProductoCreateViewModel
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


        // POST: Producto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, ProductoCreateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var categorias = await _db.Categorias.ToListAsync();
                ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre"); // ← CAMBIA ESTA LÍNEA
                return View(model);
            }

            try
            {
                var producto = await _db.Productos.FindAsync(id);
                if (producto == null)
                {
                    return NotFound();
                }

                // Verificar que el usuario puede editar este producto
                if (!CanEdit(producto))
                {
                    return Forbid();
                }

                // Actualizar propiedades
                producto.Nombre = model.Nombre;
                producto.Descripcion = model.Descripcion;
                producto.Precio = model.Precio;
                producto.Stock = model.Stock;
                producto.ImagenUrl = model.ImagenUrl;
                producto.CategoriaId = model.CategoriaId;

                _db.Update(producto);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Producto actualizado: {@Producto}", producto);
                return RedirectToAction(nameof(Mine));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Producto/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _db.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            // Verificar que el usuario puede eliminar este producto
            if (!CanEdit(producto))
            {
                return Forbid();
            }

            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            // Verificar que el usuario puede eliminar este producto
            if (!CanEdit(producto))
            {
                return Forbid();
            }

            // Soft delete (marcar como inactivo) en lugar de eliminar físicamente
            producto.Activo = false;
            _db.Update(producto);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Producto eliminado (soft delete): {ProductoId}", id);
            return RedirectToAction(nameof(Mine));
        }

        private bool ProductoExists(int id)
        {
            return _db.Productos.Any(e => e.Id == id);
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
        public async Task<IActionResult> PorCategoria(int id)
        {
            var productos = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Usuario)
                .Where(p => p.Activo && p.CategoriaId == id)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();

            ViewBag.CategoriaSeleccionada = await _db.Categorias
                .Where(c => c.Id == id)
                .Select(c => c.Nombre)
                .FirstOrDefaultAsync();

            return View("Index", productos); // Reutiliza la vista Index.cshtml
        }


    }
}
