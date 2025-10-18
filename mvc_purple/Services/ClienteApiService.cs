using mvc_purple.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace mvc_purple.Services
{
    public class ClienteApiService : IClienteApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "JWToken";

        public ClienteApiService(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            var res = await _http.GetFromJsonAsync<IEnumerable<Cliente>>("cliente");
            return res ?? Enumerable.Empty<Cliente>();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Cliente>($"cliente/{id}");
        }

        public async Task<Cliente?> RegisterAsync(Cliente c)
        {
            // El endpoint en el API es POST: api/Cliente/register
            var r = await _http.PostAsJsonAsync("cliente/register", c);
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<Cliente>();
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var r = await _http.PostAsJsonAsync("cliente/login", new { Email = email, Password = password });
            if (!r.IsSuccessStatusCode) return null;

            // Se espera un JSON con { Token = "...", Cliente = {...} } según tu API
            var doc = await r.Content.ReadFromJsonAsync<JsonElement>();
            if (doc.ValueKind == JsonValueKind.Object)
            {
                // Intenta propiedades con mayúscula/minúscula
                if (doc.TryGetProperty("token", out var t1) || doc.TryGetProperty("Token", out t1))
                {
                    var token = t1.GetString();
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        // Guardar token en sesión
                        _httpContextAccessor.HttpContext?.Session.SetString(SessionKey, token);
                        return token;
                    }
                }
            }
            return null;
        }

        public async Task<bool> UpdateAsync(Cliente c)
        {
            var r = await _http.PutAsJsonAsync($"cliente/{c.Id}", c);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var r = await _http.DeleteAsync($"cliente/{id}");
            return r.IsSuccessStatusCode;
        }

        public Task LogoutAsync()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(SessionKey);
            return Task.CompletedTask;
        }
    }
}
