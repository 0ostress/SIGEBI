using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.UseCases;

namespace SIGEBI.Web.Controllers
{
    public class DevolucionesController : Controller
    {
        private readonly RegistrarDevolucionUseCase _devolucionUseCase;
        private readonly IPrestamoService _prestamoService;

        public DevolucionesController(RegistrarDevolucionUseCase devolucionUseCase, IPrestamoService prestamoService)
        {
            _devolucionUseCase = devolucionUseCase;
            _prestamoService = prestamoService;
        }

        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoService.ObtenerTodosAsync();
            return View(prestamos);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(int prestamoId)
        {
            await _devolucionUseCase.EjecutarAsync(prestamoId);
            return RedirectToAction("Index");
        }
    }
}
