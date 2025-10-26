using mvc_purple.Models;

namespace mvc_purple.api.IServices
{
    public interface IProductoApiService
    {
        Task<List<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto?> CreateAsync(Producto producto);
        Task<bool> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(int id);
    }
}
