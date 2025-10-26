using mvc_purple.DTO.Request;
using mvc_purple.DTO.Response;
using mvc_purple.Models;

namespace mvc_purple.api.IServices
{
    public interface IPedidoApiService
    {
        Task<List<PedidoResponse>> GetAllAsync();
        Task<PedidoResponse?> GetByIdAsync(int id);
        Task<PedidoResponse?> CreateAsync(PedidoRequest request);
        Task<List<PedidoResponse>> GetByClienteAsync(int clienteId);
        Task<bool> UpdateEstadoAsync(int pedidoId, string nuevoEstado);
    }
}
