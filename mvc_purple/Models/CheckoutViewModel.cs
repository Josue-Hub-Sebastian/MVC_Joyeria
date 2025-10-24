namespace mvc_purple.Models
{
    public class CheckoutViewModel
    {
        public List<ItemCarrito> ItemsCarrito { get; set; } = new List<ItemCarrito>();
        public decimal Total { get; set; }
        public string DireccionEnvio { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}
