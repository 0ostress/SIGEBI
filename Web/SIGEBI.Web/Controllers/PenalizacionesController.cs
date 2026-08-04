using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class PenalizacionesController : Controller
    {
        private readonly PenalizacionApiService _penalizacionApiService;

        public PenalizacionesController(PenalizacionApiService penalizacionApiService)
        {
            _penalizacionApiService = penalizacionApiService;
        }

        public async Task<IActionResult> Index()
        {
            var penalizaciones = await _penalizacionApiService.ObtenerTodasAsync();
            return View(penalizaciones);
        }

        public async Task<IActionResult> Activas()
        {
            var penalizaciones = await _penalizacionApiService.ObtenerActivasAsync();
            return View(penalizaciones);
        }

        [HttpPost]
        public async Task<IActionResult> Aplicar(int prestamoId)
        {
            await _penalizacionApiService.AplicarPenalizacionAsync(prestamoId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Resolver(int penalizacionId)
        {
            await _penalizacionApiService.ResolverPenalizacionAsync(penalizacionId);
            return RedirectToAction("Index");
        }
    }
}