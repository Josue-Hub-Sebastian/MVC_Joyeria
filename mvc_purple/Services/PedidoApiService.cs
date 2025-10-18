using mvc_purple.Models;
using System.Net.Http.Json;

namespace mvc_purple.Services
{
    public class PedidoApiService : IPedidoApiService
    {
        private readonly HttpClient _http;
        public PedidoApiService(HttpClient http) => _http = http;

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            var res = await _http.GetFromJsonAsync<IEnumerable<Pedido>>("pedido");
            return res ?? Enumerable.Empty<Pedido>();
        }

        public async Task<Pedido?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Pedido>($"pedido/{id}");
        }

        public async Task<Pedido?> CreateAsync(Pedido p)
        {
            var r = await _http.PostAsJsonAsync("pedido", p);
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<Pedido>();
        }

        public async Task<bool> CambiarEstadoAsync(int id, string nuevoEstado)
        {
            // Usa PATCH a /pedido/{id}/estado con objeto { Estado = nuevoEstado }
            var r = await _http.PatchAsJsonAsync($"pedido/{id}/estado", new { Estado = nuevoEstado });
            return r.IsSuccessStatusCode;
        }
    }
}
