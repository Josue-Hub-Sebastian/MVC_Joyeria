using System.ComponentModel.DataAnnotations;

namespace mvc_purple.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal => Cantidad * PrecioUnitario;

        public Producto? Producto { get; set; }
        public Pedido? Pedido { get; set; }
    }
}
