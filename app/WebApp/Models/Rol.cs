using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("roles")]
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}