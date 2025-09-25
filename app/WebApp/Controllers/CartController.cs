using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class CartController : Controller
    {
        private const string SessionKey = "Cart";
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<CartViewModel>(SessionKey) ?? new CartViewModel();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productoId, int cantidad = 1)
        {
            var producto = await _db.Productos.FindAsync(productoId);
            if (producto == null || !producto.Activo) return NotFound();
            var cart = HttpContext.Session.GetObject<CartViewModel>(SessionKey) ?? new CartViewModel();
            var existing = cart.Items.FirstOrDefault(i => i.ProductoId == productoId);
            var existingQty = existing?.Cantidad ?? 0;

            // Verificar stock disponible (considerando lo que ya hay en el carrito)
            if (existingQty + cantidad > producto.Stock)
            {
                TempData["CartError"] = $"No hay suficiente stock para '{producto.Nombre}'. Stock disponible: {producto.Stock - existingQty}.";
                // Intentar volver a la página donde vino la petición
                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer)) return Redirect(referer);
                return RedirectToAction("Index", "Cart");
            }

            if (existing != null)
            {
                existing.Cantidad += cantidad;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad,
                    ImagenUrl = producto.ImagenUrl
                });
            }

            HttpContext.Session.SetObject(SessionKey, cart);
            TempData["CartSuccess"] = $"'{producto.Nombre}' agregado al carrito.";
            var refer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(refer)) return Redirect(refer);
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int productoId)
        {
            var cart = HttpContext.Session.GetObject<CartViewModel>(SessionKey) ?? new CartViewModel();
            cart.Items.RemoveAll(i => i.ProductoId == productoId);
            HttpContext.Session.SetObject(SessionKey, cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productoId, int cantidad)
        {
            var producto = await _db.Productos.FindAsync(productoId);
            if (producto == null) return NotFound();

            var cart = HttpContext.Session.GetObject<CartViewModel>(SessionKey) ?? new CartViewModel();
            var item = cart.Items.FirstOrDefault(i => i.ProductoId == productoId);

            // Si cantidad menor a 1, eliminar
            if (cantidad < 1)
            {
                if (item != null) cart.Items.RemoveAll(i => i.ProductoId == productoId);
                HttpContext.Session.SetObject(SessionKey, cart);
                TempData["CartSuccess"] = $"Se removió '{producto.Nombre}' del carrito.";
                return RedirectToAction("Index");
            }

            // Verificar stock
            if (cantidad > producto.Stock)
            {
                TempData["CartError"] = $"No hay suficiente stock para '{producto.Nombre}'. Stock disponible: {producto.Stock}.";
                return RedirectToAction("Index");
            }

            if (item != null)
            {
                item.Cantidad = cantidad;
            }
            else
            {
                // Si no existía, añadir
                cart.Items.Add(new Models.CartItem
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad,
                    ImagenUrl = producto.ImagenUrl
                });
            }

            HttpContext.Session.SetObject(SessionKey, cart);
            TempData["CartSuccess"] = $"Cantidad actualizada para '{producto.Nombre}'.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(SessionKey);
            return RedirectToAction("Index");
        }
    }
}
