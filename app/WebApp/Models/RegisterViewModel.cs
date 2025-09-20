using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre completo")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Dirección")]
        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        public string? Direccion { get; set; }

        [Display(Name = "Ciudad")]
        [StringLength(50, ErrorMessage = "La ciudad no puede exceder 50 caracteres")]
        public string? City { get; set; }
    }
}