using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.Interfaces;

namespace SIGEBI.Web.Controllers
{
    public class PenalizacionesController : Controller
    {
        private readonly IPenalizacionService _penalizacionService;

        public PenalizacionesController(IPenalizacionService penalizacionService)
        {
            _penalizacionService = penalizacionService;
        }

        public async Task<IActionResult> Index()
        {
            var penalizaciones = await _penalizacionService.ObtenerTodasAsync();
            return View(penalizaciones);
        }

        public async Task<IActionResult> Activas()
        {
            var penalizaciones = await _penalizacionService.ObtenerActivasAsync();
            return View(penalizaciones);
        }
    }
}