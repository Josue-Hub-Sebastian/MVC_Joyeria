using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using mvc_purple.api.IServices;
using mvc_purple.Models;
using System.Security.Claims;

namespace mvc_purple.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteApiService _clienteService;
        private readonly IPedidoApiService _pedidoService;

        public ClienteController(IClienteApiService clienteService, IPedidoApiService pedidoService)
        {
            _clienteService = clienteService;
            _pedidoService = pedidoService;
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

        public async Task<IActionResult> MisPedidos()
        {
            var cliente = _clienteService.GetClienteActivo();
            if (cliente == null) return RedirectToAction("Login", "Cliente");

            var pedidos = await _pedidoService.GetByClienteAsync(cliente.Id);
            return View(pedidos);
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

            var cliente = _clienteService.GetClienteActivo();
            if (cliente == null)
            {
                ModelState.AddModelError("", "Error al cargar los datos del cliente.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, cliente.Nombre),
                new Claim(ClaimTypes.Email, cliente.Email),
                new Claim("EsAdmin", cliente.EsAdmin.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(8)
                });

            TempData["Success"] = "Inicio de sesión exitoso.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _clienteService.LogoutAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Success"] = "Sesión cerrada correctamente.";
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
