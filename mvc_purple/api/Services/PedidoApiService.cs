using mvc_purple.api.IServices;
using mvc_purple.DTO.Request;
using mvc_purple.DTO.Response;
using System.Text.Json;

namespace mvc_purple.api.Services
{
    public class PedidoApiService : IPedidoApiService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public PedidoApiService(HttpClient http)
        {
            _http = http;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // 🔹 Obtener todos los pedidos
        public async Task<List<PedidoResponse>> GetAllAsync()
        {
            var res = await _http.GetAsync("/api/pedidos");
            if (!res.IsSuccessStatusCode) return new List<PedidoResponse>();

            var stream = await res.Content.ReadAsStreamAsync();

            // Si el API devuelve directamente la lista (sin "data")
            try
            {
                return await JsonSerializer.DeserializeAsync<List<PedidoResponse>>(stream, _jsonOptions) ?? new();
            }
            catch
            {
                // Si el API devuelve { success, data }
                stream.Position = 0;
                using var doc = await JsonDocument.ParseAsync(stream);
                if (doc.RootElement.TryGetProperty("data", out var dataElement))
                {
                    return JsonSerializer.Deserialize<List<PedidoResponse>>(dataElement.GetRawText(), _jsonOptions) ?? new();
                }
                return new();
            }
        }

        // 🔹 Obtener pedido por ID
        public async Task<PedidoResponse?> GetByIdAsync(int id)
        {
            var res = await _http.GetAsync($"/api/pedidos/{id}");
            if (!res.IsSuccessStatusCode) return null;

            var stream = await res.Content.ReadAsStreamAsync();

            using var doc = await JsonDocument.ParseAsync(stream);
            if (doc.RootElement.TryGetProperty("data", out var dataElement))
            {
                return JsonSerializer.Deserialize<PedidoResponse>(dataElement.GetRawText(), _jsonOptions);
            }

            // En caso de que el API devuelva directo el pedido
            return JsonSerializer.Deserialize<PedidoResponse>(doc.RootElement.GetRawText(), _jsonOptions);
        }

        // 🔹 Crear nuevo pedido
        public async Task<PedidoResponse?> CreateAsync(PedidoRequest request)
        {
            var res = await _http.PostAsJsonAsync("/api/pedidos", request, _jsonOptions);
            if (!res.IsSuccessStatusCode) return null;

            var stream = await res.Content.ReadAsStreamAsync();

            using var doc = await JsonDocument.ParseAsync(stream);
            if (doc.RootElement.TryGetProperty("data", out var dataElement))
            {
                return JsonSerializer.Deserialize<PedidoResponse>(dataElement.GetRawText(), _jsonOptions);
            }

            // En caso de que el API devuelva directamente el pedido
            return JsonSerializer.Deserialize<PedidoResponse>(doc.RootElement.GetRawText(), _jsonOptions);
        }

        // 🔹 Obtener pedidos de un cliente (filtrado localmente)
        public async Task<List<PedidoResponse>> GetByClienteAsync(int clienteId)
        {
            var pedidos = await GetAllAsync();
            return pedidos.Where(p => p.Id == clienteId).ToList();
        }

        // 🔹 Actualizar estado de pedido
        public async Task<bool> UpdateEstadoAsync(int pedidoId, string nuevoEstado)
        {
            var content = JsonContent.Create(new { Estado = nuevoEstado }, options: _jsonOptions);
            var res = await _http.PatchAsync($"/api/pedidos/{pedidoId}/estado", content);
            return res.IsSuccessStatusCode;
        }
    }
}
