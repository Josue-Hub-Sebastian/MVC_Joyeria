using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_purple.Data;
using mvc_purple.Models;

namespace mvc_purple.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly JoyeriaDbContext _context;

        public AdminController(JoyeriaDbContext context)
        {
            _context = context;
        }

        // Dashboard principal del admin
        public async Task<IActionResult> Index()
        {
            var dashboard = new AdminDashboardViewModel
            {
                TotalPedidos = await _context.Pedidos.CountAsync(),
                PedidosPendientes = await _context.Pedidos.CountAsync(p => p.Estado == "Pendiente"),
                TotalProductos = await _context.Productos.CountAsync(),
                ProductosBajoStock = await _context.Productos.CountAsync(p => p.Stock <= 5),
                VentasDelMes = await _context.Pedidos
                    .Where(p => p.FechaPedido.Month == DateTime.Now.Month
                             && p.FechaPedido.Year == DateTime.Now.Year
                             && p.Estado != "Cancelado")
                    .SumAsync(p => p.Total),
                UltimosPedidos = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .OrderByDescending(p => p.FechaPedido)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboard);
        }

        // Gestión de pedidos
        public async Task<IActionResult> Pedidos(string estado = "")
        {
            var query = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(p => p.Estado == estado);
            }

            var pedidos = await query
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            ViewBag.EstadoFiltro = estado;
            return View(pedidos);
        }

        // Cambiar estado de pedido
        [HttpPost]
        public async Task<IActionResult> CambiarEstadoPedido(int id, string nuevoEstado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            pedido.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Estado del pedido #{id} cambiado a {nuevoEstado}";
            return RedirectToAction("Pedidos");
        }

        // Gestión de productos - Lista
        public async Task<IActionResult> Productos()
        {
            var productos = await _context.Productos
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return View(productos);
        }

        // Crear producto - GET
        public IActionResult CrearProducto()
        {
            return View();
        }

        // Crear producto - POST
        [HttpPost]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.FechaCreacion = DateTime.Now;
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Producto creado exitosamente";
                return RedirectToAction("Productos");
            }

            return View(producto);
        }

        // Editar producto - GET
        public async Task<IActionResult> EditarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // Editar producto - POST
        [HttpPost]
        public async Task<IActionResult> EditarProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Update(producto);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Producto actualizado exitosamente";
                return RedirectToAction("Productos");
            }

            return View(producto);
        }

        // Eliminar producto
        [HttpPost]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            // Verificar si tiene pedidos asociados
            var tienePedidos = await _context.DetallesPedido
                .AnyAsync(d => d.ProductoId == id);

            if (tienePedidos)
            {
                // Solo deshabilitar si tiene pedidos
                producto.Disponible = false;
                _context.Update(producto);
                TempData["Info"] = "Producto deshabilitado (tiene pedidos asociados)";
            }
            else
            {
                // Eliminar completamente si no tiene pedidos
                _context.Productos.Remove(producto);
                TempData["Success"] = "Producto eliminado exitosamente";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Productos");
        }

        // Ver detalle de pedido
        public async Task<IActionResult> DetallePedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }
    }
}