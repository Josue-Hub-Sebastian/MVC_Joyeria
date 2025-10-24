using Microsoft.AspNetCore.Mvc;
using mvc_purple.Models;
using mvc_purple.Models.DTOs;
using mvc_purple.Services;
using Newtonsoft.Json;

namespace mvc_purple.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoApiService _pedidoService;
        public PedidoController(IPedidoApiService pedidoService) => _pedidoService = pedidoService;

        public async Task<IActionResult> Index()
        {
            var pedidos = await _pedidoService.GetAllAsync();
            return View(pedidos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var p = await _pedidoService.GetByIdAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }


        //CREADO PARA EL CHECKOUT
        [HttpGet]
        public IActionResult Checkout()
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            var items = string.IsNullOrEmpty(carritoJson)
                ? new List<ItemCarrito>()
                : JsonConvert.DeserializeObject<List<ItemCarrito>>(carritoJson);

            var total = items.Sum(i => i.Subtotal);

            var model = new CheckoutViewModel
            {
                ItemsCarrito = items,
                Total = total
            };

            return View(model);
        }




        //No lo llama nada pero esta aqui :v
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcesarPedido(string direccionEnvio, string observaciones)
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            var items = string.IsNullOrEmpty(carritoJson)
                ? new List<ItemCarrito>()
                : JsonConvert.DeserializeObject<List<ItemCarrito>>(carritoJson);

            if (!items.Any())
            {
                TempData["Error"] = "El carrito está vacío.";
                return RedirectToAction("Checkout");
            }

            var clienteJson = HttpContext.Session.GetString("ClienteActivo");
            var cliente = string.IsNullOrEmpty(clienteJson)
                ? null
                : JsonConvert.DeserializeObject<Cliente>(clienteJson);

            if (cliente == null)
            {
                TempData["Error"] = "No se pudo identificar al cliente.";
                return RedirectToAction("Login", "Cliente");
            }

            var detalles = items.Select(i => new DetallePedido
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.Precio
            }).ToList();

            var pedidoRequest = new PedidoRequest
            {
                ClienteId = cliente.Id,
                DireccionEnvio = direccionEnvio,
                Observaciones = observaciones,
                ProductosIds = items.Select(i => i.ProductoId).ToList(),
                Cantidades = items.Select(i => i.Cantidad).ToList()
            };

            var creado = await _pedidoService.CreateAsync(pedidoRequest);

            if (creado == null)
            {
                TempData["Error"] = "No se pudo procesar el pedido.";
                return RedirectToAction("Checkout");
            }

            // Limpiar carrito
            HttpContext.Session.Remove("Carrito");

            TempData["Success"] = "¡Pedido realizado con éxito!";
            return RedirectToAction("MisPedidos", "Cliente");
        }


        //CREADO PARA EL CHEACK


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
        {
            var ok = await _pedidoService.CambiarEstadoAsync(id, nuevoEstado);
            if (!ok) return BadRequest();
            return RedirectToAction(nameof(Index));
        }

    }
}
