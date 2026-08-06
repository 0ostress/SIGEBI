using Microsoft.AspNetCore.Mvc;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionRepository _notificacionRepository;

        public NotificacionesController(INotificacionRepository notificacionRepository)
        {
            _notificacionRepository = notificacionRepository;
        }

        // GET: api/notificaciones/usuario/1
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
        {
            var notificaciones = await _notificacionRepository.GetByUsuarioIdAsync(usuarioId);
            return Ok(notificaciones);
        }

        // GET: api/notificaciones/noleidas/1
        [HttpGet("noleidas/{usuarioId}")]
        public async Task<IActionResult> ObtenerNoLeidas(int usuarioId)
        {
            var notificaciones = await _notificacionRepository.GetNoLeidasAsync(usuarioId);
            return Ok(notificaciones);
        }

        // PUT: api/notificaciones/leer/1
        [HttpPut("leer/{notificacionId}")]
        public async Task<IActionResult> MarcarComoLeida(int notificacionId)
        {
            await _notificacionRepository.MarcarComoLeidaAsync(notificacionId);
            return NoContent();
        }
    }
}