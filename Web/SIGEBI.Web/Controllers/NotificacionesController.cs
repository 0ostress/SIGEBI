using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class NotificacionesController : Controller
    {
        private readonly NotificacionApiService _notificacionApiService;

        public NotificacionesController(NotificacionApiService notificacionApiService)
        {
            _notificacionApiService = notificacionApiService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
                return RedirectToAction("Login", "Auth");

            var usuarioId = int.Parse(usuarioIdStr);
            var notificaciones = await _notificacionApiService.ObtenerNoLeidasAsync(usuarioId);

            foreach (var n in notificaciones)
                await _notificacionApiService.MarcarComoLeidaAsync(n.Id);

            return View(notificaciones);
        }
    }
}