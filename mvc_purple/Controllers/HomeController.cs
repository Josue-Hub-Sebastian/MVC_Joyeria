using Microsoft.AspNetCore.Mvc;
using mvc_purple.Extensions;
using mvc_purple.Models;
using mvc_purple.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

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

        // Página principal - Catálogo de productos
        public async Task<IActionResult> Index()
        {
            // 🔹 Verificar si hay un cliente activo en sesión
            var cliente = _clienteService.GetClienteActivo();
            ViewBag.ClienteActivo = cliente;

            // Obtener productos desde el API
            var productos = await _productoService.GetAllAsync();

            var disponibles = productos
                .Where(p => p.Disponible && p.Stock > 0)
                .OrderBy(p => p.Nombre)
                .ToList();

            return View(disponibles);
        }

        // Detalle de un producto
        public async Task<IActionResult> Detalle(int id)
        {
            var cliente = _clienteService.GetClienteActivo();
            ViewBag.ClienteActivo = cliente;

            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null || !producto.Disponible)
                return NotFound();

            return View(producto);
        }

        // Agregar al carrito
        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int productoId, int cantidad = 1)
        {
            var cliente = _clienteService.GetClienteActivo();
            if (cliente == null)
            {
                TempData["Error"] = "Debes iniciar sesión para agregar productos al carrito.";
                return RedirectToAction("Login", "Cliente");
            }

            var producto = await _productoService.GetByIdAsync(productoId);

            if (producto == null || !producto.Disponible || producto.Stock < cantidad)
            {
                TempData["Error"] = "Producto no disponible o stock insuficiente";
                return RedirectToAction("Index");
            }

            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarrito>>("Carrito")
                         ?? new List<ItemCarrito>();

            var itemExistente = carrito.FirstOrDefault(i => i.ProductoId == productoId);

            if (itemExistente != null)
                itemExistente.Cantidad += cantidad;
            else
                carrito.Add(new ItemCarrito
                {
                    ProductoId = productoId,
                    NombreProducto = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad
                });

            HttpContext.Session.SetObjectAsJson("Carrito", carrito);
            TempData["Success"] = $"{producto.Nombre} agregado al carrito";
            return RedirectToAction("Index");
        }

        // Ver carrito
        public IActionResult Carrito()
        {
            var cliente = _clienteService.GetClienteActivo();
            if (cliente == null)
                return RedirectToAction("Login", "Cliente");

            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarrito>>("Carrito")
                         ?? new List<ItemCarrito>();

            return View(carrito);
        }

        // Eliminar del carrito
        [HttpPost]
        public IActionResult EliminarDelCarrito(int productoId)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarrito>>("Carrito")
                         ?? new List<ItemCarrito>();

            carrito.RemoveAll(i => i.ProductoId == productoId);
            HttpContext.Session.SetObjectAsJson("Carrito", carrito);

            TempData["Success"] = "Producto eliminado del carrito";
            return RedirectToAction("Carrito");
        }

        public IActionResult AccessDenied() => View();
        public IActionResult Error() => View();
    }
}
