using Microsoft.AspNetCore.Authentication;
using mvc_purple.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace mvc_purple.Services
{
    public class ClienteApiService : IClienteApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string SessionTokenKey = "JWToken";
        private const string SessionClienteKey = "ClienteActivo";

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
            var r = await _http.PostAsJsonAsync("auth/register", c);
            if (!r.IsSuccessStatusCode)
                return null;

            var doc = await r.Content.ReadFromJsonAsync<JsonElement>();
            if (doc.ValueKind != JsonValueKind.Object)
                return null;

            var cliente = new Cliente
            {
                Id = doc.TryGetProperty("id", out var id) ? id.GetInt32() : 0,
                Nombre = doc.TryGetProperty("nombre", out var n) ? n.GetString() ?? "" : "",
                Email = doc.TryGetProperty("email", out var e) ? e.GetString() ?? "" : "",
                Direccion = doc.TryGetProperty("direccion", out var d) ? d.GetString() : null
            };

            return cliente;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var r = await _http.PostAsJsonAsync("auth/login", new { Email = email, Password = password });
            if (!r.IsSuccessStatusCode)
                return null;

            var doc = await r.Content.ReadFromJsonAsync<JsonElement>();
            if (doc.ValueKind != JsonValueKind.Object)
                return null;

            string? token = null;
            Cliente? cliente = null;

            // 🔹 Leer token y cliente
            if (doc.TryGetProperty("token", out var t1) || doc.TryGetProperty("Token", out t1))
                token = t1.GetString();

            if (doc.TryGetProperty("cliente", out var c1) || doc.TryGetProperty("Cliente", out c1))
            {
                try
                {
                    cliente = JsonSerializer.Deserialize<Cliente>(
                        c1.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch { }
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // 🔸 Guardar token y cliente en sesión
                if (!string.IsNullOrWhiteSpace(token))
                    httpContext.Session.SetString(SessionTokenKey, token);

                if (cliente != null)
                {
                    var clienteJson = JsonSerializer.Serialize(cliente);
                    httpContext.Session.SetString(SessionClienteKey, clienteJson);

                    // ✅ Crear Claims seguros (sin valores nulos)
                    var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, cliente.Nombre ?? "Cliente"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, cliente.Email ?? "desconocido@ejemplo.com")
            };

                    var identity = new System.Security.Claims.ClaimsIdentity(claims, "SesionCliente");
                    var principal = new System.Security.Claims.ClaimsPrincipal(identity);

                    await httpContext.SignInAsync(principal);
                }
            }

            return token;
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

        public async Task LogoutAsync()
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx != null)
            {
                ctx.Session.Remove(SessionTokenKey);
                ctx.Session.Remove(SessionClienteKey);
                await ctx.SignOutAsync();
            }
        }


        // 🔹 Método útil para obtener cliente desde sesión
        public Cliente? GetClienteActivo()
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return null;

            var json = ctx.Session.GetString("ClienteActivo");
            if (string.IsNullOrEmpty(json)) return null;

            return JsonSerializer.Deserialize<Cliente>(json);
        }

    }
}
