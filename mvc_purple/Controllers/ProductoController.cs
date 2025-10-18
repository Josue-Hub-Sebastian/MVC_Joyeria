using Microsoft.AspNetCore.Mvc;
using mvc_purple.Models;
using mvc_purple.Services;

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

        public IActionResult Crear() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto p)
        {
            if (!ModelState.IsValid) return View(p);
            var creado = await _prodService.CreateAsync(p);
            if (creado == null)
            {
                ModelState.AddModelError("", "No se pudo crear el producto.");
                return View(p);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            var p = await _prodService.GetByIdAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Producto p)
        {
            if (!ModelState.IsValid) return View(p);
            var ok = await _prodService.UpdateAsync(p);
            if (!ok)
            {
                ModelState.AddModelError("", "No se pudo actualizar el producto.");
                return View(p);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ok = await _prodService.DeleteAsync(id);
            if (!ok) return BadRequest();
            return RedirectToAction(nameof(Index));
        }
    }
}
