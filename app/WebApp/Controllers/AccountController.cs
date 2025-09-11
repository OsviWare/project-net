using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public IActionResult Login()
        {
            // Si el usuario ya está autenticado, redirigir al home
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            // Esto es un ejemplo básico - en una aplicación real deberías validar contra una base de datos
            if (ModelState.IsValid)
            {
                // Simulación de autenticación (reemplazar con lógica real)
                if (model.Email == "usuario@ejemplo.com" && model.Password == "Password123!")
                {
                    // Aquí iría la lógica real de autenticación
                    // Por ahora solo redirigimos al home
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                }
            }

            // Si llegamos hasta aquí, algo falló, volver a mostrar el formulario
            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica de registro de usuario
                // Por ahora solo redirigimos al login
                return RedirectToAction("Login", "Account");
            }

            // Si llegamos hasta aquí, algo falló, volver a mostrar el formulario
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Aquí iría la lógica de logout
            // Por ahora solo redirigimos al login
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}