using Microsoft.AspNetCore.Mvc;
using mvc_purple.api.IServices;
using mvc_purple.DTO.Request;
using mvc_purple.Extensions;
using mvc_purple.Models;

namespace mvc_purple.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoApiService _pedidoService;
        private readonly IClienteApiService _clienteService;

        public PedidoController(IPedidoApiService pedidoService, IClienteApiService clienteService)
        {
            _pedidoService = pedidoService;
            _clienteService = clienteService;
        }

        public IActionResult Checkout()
        {
            var cliente = _clienteService.GetClienteActivo();
            if (cliente == null) return RedirectToAction("Login", "Cliente");

            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarrito>>("Carrito") ?? new List<ItemCarrito>();
            var vm = new CheckoutViewModel
            {
                ItemsCarrito = carrito,
                Total = carrito.Sum(i => i.Subtotal)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcesarPedido(string direccionEnvio, string? observaciones)
        {
            var cliente = _clienteService.GetClienteActivo();
            if (cliente == null) return RedirectToAction("Login", "Cliente");

            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarrito>>("Carrito") ?? new List<ItemCarrito>();
            if (!carrito.Any()) return RedirectToAction("Carrito", "Home");

            var request = new PedidoRequest
            {
                ClienteId = cliente.Id,
                DireccionEnvio = direccionEnvio,
                Observaciones = observaciones,
                ProductosIds = carrito.Select(c => c.ProductoId).ToList(),
                Cantidades = carrito.Select(c => c.Cantidad).ToList()
            };

            var pedido = await _pedidoService.CreateAsync(request);
            if (pedido == null)
            {
                TempData["Error"] = "Hubo un problema al procesar el pedido";
                return RedirectToAction("Checkout");
            }

            HttpContext.Session.Remove("Carrito");
            return RedirectToAction("Confirmacion", new { id = pedido.Id });
        }

        public async Task<IActionResult> Confirmacion(int id)
        {
            var pedido = await _pedidoService.GetByIdAsync(id);
            if (pedido == null) return NotFound();
            return View(pedido);
        }
    }
}
