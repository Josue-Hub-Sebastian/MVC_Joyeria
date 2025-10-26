using System.Net.Http.Json;
using System.Text.Json;
using mvc_purple.api.IServices;
using mvc_purple.Models;

namespace mvc_purple.api.Services
{
    public class ProductoApiService : IProductoApiService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductoApiService(HttpClient http)
        {
            _http = http;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Producto>> GetAllAsync()
        {
            var res = await _http.GetAsync("/api/productos");
            if (!res.IsSuccessStatusCode) return new List<Producto>();
            var stream = await res.Content.ReadAsStreamAsync();
            var productos = await JsonSerializer.DeserializeAsync<List<Producto>>(stream, _jsonOptions);
            return productos ?? new List<Producto>();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            var res = await _http.GetAsync($"/api/productos/{id}");
            if (!res.IsSuccessStatusCode) return null;

            var stream = await res.Content.ReadAsStreamAsync();

            using var doc = await JsonDocument.ParseAsync(stream);
            if (!doc.RootElement.TryGetProperty("data", out var dataElement))
                return null;

            var producto = JsonSerializer.Deserialize<Producto>(dataElement.GetRawText(), _jsonOptions);
            return producto;
        }

        public async Task<Producto?> CreateAsync(Producto producto)
        {
            var res = await _http.PostAsJsonAsync("/api/productos", producto, _jsonOptions);
            if (!res.IsSuccessStatusCode) return null;
            var stream = await res.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Producto>(stream, _jsonOptions);
        }

        public async Task<bool> UpdateAsync(Producto producto)
        {
            var res = await _http.PutAsJsonAsync($"/api/productos/{producto.Id}", producto, _jsonOptions);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await _http.DeleteAsync($"/api/productos/{id}");
            return res.IsSuccessStatusCode;
        }
    }
}
