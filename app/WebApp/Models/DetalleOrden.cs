using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("detalle_orden")]
    public class DetalleOrden
    {
        public int Id { get; set; }

        [ForeignKey("Orden")]
        public int OrdenId { get; set; }

        public virtual Orden Orden { get; set; } = null!;

        [ForeignKey("Producto")]
        public int ProductoId { get; set; }

        public virtual Producto Producto { get; set; } = null!;

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }
    }
}