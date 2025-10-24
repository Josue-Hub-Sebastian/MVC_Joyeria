using mvc_purple.Models;

namespace mvc_purple.Services
{
    public interface IProductoApiService
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto?> CreateAsync(Producto p);
        Task<bool> UpdateAsync(Producto p);
        Task<bool> DeleteAsync(int id);
    }
}
