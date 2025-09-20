using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class AdminController : Controller
{
    [Authorize(Roles = "Administrador")]
    public IActionResult AdminOnly()
    {
        return Content("¡Esta es una página solo para administradores!");
    }
    
    // Puedes agregar más acciones de administración aquí
    [Authorize(Roles = "Administrador")]
    public IActionResult Dashboard()
    {
        return View(); // O return Content("Dashboard de Admin");
    }
}