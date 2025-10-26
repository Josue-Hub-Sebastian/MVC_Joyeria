using mvc_purple.Models;

namespace mvc_purple.DTO.Response
{
    public class DetallePedidoResponse
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public Producto? Producto { get; set; }
    }
}