using mvc_purple.api.IServices;
using mvc_purple.DTO.Response;
using mvc_purple.Models;
using System.Text.Json;

namespace mvc_purple.api.Services
{
    public class ClienteApiService : IClienteApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string SESSION_CLIENTE = "ClienteActivo";

        public ClienteApiService(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<Cliente>> GetAllAsync()
        {
            var res = await _http.GetAsync("/api/clientes");
            if (!res.IsSuccessStatusCode) return new List<Cliente>();
            var stream = await res.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<Cliente>>(stream, _jsonOptions) ?? new();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            var res = await _http.GetAsync($"/api/clientes/{id}");
            if (!res.IsSuccessStatusCode) return null;
            var stream = await res.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Cliente>(stream, _jsonOptions);
        }

        public async Task<Cliente?> RegisterAsync(Cliente c)
        {
            var res = await _http.PostAsJsonAsync("/api/clientes/register", c, _jsonOptions);
            if (!res.IsSuccessStatusCode) return null;
            var stream = await res.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Cliente>(stream, _jsonOptions);
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var res = await _http.PostAsJsonAsync("clientes/login", new { Email = email, Password = password }, _jsonOptions);
            if (!res.IsSuccessStatusCode) return null;

            var wrapper = await res.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(_jsonOptions);
            if (wrapper?.Data?.Token != null && wrapper.Data.Cliente != null)
            {
                var ctx = _httpContextAccessor.HttpContext;
                if (ctx != null)
                {
                    // Guarda token en sesión
                    ctx.Session.Set("Token", System.Text.Encoding.UTF8.GetBytes(wrapper.Data.Token));
                }

                await SetClienteActivoAsync(wrapper.Data.Cliente);
                return wrapper.Data.Token;
            }

            return null;
        }

        public Task LogoutAsync()
        {
            var ctx = _httpContextAccessor.HttpContext;
            ctx?.Session.Remove(SESSION_CLIENTE);
            return Task.CompletedTask;
        }

        public Cliente? GetClienteActivo()
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return null;
            var json = ctx.Session.GetString(SESSION_CLIENTE);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<Cliente>(json, _jsonOptions);
        }

        public Task SetClienteActivoAsync(Cliente cliente)
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return Task.CompletedTask;
            ctx.Session.SetString(SESSION_CLIENTE, JsonSerializer.Serialize(cliente, _jsonOptions));
            return Task.CompletedTask;
        }

        private class LoginResponse
        {
            public string? Token { get; set; }
            public Cliente? Cliente { get; set; }
        }
    }
}
