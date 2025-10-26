using System.ComponentModel.DataAnnotations;

namespace mvc_purple.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int Stock { get; set; }

        public bool Disponible { get; set; } = true;

        public string? ImagenUrl { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Esta propiedad puede ser útil si tu API incluye la relación inversa
        public List<DetallePedido>? DetallesPedido { get; set; }
    }
}
