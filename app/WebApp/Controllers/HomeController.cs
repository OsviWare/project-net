using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Pasar información del usuario a la vista
            ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
            ViewBag.UserName = User.Identity?.Name;
            ViewBag.UserRole = User.IsInRole("Administrador") ? "Administrador" : "Usuario";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        [AllowAnonymous]
        public IActionResult PublicPage()
        {
            return Content("¡Esta página es pública para todos!");
        }
    }
}