using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly PrestamoApiService _prestamoApiService;
        private readonly UsuarioApiService _usuarioApiService;
        private readonly RecursoApiService _recursoApiService;

        public PrestamosController(
            PrestamoApiService prestamoApiService,
            UsuarioApiService usuarioApiService,
            RecursoApiService recursoApiService)
        {
            _prestamoApiService = prestamoApiService;
            _usuarioApiService = usuarioApiService;
            _recursoApiService = recursoApiService;
        }

        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoApiService.ObtenerTodosAsync();
            return View(prestamos);
        }

        public async Task<IActionResult> PorUsuario(int usuarioId)
        {
            var prestamos = await _prestamoApiService.ObtenerPorUsuarioAsync(usuarioId);
            return View(prestamos);
        }

        public async Task<IActionResult> Solicitar()
        {
            ViewBag.Usuarios = await _usuarioApiService.ObtenerTodosAsync();
            ViewBag.Recursos = await _recursoApiService.ObtenerDisponiblesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Solicitar(int usuarioId, int recursoId)
        {
            var (exito, mensaje) = await _prestamoApiService.SolicitarPrestamoAsync(usuarioId, recursoId);
            if (exito)
            {
                TempData["Exito"] = mensaje;
                return RedirectToAction("Index");
            }
            TempData["Error"] = mensaje;
            ViewBag.Usuarios = await _usuarioApiService.ObtenerTodosAsync();
            ViewBag.Recursos = await _recursoApiService.ObtenerDisponiblesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Devolver(int prestamoId)
        {
            var (exito, mensaje) = await _prestamoApiService.RegistrarDevolucionAsync(prestamoId);
            if (exito)
                TempData["Exito"] = mensaje;
            else
                TempData["Error"] = mensaje;

            return RedirectToAction("Index");
        }
    }
}