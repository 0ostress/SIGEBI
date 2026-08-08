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
            ViewBag.UsuarioId = HttpContext.Session.GetString("UsuarioId");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Solicitar(int usuarioId, int recursoId)
        {
            var (exito, mensaje) = await _prestamoApiService.SolicitarPrestamoAsync(usuarioId, recursoId);
            if (exito)
            {
                TempData["ExitoSolicitud"] = true;
                return RedirectToAction("Solicitar");
            }
            TempData["Error"] = mensaje;
            ViewBag.Usuarios = await _usuarioApiService.ObtenerTodosAsync();
            ViewBag.Recursos = await _recursoApiService.ObtenerDisponiblesAsync();
            ViewBag.UsuarioId = HttpContext.Session.GetString("UsuarioId");
            return View();
        }

        public async Task<IActionResult> MisPrestamos()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
                return RedirectToAction("Login", "Auth");
            var prestamos = await _prestamoApiService.ObtenerPorUsuarioAsync(int.Parse(usuarioId));
            return View(prestamos);
        }

        [HttpPost]
        public async Task<IActionResult> Aprobar(int prestamoId, DateTime fechaVencimiento)
        {
            var (exito, mensaje) = await _prestamoApiService.AprobarPrestamoAsync(prestamoId, fechaVencimiento);
            if (exito)
                TempData["Exito"] = mensaje;
            else
                TempData["Error"] = mensaje;
            return RedirectToAction("Pendientes");
        }

        [HttpPost]
        public async Task<IActionResult> Rechazar(int prestamoId, string motivo)
        {
            var (exito, mensaje) = await _prestamoApiService.RechazarPrestamoAsync(prestamoId, motivo);
            if (exito)
                TempData["Exito"] = mensaje;
            else
                TempData["Error"] = mensaje;

            return RedirectToAction("Pendientes");
        }

        public async Task<IActionResult> Pendientes()
        {
            var prestamos = await _prestamoApiService.ObtenerTodosAsync();
            var pendientes = prestamos.Where(p => p.Estado == "Pendiente");
            return View(pendientes);
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

        [HttpPost]
        public async Task<IActionResult> SolicitarAjax([FromBody] SolicitarPrestamoRequest request)
        {
            var (exito, mensaje) = await _prestamoApiService.SolicitarPrestamoAsync(request.UsuarioId, request.RecursoId);
            if (exito)
                return Ok(new { mensaje });
            return BadRequest(new { mensaje });
        }

        [HttpPost]
        public async Task<IActionResult> Renovar(int prestamoId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
                return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _prestamoApiService.RenovarPrestamoAsync(prestamoId, int.Parse(usuarioIdStr));
            if (exito)
                TempData["Exito"] = mensaje;
            else
                TempData["Error"] = mensaje;

            return RedirectToAction("MisPrestamos");
        }

        public class SolicitarPrestamoRequest
        {
            public int UsuarioId { get; set; }
            public int RecursoId { get; set; }
        }
    }
}