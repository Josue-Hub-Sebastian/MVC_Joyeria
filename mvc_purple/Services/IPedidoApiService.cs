using mvc_purple.Models;

namespace mvc_purple.Services
{
    public interface IPedidoApiService
    {
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<Pedido?> GetByIdAsync(int id);
        Task<Pedido?> CreateAsync(Pedido p);
        Task<bool> CambiarEstadoAsync(int id, string nuevoEstado);
    }
}
