using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.DTOs;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecursosController : ControllerBase
    {
        private readonly IRecursoService _recursoService;

        public RecursosController(IRecursoService recursoService)
        {
            _recursoService = recursoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var recursos = await _recursoService.ObtenerTodosAsync();
            return Ok(recursos);
        }

        [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerDisponibles()
        {
            var recursos = await _recursoService.ObtenerDisponiblesAsync();
            return Ok(recursos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var recurso = await _recursoService.ObtenerPorIdAsync(id);
                return Ok(recurso);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RecursoDTO recursoDto)
        {
            try
            {
                var nuevo = await _recursoService.RegistrarAsync(recursoDto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevo.Id }, nuevo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] RecursoDTO recursoDto)
        {
            try
            {
                recursoDto.Id = id;
                await _recursoService.ActualizarAsync(recursoDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _recursoService.EliminarAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}