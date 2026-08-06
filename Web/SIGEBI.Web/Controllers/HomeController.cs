using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Models;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NotificacionApiService _notificacionApiService;

        public HomeController(ILogger<HomeController> logger, NotificacionApiService notificacionApiService)
        {
            _logger = logger;
            _notificacionApiService = notificacionApiService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            var rol = HttpContext.Session.GetString("Rol");

            if (!string.IsNullOrEmpty(usuarioIdStr) && (rol == "Estudiante" || rol == "Docente"))
            {
                var usuarioId = int.Parse(usuarioIdStr);
                var notificaciones = await _notificacionApiService.ObtenerNoLeidasAsync(usuarioId);
                ViewBag.Notificaciones = notificaciones.ToList();

                foreach (var n in notificaciones)
                    await _notificacionApiService.MarcarComoLeidaAsync(n.Id);
            }
            else
            {
                ViewBag.Notificaciones = new List<object>();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}