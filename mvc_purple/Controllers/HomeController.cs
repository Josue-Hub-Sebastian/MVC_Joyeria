using Microsoft.AspNetCore.Mvc;
using mvc_purple.api.IServices;
using mvc_purple.Extensions;
using mvc_purple.Models;

namespace mvc_purple.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductoApiService _productoService;
        private readonly IClienteApiService _clienteService;

        public HomeController(IProductoApiService productoService, IClienteApiService clienteService)
        {
            _productoService = productoService;
            _clienteService = clienteService;
        }

        public async Task<IActionResult> Index()
        {
            var cliente = _clienteService.GetClienteActivo();
            ViewBag.ClienteActivo = cliente;

            var productos = await _productoService.GetAllAsync();
            var disponibles = productos.Where(p => p.Disponible && p.Stock > 0).OrderBy(p => p.Nombre).ToList();

            return View(disponibles);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var cliente = _clienteService.GetClienteActivo();
            ViewBag.ClienteActivo = cliente;

            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null || !producto.Disponible) return NotFound();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarAlCarrito(int productoId, int cantidad = 1)
        {
            var cliente = _clienteService.GetClienteActivo();
            if (cliente == null)
            {
                TempData["Error"] = "Debes iniciar sesión para agregar productos al carrito.";
                return RedirectToAction("Login", "Cliente");
            }

            var producto = await _productoService.GetByIdAsync(productoId);
            if (producto == null)
            {
                TempData["Error"] = "El producto no existe o fue eliminado.";
                return RedirectToAction("Index");
            }

            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarrito>>("Carrito") ?? new List<ItemCarrito>();
            var itemExistente = carrito.FirstOrDefault(i => i.ProductoId == productoId);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new ItemCarrito
                {
                    ProductoId = producto.Id,
                    NombreProducto = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad
                });
            }

            HttpContext.Session.SetObjectAsJson("Carrito", carrito);
            TempData["Success"] = $"{producto.Nombre} agregado al carrito.";
            return RedirectToAction("Index");
        }


        public IActionResult Carrito()
        {
            var cliente = _clienteService.GetClienteActivo();
            if (cliente == null) return RedirectToAction("Login", "Cliente");

            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarrito>>("Carrito") ?? new List<ItemCarrito>();
            return View(carrito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarDelCarrito(int productoId)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarrito>>("Carrito") ?? new List<ItemCarrito>();
            carrito.RemoveAll(i => i.ProductoId == productoId);
            HttpContext.Session.SetObjectAsJson("Carrito", carrito);
            TempData["Success"] = "Producto eliminado del carrito";
            return RedirectToAction("Carrito");
        }

        public IActionResult Privacy() => View();
    }
}
