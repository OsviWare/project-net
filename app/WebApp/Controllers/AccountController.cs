using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Authentication;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "/")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _authService.Authenticate(model.Email!, model.Password!);
                    
                    if (user != null && user.Activo)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Nombre!),
                            new Claim(ClaimTypes.Email, user.Email!),
                            new Claim(ClaimTypes.Role, user.Rol?.Nombre ?? "Usuario"),
                            new Claim("UserId", user.Id.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        _logger.LogInformation($"Usuario {user.Email} ha iniciado sesión.");
                        
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    
                    ModelState.AddModelError("", "Credenciales inválidas o usuario inactivo");
                    _logger.LogWarning($"Intento de login fallido para: {model.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante el login");
                    ModelState.AddModelError("", "Error durante la autenticación");
                }
            }
            
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Usuario cerró sesión.");
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Mapear desde RegisterViewModel a Usuario
                    var user = new Usuario
                    {
                        Nombre = model.FullName,
                        Email = model.Email,
                        Telefono = model.PhoneNumber,
                        Direccion = model.Direccion,
                        Ciudad = model.City,
                        RolId = 2, // usuario_sistema por defecto
                        Activo = true
                    };

                    var result = await _authService.Register(user, model.Password!);
                    
                    if (result != null)
                    {
                        _logger.LogInformation($"Nuevo usuario registrado: {model.Email}");
                        
                        // Iniciar sesión automáticamente después del registro
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, result.Nombre!),
                            new Claim(ClaimTypes.Email, result.Email!),
                            new Claim(ClaimTypes.Role, result.Rol?.Nombre ?? "Usuario"),
                            new Claim("UserId", result.Id.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error durante el registro");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante el registro");
                    
                    if (ex.Message.Contains("email ya está registrado"))
                    {
                        ModelState.AddModelError("", "El email ya está registrado");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error durante el registro");
                    }
                }
            }
            return View(model);
        }
    }
}