using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    public class ProductoController : Controller
    {
        // Acción pública para ver productos
        [AllowAnonymous]
        public IActionResult Index()
        {
            // Lógica para mostrar productos
            return View();
        }
        
        // Acción solo para administradores para crear productos
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }
        
        // Acción para usuarios registrados para comprar
        [Authorize(Roles = "Usuario,Administrador")]
        public IActionResult Comprar(int id)
        {
            // Lógica de compra
            return Content($"Producto {id} comprado");
        }
    }
}