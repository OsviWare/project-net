using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("ordenes")]
    public class Orden
    {
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;

        public DateTime FechaOrden { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        public string Estado { get; set; } = "pendiente";

        public string? DireccionEnvio { get; set; }

        public virtual ICollection<DetalleOrden> Detalles { get; set; } = new List<DetalleOrden>();
    }
}