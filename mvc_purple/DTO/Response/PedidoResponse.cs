namespace mvc_purple.DTO.Response
{
    public class PedidoResponse
    {
        public int Id { get; set; }

        public ClienteResponse Cliente { get; set; } = new ClienteResponse();

        public decimal Total { get; set; }

        public DateTime FechaPedido { get; set; }

        public string Estado { get; set; } = "";
        public string? DireccionEnvio { get; set; }
        public string? Observaciones { get; set; }

        public List<DetallePedidoResponse> Detalles { get; set; } = new();
    }
}