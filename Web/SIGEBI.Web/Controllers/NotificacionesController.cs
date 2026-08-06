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

            // Marcar no leidas como leidas
            var noLeidas = await _notificacionApiService.ObtenerNoLeidasAsync(usuarioId);
            foreach (var n in noLeidas)
                await _notificacionApiService.MarcarComoLeidaAsync(n.Id);

            // Mostrar todas
            var todas = await _notificacionApiService.ObtenerTodasAsync(usuarioId);
            return View(todas);
        }
    }
}