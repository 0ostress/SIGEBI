using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Context;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResenasController : ControllerBase
    {
        private readonly SigebiContext _context;

        public ResenasController(SigebiContext context)
        {
            _context = context;
        }

        // GET: api/resenas/recurso/1
        [HttpGet("recurso/{recursoId}")]
        public async Task<IActionResult> ObtenerPorRecurso(int recursoId)
        {
            var resenas = await _context.Resenas
                .Include(r => r.Usuario)
                .Where(r => r.RecursoId == recursoId)
                .OrderByDescending(r => r.FechaCreacion)
                .Select(r => new
                {
                    r.Id,
                    r.RecursoId,
                    r.UsuarioId,
                    NombreUsuario = $"{r.Usuario.Nombre} {r.Usuario.Apellido}",
                    r.Estrellas,
                    r.Comentario,
                    r.FechaCreacion
                })
                .ToListAsync();

            return Ok(resenas);
        }

        // POST: api/resenas
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Resena resena)
        {
            try
            {
                // Verificar si ya dejó una reseña
                var existente = await _context.Resenas
                    .FirstOrDefaultAsync(r => r.RecursoId == resena.RecursoId && r.UsuarioId == resena.UsuarioId);

                if (existente != null)
                    return BadRequest(new { mensaje = "Ya dejaste una reseña para este libro." });

                if (resena.Estrellas < 1 || resena.Estrellas > 5)
                    return BadRequest(new { mensaje = "La valoración debe ser entre 1 y 5 estrellas." });

                resena.FechaCreacion = DateTime.Now;
                _context.Resenas.Add(resena);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Reseña agregada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}