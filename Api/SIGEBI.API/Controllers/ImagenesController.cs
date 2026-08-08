using Microsoft.AspNetCore.Mvc;
using SIGEBI.Persistence.Context;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly SigebiContext _context;

        public ImagenesController(IWebHostEnvironment env, SigebiContext context)
        {
            _env = env;
            _context = context;
        }

        [HttpPost("subir/{recursoId}")]
        public async Task<IActionResult> SubirImagen(int recursoId, IFormFile imagen)
        {
            try
            {
                if (imagen == null || imagen.Length == 0)
                    return BadRequest(new { mensaje = "No se recibio ninguna imagen." });

                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imagen.FileName).ToLower();

                if (!extensionesPermitidas.Contains(extension))
                    return BadRequest(new { mensaje = "Solo se permiten imagenes JPG, PNG o WEBP." });

                var carpeta = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                Directory.CreateDirectory(carpeta);

                var nombreArchivo = $"recurso_{recursoId}_{Guid.NewGuid()}{extension}";
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    await imagen.CopyToAsync(stream);

                var imagenUrl = $"/uploads/{nombreArchivo}";

                // Actualizar el recurso
                var recurso = await _context.RecursosBibliograficos.FindAsync(recursoId);
                if (recurso != null)
                {
                    recurso.ImagenUrl = imagenUrl;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { imagenUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}