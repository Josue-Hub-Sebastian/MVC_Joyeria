using System.ComponentModel.DataAnnotations;

namespace mvc_purple.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Direccion { get; set; }

        public bool EsAdmin { get; set; } = false;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public List<Pedido>? Pedidos { get; set; }
    }
}
