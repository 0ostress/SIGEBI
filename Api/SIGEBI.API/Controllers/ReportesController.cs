using Microsoft.AspNetCore.Mvc;
using SIGEBI.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly SigebiContext _context;

        public ReportesController(SigebiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerReporte()
        {
            var totalPrestamosActivos = await _context.Prestamos
                .CountAsync(p => p.Estado == "Activo");

            var totalPrestamosPendientes = await _context.Prestamos
                .CountAsync(p => p.Estado == "Pendiente");

            var totalRecursosDisponibles = await _context.RecursosBibliograficos
                .CountAsync(r => r.Estado == "Disponible");

            var usuariosConPenalizaciones = await _context.Penalizaciones
                .Where(p => p.Estado == "Activa")
                .Select(p => p.UsuarioId)
                .Distinct()
                .CountAsync();

            var recursosMasPrestados = await _context.Prestamos
                .GroupBy(p => p.RecursoId)
                .Select(g => new
                {
                    RecursoId = g.Key,
                    TotalPrestamos = g.Count()
                })
                .OrderByDescending(g => g.TotalPrestamos)
                .Take(5)
                .Join(_context.RecursosBibliograficos,
                    p => p.RecursoId,
                    r => r.Id,
                    (p, r) => new
                    {
                        r.Titulo,
                        r.Autor,
                        p.TotalPrestamos
                    })
                .ToListAsync();

            return Ok(new
            {
                totalPrestamosActivos,
                totalPrestamosPendientes,
                totalRecursosDisponibles,
                usuariosConPenalizaciones,
                recursosMasPrestados
            });
        }
    }
}