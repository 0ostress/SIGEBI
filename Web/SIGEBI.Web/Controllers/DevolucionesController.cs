using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class DevolucionesController : Controller
    {
        private readonly PrestamoApiService _prestamoApiService;

        public DevolucionesController(PrestamoApiService prestamoApiService)
        {
            _prestamoApiService = prestamoApiService;
        }

        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoApiService.ObtenerTodosAsync();
            return View(prestamos);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(int prestamoId)
        {
            await _prestamoApiService.RegistrarDevolucionAsync(prestamoId);
            return RedirectToAction("Index");
        }
    }
}