using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.UseCases;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;
        private readonly RegistrarDevolucionUseCase _devolucionUseCase;

        public PrestamosController(IPrestamoService prestamoService, RegistrarDevolucionUseCase devolucionUseCase)
        {
            _prestamoService = prestamoService;
            _devolucionUseCase = devolucionUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var prestamos = await _prestamoService.ObtenerTodosAsync();
            return Ok(prestamos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var prestamo = await _prestamoService.ObtenerPorIdAsync(id);
                return Ok(prestamo);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
        {
            var prestamos = await _prestamoService.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(prestamos);
        }

        [HttpPost("solicitar")]
        public async Task<IActionResult> SolicitarPrestamo([FromBody] SolicitarPrestamoRequest request)
        {
            try
            {
                var prestamo = await _prestamoService.SolicitarPrestamoAsync(request.UsuarioId, request.RecursoId);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = prestamo.Id }, prestamo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // POST: api/prestamos/aprobar/1

        [HttpPost("aprobar/{prestamoId}")]
        public async Task<IActionResult> AprobarPrestamo(int prestamoId, [FromBody] AprobarPrestamoRequest request)
        {
            try
            {
                var resultado = await _prestamoService.AprobarPrestamoAsync(prestamoId, request.FechaVencimiento);
                if (resultado)
                    return NoContent();
                return BadRequest(new { mensaje = "No se pudo aprobar el prestamo. Verifique que este pendiente y haya ejemplares disponibles." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // POST: api/prestamos/notificar-vencimientos
        [HttpPost("notificar-vencimientos")]
        public async Task<IActionResult> NotificarVencimientos()
        {
            try
            {
                await _prestamoService.NotificarVencimientosProximosAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("rechazar/{prestamoId}")]
        public async Task<IActionResult> RechazarPrestamo(int prestamoId, [FromBody] RechazarPrestamoRequest request)
        {
            try
            {
                var resultado = await _prestamoService.RechazarPrestamoAsync(prestamoId, request.Motivo);
                if (resultado)
                    return NoContent();
                return BadRequest(new { mensaje = "No se pudo rechazar el prestamo. Verifique que este pendiente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("renovar/{prestamoId}")]
        public async Task<IActionResult> RenovarPrestamo(int prestamoId, [FromBody] RenovarPrestamoRequest request)
        {
            try
            {
                var resultado = await _prestamoService.RenovarPrestamoAsync(prestamoId, request.UsuarioId);
                if (resultado)
                    return NoContent();
                return BadRequest(new { mensaje = "No se pudo renovar el prestamo. Verifique que este activo y no vencido." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }



        [HttpPost("devolver/{prestamoId}")]
        public async Task<IActionResult> RegistrarDevolucion(int prestamoId)
        {
            try
            {
                await _devolucionUseCase.EjecutarAsync(prestamoId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }

 

    public class AprobarPrestamoRequest
    {
        public DateTime FechaVencimiento { get; set; }
    }

    public class SolicitarPrestamoRequest
    {
        public int UsuarioId { get; set; }
        public int RecursoId { get; set; }
    }

    public class RechazarPrestamoRequest
    {
        public string Motivo { get; set; }
    }

    public class RenovarPrestamoRequest
    {
        public int UsuarioId { get; set; }
    }


}