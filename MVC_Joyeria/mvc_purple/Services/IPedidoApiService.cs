using mvc_purple.Models;
using mvc_purple.Models.DTOs;

namespace mvc_purple.Services
{
    public interface IPedidoApiService
    {
        Task<IEnumerable<Pedido>> GetAllAsync();
       // Task<Pedido?> GetByIdAsync(int id);
        Task<PedidoResponse?> GetByIdAsync(int id);

        Task<Pedido?> CreateAsync(PedidoRequest request);
        Task<bool> CambiarEstadoAsync(int id, string nuevoEstado);
    }
}
