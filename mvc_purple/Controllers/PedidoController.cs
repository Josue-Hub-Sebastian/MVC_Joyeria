using Microsoft.AspNetCore.Mvc;
using mvc_purple.Models;
using mvc_purple.Services;

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
