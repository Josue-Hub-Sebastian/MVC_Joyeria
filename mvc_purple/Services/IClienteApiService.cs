using mvc_purple.Models;

namespace mvc_purple.Services
{
    public interface IClienteApiService
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(int id);
        Task<Cliente?> RegisterAsync(Cliente c);
        Task<string?> LoginAsync(string email, string password); // devuelve token
        Task<bool> UpdateAsync(Cliente c);
        Task<bool> DeleteAsync(int id);
        Task LogoutAsync();
        Cliente? GetClienteActivo();
    }
}
