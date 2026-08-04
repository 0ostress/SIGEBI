using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class RecursosController : Controller
    {
        private readonly RecursoApiService _recursoApiService;

        public RecursosController(RecursoApiService recursoApiService)
        {
            _recursoApiService = recursoApiService;
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
        public async Task<IActionResult> Crear(RecursoDTO recursoDto)
        {
            var (exito, mensaje) = await _recursoApiService.RegistrarAsync(recursoDto);
            if (exito)
            {
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
        public async Task<IActionResult> Editar(RecursoDTO recursoDto)
        {
            var (exito, mensaje) = await _recursoApiService.ActualizarAsync(recursoDto);
            if (exito)
            {
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
    }
}