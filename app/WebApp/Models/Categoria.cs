using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("categorias")]
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}