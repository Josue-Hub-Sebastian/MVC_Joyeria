namespace mvc_purple.DTO.Request
{
    public class PedidoRequest
    {
        public int ClienteId { get; set; }
        public List<int> ProductosIds { get; set; } = new();
        public List<int> Cantidades { get; set; } = new();
        public string DireccionEnvio { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}