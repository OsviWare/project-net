using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // CAMBIA "Administrador" por "administrador"
        // [Authorize(Roles = "administrador")]
        public IActionResult AdminPanel()
        {
            return View();
        }

        // También corrige los otros roles
        [Authorize(Roles = "administrador,usuario_sistema")]
        public IActionResult MyProducts()
        {
            return View();
        }

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