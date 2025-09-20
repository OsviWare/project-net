using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApp.Middlewares
{
    public class AccessLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AccessLoggingMiddleware> _logger;

        public AccessLoggingMiddleware(RequestDelegate next, ILogger<AccessLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Información de la solicitud
            var requestPath = context.Request.Path;
            var user = context.User.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "Anónimo";
            var role = context.User.Identity?.IsAuthenticated == true ? 
                string.Join(",", context.User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value)) : "Ninguno";

            // Ejecutar el siguiente middleware
            await _next(context);

            stopwatch.Stop();

            // Log del acceso
            _logger.LogInformation("Acceso: {Path} | Usuario: {User} | Rol: {Role} | Método: {Method} | Status: {StatusCode} | Tiempo: {Elapsed}ms", 
                requestPath, user, role, context.Request.Method, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            
            // Mensaje educativo en consola
            Console.WriteLine($"💡 ¿Sabías que cada acceso a '{requestPath}' fue procesado en {stopwatch.ElapsedMilliseconds}ms?");
        }
    }
}