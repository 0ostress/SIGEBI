using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class RecursosController : Controller
    {
        private readonly RecursoApiService _recursoApiService;
        private readonly ImagenApiService _imagenApiService;
        private readonly ResenaApiService _resenaApiService;

        public RecursosController(RecursoApiService recursoApiService, ImagenApiService imagenApiService, ResenaApiService resenaApiService)
        {
            _recursoApiService = recursoApiService;
            _imagenApiService = imagenApiService;
            _resenaApiService = resenaApiService;
        }

        public async Task<IActionResult> Index()
        {
            var recursos = await _recursoApiService.ObtenerTodosAsync();
            return View(recursos);
        }

        public async Task<IActionResult> Disponibles()
        {
            var recursos = await _recursoApiService.ObtenerDisponiblesAsync();
            return View(recursos);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(RecursoDTO recursoDto, IFormFile? imagenFile)
        {
            var (exito, mensaje) = await _recursoApiService.RegistrarAsync(recursoDto);
            if (exito)
            {
                if (imagenFile != null && imagenFile.Length > 0)
                {
                    var recursos = await _recursoApiService.ObtenerTodosAsync();
                    var nuevoRecurso = recursos.OrderByDescending(r => r.Id).FirstOrDefault();
                    if (nuevoRecurso != null)
                        await _imagenApiService.SubirImagenAsync(nuevoRecurso.Id, imagenFile);
                }
                TempData["Exito"] = mensaje;
                return RedirectToAction("Index");
            }
            TempData["Error"] = mensaje;
            return View(recursoDto);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var recurso = await _recursoApiService.ObtenerPorIdAsync(id);
            if (recurso == null)
            {
                TempData["Error"] = "Recurso no encontrado.";
                return RedirectToAction("Index");
            }
            return View(recurso);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(RecursoDTO recursoDto, IFormFile? imagenFile)
        {
            var (exito, mensaje) = await _recursoApiService.ActualizarAsync(recursoDto);
            if (exito)
            {
                if (imagenFile != null && imagenFile.Length > 0)
                    await _imagenApiService.SubirImagenAsync(recursoDto.Id, imagenFile);

                TempData["Exito"] = mensaje;
                return RedirectToAction("Index");
            }
            TempData["Error"] = mensaje;
            return View(recursoDto);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var (exito, mensaje) = await _recursoApiService.EliminarAsync(id);
            if (exito)
                TempData["Exito"] = mensaje;
            else
                TempData["Error"] = mensaje;

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var recurso = await _recursoApiService.ObtenerPorIdAsync(id);
            if (recurso == null)
            {
                TempData["Error"] = "Recurso no encontrado.";
                return RedirectToAction("Disponibles");
            }

            var resenas = await _resenaApiService.ObtenerPorRecursoAsync(id);
            var promedio = resenas.Any() ? resenas.Average(r => r.Estrellas) : 0;

            var viewModel = new SIGEBI.Web.ViewModels.RecursoDetalleViewModel
            {
                Recurso = recurso,
                Resenas = resenas,
                PromedioEstrellas = Math.Round(promedio, 1)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarResena(int recursoId, int estrellas, string comentario)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
                return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _resenaApiService.CrearAsync(recursoId, int.Parse(usuarioIdStr), estrellas, comentario);
            if (exito)
                TempData["Exito"] = mensaje;
            else
                TempData["Error"] = mensaje;

            return RedirectToAction("Detalle", new { id = recursoId });
        }
    }
}