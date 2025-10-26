using Microsoft.AspNetCore.Mvc;
using mvc_purple.api.IServices;
using mvc_purple.Models;

namespace mvc_purple.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoApiService _prodService;

        public ProductoController(IProductoApiService prodService) => _prodService = prodService;

        public async Task<IActionResult> Index()
        {
            var productos = await _prodService.GetAllAsync();
            return View(productos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var p = await _prodService.GetByIdAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // Estas acciones las dejo para uso general (no admin)
        public IActionResult Crear() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto p)
        {
            if (!ModelState.IsValid) return View(p);
            var creado = await _prodService.CreateAsync(p);
            if (creado == null) ModelState.AddModelError("", "No se pudo crear el producto.");
            return RedirectToAction(nameof(Index));
        }
    }
}
