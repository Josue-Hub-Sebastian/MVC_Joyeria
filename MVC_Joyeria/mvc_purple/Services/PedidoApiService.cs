using mvc_purple.Models;
using mvc_purple.Models.DTOs;
using System.Net.Http.Json;

namespace mvc_purple.Services
{
    public class PedidoApiService : IPedidoApiService
    {
        private readonly HttpClient _http;
        public PedidoApiService(HttpClient http) => _http = http;

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            //var res = await _http.GetFromJsonAsync<IEnumerable<Pedido>>("pedido");
            //return res ?? Enumerable.Empty<Pedido>();

            var res = await _http.GetFromJsonAsync<IEnumerable<PedidoResponse>>("pedido");
            return res?.Select(r => new Pedido
            {
                Id = r.Id,
                Total = r.Total,
                FechaPedido = r.Fecha,
                Cliente = new Cliente { Nombre = r.ClienteNombre }
            }) ?? Enumerable.Empty<Pedido>();

        }

        //public async Task<Pedido?> GetByIdAsync(int id)
        public async Task<PedidoResponse?> GetByIdAsync(int id)
        {
            // return await _http.GetFromJsonAsync<Pedido>($"pedido/{id}");
            return await _http.GetFromJsonAsync<PedidoResponse>($"pedido/{id}");
        }

        public async Task<Pedido?> CreateAsync(PedidoRequest request)
        {
            var r = await _http.PostAsJsonAsync("pedido", request);
            if (!r.IsSuccessStatusCode)
            {
                var error = await r.Content.ReadAsStringAsync();
                Console.WriteLine("Error al crear pedido: " + error); // opcional para depurar
                return null;
            }

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
