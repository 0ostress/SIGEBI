using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.Interfaces;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PenalizacionesController : ControllerBase
    {
        private readonly IPenalizacionService _penalizacionService;

        public PenalizacionesController(IPenalizacionService penalizacionService)
        {
            _penalizacionService = penalizacionService;
        }

        // GET: api/penalizaciones
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var penalizaciones = await _penalizacionService.ObtenerTodasAsync();
            return Ok(penalizaciones);
        }

        // GET: api/penalizaciones/activas
        [HttpGet("activas")]
        public async Task<IActionResult> ObtenerActivas()
        {
            var penalizaciones = await _penalizacionService.ObtenerActivasAsync();
            return Ok(penalizaciones);
        }

        // GET: api/penalizaciones/usuario/1
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
        {
            var penalizaciones = await _penalizacionService.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(penalizaciones);
        }

        // POST: api/penalizaciones/aplicar/1
        [HttpPost("aplicar/{prestamoId}")]
        public async Task<IActionResult> AplicarPenalizacion(int prestamoId)
        {
            try
            {
                await _penalizacionService.AplicarPenalizacionAsync(prestamoId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT: api/penalizaciones/resolver/1
        [HttpPut("resolver/{penalizacionId}")]
        public async Task<IActionResult> ResolverPenalizacion(int penalizacionId)
        {
            try
            {
                await _penalizacionService.ResolverPenalizacionAsync(penalizacionId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
    }
}