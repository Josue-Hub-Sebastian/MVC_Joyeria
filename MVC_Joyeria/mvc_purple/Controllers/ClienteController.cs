using Microsoft.AspNetCore.Mvc;
using mvc_purple.Models;
using mvc_purple.Services;
using System.Text.Json;

namespace mvc_purple.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteApiService _clienteService;
        private readonly IPedidoApiService _pedidoService;
        public ClienteController(IClienteApiService clienteService, IPedidoApiService pedidoApiService)
        {
            _clienteService = clienteService;
            _pedidoService = pedidoApiService;
        }
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

            TempData["Success"] = "Registro exitoso. Ahora puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            // Si ya hay sesión activa, redirige al home
            var clienteActivo = HttpContext.Session.GetString("ClienteActivo");
            if (!string.IsNullOrEmpty(clienteActivo))
                return RedirectToAction("Index", "Home");

            return View();
        }

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

            TempData["Success"] = "Inicio de sesión exitoso.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _clienteService.LogoutAsync();
            TempData["Success"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var c = await _clienteService.GetByIdAsync(id);
            if (c == null) return NotFound();
            return View(c);
        }


        [HttpGet]
        public async Task<IActionResult> MisPedidos()
        {
            var pedidos = await _pedidoService.GetAllAsync();

            var usuarioEmail = User.Identity.Name;
            var pedidosUsuario = pedidos
                .Where(p => p.Cliente != null && p.Cliente.Email == usuarioEmail)
                .ToList();

            return View(pedidosUsuario);
        }


    }
}
