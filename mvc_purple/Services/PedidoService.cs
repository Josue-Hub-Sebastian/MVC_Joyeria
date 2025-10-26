using mvc_purple.Models;
using System.Text.Json;

namespace mvc_purple.Services
{
    public class PedidoService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public PedidoService(HttpClient http)
        {
            _http = http;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<Pedido>> GetAllAsync()
        {
            var res = await _http.GetAsync("/api/pedidos");
            if (!res.IsSuccessStatusCode)
                return new List<Pedido>();

            var stream = await res.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<Pedido>>(stream, _jsonOptions) ?? new List<Pedido>();
        }

        public async Task<Pedido?> GetByIdAsync(int id)
        {
            var res = await _http.GetAsync($"/api/pedidos/{id}");
            if (!res.IsSuccessStatusCode)
                return null;

            var stream = await res.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Pedido>(stream, _jsonOptions);
        }

        public async Task<bool> UpdateEstadoAsync(int pedidoId, string nuevoEstado)
        {
            var res = await _http.PatchAsJsonAsync($"/api/pedidos/{pedidoId}/estado", new { Estado = nuevoEstado });
            return res.IsSuccessStatusCode;
        }
    }
}
