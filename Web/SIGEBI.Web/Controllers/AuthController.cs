using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthApiService _authApiService;

        public AuthController(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var (exito, mensaje, token, rol, nombre) = await _authApiService.LoginAsync(loginDto);

            if (exito)
            {
                var partes = token.Split('|');
                HttpContext.Session.SetString("Token", token);
                HttpContext.Session.SetString("Rol", rol);
                HttpContext.Session.SetString("Nombre", nombre);
                HttpContext.Session.SetString("UsuarioId", partes[0]);
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = mensaje;
            return View(loginDto);
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegisterDTO registerDto)
        {
            var (exito, mensaje) = await _authApiService.RegistroAsync(registerDto);

            if (exito)
            {
                TempData["Exito"] = "Cuenta creada exitosamente. Inicia sesion.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = mensaje;
            return View(registerDto);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}