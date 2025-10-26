using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvc_purple.api.IServices;

using mvc_purple.Models;

namespace mvc_purple.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly IProductoApiService _productoService;
        private readonly IPedidoApiService _pedidoService;
        private readonly IClienteApiService _clienteService;

        public AdminController(IProductoApiService productoService,
                               IPedidoApiService pedidoService,
                               IClienteApiService clienteService)
        {
            _productoService = productoService;
            _pedidoService = pedidoService;
            _clienteService = clienteService;
        }


        // Dashboard
        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.GetAllAsync();
            var pedidos = await _pedidoService.GetAllAsync();

            var dashboard = new AdminDashboardViewModel
            {
                TotalPedidos = pedidos.Count,
                PedidosPendientes = pedidos.Count(p => p.Estado == "Pendiente"),
                TotalProductos = productos.Count,
                ProductosBajoStock = productos.Count(p => p.Stock <= 5),
                VentasDelMes = pedidos
                    .Where(p => p.FechaPedido.Month == DateTime.Now.Month && p.FechaPedido.Year == DateTime.Now.Year && p.Estado != "Cancelado")
                    .Sum(p => p.Total),
                UltimosPedidos = pedidos.OrderByDescending(p => p.FechaPedido).Take(5).ToList()
            };

            return View(dashboard);
        }

        // Listar productos (vista Productos.cshtml)
        public async Task<IActionResult> Productos()
        {
            var productos = await _productoService.GetAllAsync();
            return View(productos);
        }

        // Crear producto - GET
        public IActionResult CrearProducto()
        {
            return View();
        }

        // Crear producto - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            if (!ModelState.IsValid) return View(producto);

            producto.FechaCreacion = DateTime.Now;
            var creado = await _productoService.CreateAsync(producto);
            if (creado == null)
            {
                ModelState.AddModelError("", "No se pudo crear el producto.");
                return View(producto);
            }

            TempData["Success"] = "Producto creado exitosamente";
            return RedirectToAction(nameof(Productos));
        }

        // Editar producto - GET
        public async Task<IActionResult> EditarProducto(int id)
        {
            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        // Editar producto - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProducto(Producto producto)
        {
            if (!ModelState.IsValid) return View(producto);

            var ok = await _productoService.UpdateAsync(producto);
            if (!ok)
            {
                ModelState.AddModelError("", "No se pudo actualizar el producto.");
                return View(producto);
            }

            TempData["Success"] = "Producto actualizado exitosamente";
            return RedirectToAction(nameof(Productos));
        }

        // Eliminar / deshabilitar producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            // Intentamos eliminar (si API permite), si falla podrías implementar "disponible=false" mediante Update.
            var ok = await _productoService.DeleteAsync(id);
            if (!ok)
            {
                TempData["Error"] = "No se pudo eliminar el producto. Intente deshabilitarlo manualmente.";
            }
            else
            {
                TempData["Success"] = "Producto eliminado correctamente.";
            }
            return RedirectToAction(nameof(Productos));
        }

        // Lista de pedidos (vista Pedidos.cshtml)
        public async Task<IActionResult> Pedidos(string estado = "")
        {
            var pedidos = await _pedidoService.GetAllAsync();
            if (!string.IsNullOrEmpty(estado))
                pedidos = pedidos.Where(p => p.Estado == estado).ToList();

            ViewBag.EstadoFiltro = estado;
            return View(pedidos);
        }

        // Cambiar estado de pedido (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoPedido(int id, string nuevoEstado)
        {
            var ok = await _pedidoService.UpdateEstadoAsync(id, nuevoEstado);
            if (!ok)
            {
                TempData["Error"] = "No se pudo cambiar el estado.";
            }
            else
            {
                TempData["Success"] = $"Estado del pedido #{id} cambiado a {nuevoEstado}";
            }

            return RedirectToAction(nameof(Pedidos));
        }

        // Ver detalle de pedido (vista DetallePedido.cshtml)
        public async Task<IActionResult> DetallePedido(int id)
        {
            var pedido = await _pedidoService.GetByIdAsync(id);
            if (pedido == null) return NotFound();

            // Asegurar que cada detalle tenga el producto cargado (si la API no lo trae)
            foreach (var d in pedido.Detalles)
            {
                if (d.Producto == null)
                {
                    d.Producto = await _productoService.GetByIdAsync(d.ProductoId) ?? new Producto { Nombre = "Producto no disponible" };
                }
            }

            return View(pedido);
        }
    }
}
