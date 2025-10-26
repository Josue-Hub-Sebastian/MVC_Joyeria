namespace mvc_purple.DTO.Response
{
    public class ClienteResponse
    {
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Direccion { get; set; } = "";
        public DateTime FechaRegistro { get; set; }
    }
}