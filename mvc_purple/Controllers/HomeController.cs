using Microsoft.AspNetCore.Mvc;
using mvc_purple.Extensions;
using mvc_purple.Models;
using mvc_purple.Services;
using System.Linq;
using System.Threading.Tasks;

namespace mvc_purple.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductoApiService _productoService;

        public HomeController(IProductoApiService productoService)
        {
            _productoService = productoService;
        }

        // Página principal - Catálogo de productos
        public async Task<IActionResult> Index()
        {
            // Obtener productos desde el API
            var productos = await _productoService.GetAllAsync();

            // Filtrar solo disponibles y con stock > 0
            var disponibles = productos
                .Where(p => p.Disponible && p.Stock > 0)
                .OrderBy(p => p.Nombre)
                .ToList();

            return View(disponibles);
        }

        // Detalle de un producto
        public async Task<IActionResult> Detalle(int id)
        {
            var producto = await _productoService.GetByIdAsync(id);

            if (producto == null || !producto.Disponible)
            {
                return NotFound();
            }

            return View(producto);
        }

        // Agregar al carrito (usando sesiones)
        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int productoId, int cantidad = 1)
        {
            var producto = await _productoService.GetByIdAsync(productoId);

            if (producto == null || !producto.Disponible || producto.Stock < cantidad)
            {
                TempData["Error"] = "Producto no disponible o stock insuficiente";
                return RedirectToAction("Index");
            }

            // Obtener carrito de la sesión
            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarrito>>("Carrito")
                         ?? new List<ItemCarrito>();

            // Verificar si el producto ya está en el carrito
            var itemExistente = carrito.FirstOrDefault(i => i.ProductoId == productoId);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new ItemCarrito
                {
                    ProductoId = productoId,
                    NombreProducto = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad
                });
            }

            // Guardar carrito en sesión
            HttpContext.Session.SetObjectAsJson("Carrito", carrito);

            TempData["Success"] = $"{producto.Nombre} agregado al carrito";
            return RedirectToAction("Index");
        }

        // Ver carrito
        public IActionResult Carrito()
        {
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

        // Página de acceso denegado
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Página de error
        public IActionResult Error()
        {
            return View();
        }
    }
}
