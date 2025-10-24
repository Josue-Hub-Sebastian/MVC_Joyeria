using mvc_purple.Models;
using System.Net.Http.Json;

namespace mvc_purple.Services
{
    public class ProductoApiService : IProductoApiService
    {
        private readonly HttpClient _http;
        public ProductoApiService(HttpClient http) => _http = http;

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            var res = await _http.GetFromJsonAsync<IEnumerable<Producto>>("producto");
            return res ?? Enumerable.Empty<Producto>();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Producto>($"producto/{id}");
        }

        public async Task<Producto?> CreateAsync(Producto p)
        {
            var r = await _http.PostAsJsonAsync("producto", p);
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<Producto>();
        }

        public async Task<bool> UpdateAsync(Producto p)
        {
            var r = await _http.PutAsJsonAsync($"producto/{p.Id}", p);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var r = await _http.DeleteAsync($"producto/{id}");
            return r.IsSuccessStatusCode;
        }
    }
}
