namespace mvc_purple.Models.DTOs
{
    public class PedidoRequest
    {
        public int ClienteId { get; set; }
        public List<int> ProductosIds { get; set; } = new();
        public List<int> Cantidades { get; set; } = new();
        public string? DireccionEnvio { get; set; }
        public string? Observaciones { get; set; }
    }

}
