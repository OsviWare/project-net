using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using WebApp.Models;
using WebApp.Data;
using System.Linq;

namespace WebApp.Controllers
{
    [Authorize] 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AllowAnonymous] // El home es público
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                ViewBag.IsAuthenticated = true;
                ViewBag.UserName = User.Identity?.Name ?? "Invitado";
                ViewBag.UserRole = User.IsInRole("administrador") ? "Administrador" : "Usuario";
            }
            else
            {
                ViewBag.IsAuthenticated = false;
                ViewBag.UserName = "Invitado";
                ViewBag.UserRole = "N/A";
            }

            // 🚀 Cargar productos desde MySQL
            var productos = _context.Productos
                .OrderByDescending(p => p.Id)
                .Take(10)
                .ToList();

            return View(productos);
        }

        [Authorize(Roles = "administrador")]
        public IActionResult AdminOnly()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult PublicPage()
        {
            return Content("¡Esta página es pública para todos!");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
