using Microsoft.AspNetCore.Mvc;
using mvc_purple.Models;
using mvc_purple.Services;

namespace mvc_purple.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteApiService _clienteService;
        public ClienteController(IClienteApiService clienteService) => _clienteService = clienteService;

        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteService.GetAllAsync();
            return View(clientes);
        }

        public IActionResult Registro() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(Cliente c)
        {
            if (!ModelState.IsValid) return View(c);

            var creado = await _clienteService.RegisterAsync(c);
            if (creado == null)
            {
                ModelState.AddModelError("", "No se pudo registrar al cliente.");
                return View(c);
            }
            return RedirectToAction("Login");
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email y contraseña son obligatorios.");
                return View();
            }

            var token = await _clienteService.LoginAsync(email, password);
            if (token == null)
            {
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View();
            }

            // token almacenado en sesión por ClienteApiService
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _clienteService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var c = await _clienteService.GetByIdAsync(id);
            if (c == null) return NotFound();
            return View(c);
        }
    }
}
