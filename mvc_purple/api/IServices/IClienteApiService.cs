using mvc_purple.Models;

namespace mvc_purple.api.IServices
{
    public interface IClienteApiService
    {
        Task<List<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(int id);
        Task<Cliente?> RegisterAsync(Cliente c);
        Task<string?> LoginAsync(string email, string password);
        Task LogoutAsync();
        Cliente? GetClienteActivo();
        Task SetClienteActivoAsync(Cliente cliente);
    }
}
