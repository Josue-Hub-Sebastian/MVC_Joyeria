using System.ComponentModel.DataAnnotations;

namespace mvc_purple.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
        public decimal Total { get; set; }
        public string? DireccionEnvio { get; set; }
        public string? Observaciones { get; set; }

        public Cliente? Cliente { get; set; }
        public List<DetallePedido> Detalles { get; set; } = new();
        public void CalcularTotal() => Total = Detalles.Sum(d => d.Subtotal);
    }
}
