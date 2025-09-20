using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        [Phone]
        public string? Telefono { get; set; }

        public string? Direccion { get; set; }
        
        public string? Ciudad { get; set; }

        // Propiedad RolId para la relación con Roles
        public int RolId { get; set; } = 2; // Valor por defecto: usuario_sistema

        // Propiedad de navegación
        public virtual Rol? Rol { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Activo { get; set; } = true;
    }
}