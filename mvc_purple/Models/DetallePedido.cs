using System.ComponentModel.DataAnnotations;

namespace mvc_purple.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PrecioUnitario { get; set; }

        // Propiedad calculada
        public decimal Subtotal => Cantidad * PrecioUnitario;

        // Relaciones
        public Pedido? Pedido { get; set; }
        public Producto? Producto { get; set; }
    }
}
