using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize] // Requiere autenticación para todo el controlador
    public class DashboardController : Controller
    {
        // Ruta segura 1: Dashboard principal - Accesible para todos los roles autenticados
        public IActionResult Index()
        {
            return View();
        }

        // Ruta segura 2: Panel de administración - Solo para administradores
        [Authorize(Roles = "Administrador")]
        public IActionResult AdminPanel()
        {
            return View();
        }

        // Ruta segura 3: Mis productos - Solo para usuarios que pueden vender
        [Authorize(Roles = "Administrador,Usuario")]
        public IActionResult MyProducts()
        {
            return View();
        }

        // Ruta segura 4: Perfil de usuario - Accesible para todos los autenticados
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize(Roles = "Usuario,Administrador")]
        public IActionResult UserArea()
        {
            return Content("¡Área para usuarios registrados!");
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult AdminOnly()
        {
            return Content("¡Esta es una página solo para administradores!");
        }
    }
}