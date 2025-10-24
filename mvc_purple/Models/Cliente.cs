using System.ComponentModel.DataAnnotations;

namespace mvc_purple.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        public bool EsAdmin { get; set; } = false;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relación con pedidos
        public List<Pedido> Pedidos { get; set; } = new();
    }
}
