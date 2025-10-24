namespace mvc_purple.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalPedidos { get; set; }
        public int PedidosPendientes { get; set; }
        public int TotalProductos { get; set; }
        public int ProductosBajoStock { get; set; }
        public decimal VentasDelMes { get; set; }
        public List<Pedido> UltimosPedidos { get; set; } = new List<Pedido>();
    }
}
