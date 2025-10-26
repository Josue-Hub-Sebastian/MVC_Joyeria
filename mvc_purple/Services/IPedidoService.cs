using mvc_purple.Models;

namespace mvc_purple.Services
{
    public interface IPedidoService
    {
        Task<List<Pedido>> GetAllAsync();
        Task<Pedido?> GetByIdAsync(int id);
        Task<bool> UpdateEstadoAsync(int pedidoId, string nuevoEstado);
    }
}
