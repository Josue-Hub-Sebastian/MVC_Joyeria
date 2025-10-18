using System.ComponentModel.DataAnnotations;

namespace mvc_purple.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        public DateTime FechaPedido { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Total { get; set; }

        [StringLength(200)]
        public string? DireccionEnvio { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        // Relaciones
        public Cliente? Cliente { get; set; }
        public List<DetallePedido> Detalles { get; set; } = new();

        // Método para calcular el total
        public void CalcularTotal()
        {
            Total = Detalles.Sum(d => d.Subtotal);
        }
    }
}
