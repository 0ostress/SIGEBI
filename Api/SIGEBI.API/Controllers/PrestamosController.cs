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

        public PrestamosController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        // GET: api/prestamos
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var prestamos = await _prestamoService.ObtenerTodosAsync();
            return Ok(prestamos);
        }

        // GET: api/prestamos/1
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

        // GET: api/prestamos/usuario/1
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
        {
            var prestamos = await _prestamoService.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(prestamos);
        }

        // POST: api/prestamos/solicitar
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

        // POST: api/prestamos/devolver/1
        [HttpPost("devolver/{prestamoId}")]
        public async Task<IActionResult> RegistrarDevolucion(int prestamoId)
        {
            try
            {
                await _prestamoService.RegistrarDevolucionAsync(prestamoId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }

    public class SolicitarPrestamoRequest
    {
        public int UsuarioId { get; set; }
        public int RecursoId { get; set; }
    }
}