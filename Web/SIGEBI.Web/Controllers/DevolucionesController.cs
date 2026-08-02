using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases;

namespace SIGEBI.Web.Controllers
{
    public class DevolucionesController : Controller
    {
        private readonly RegistrarDevolucionUseCase _devolucionUseCase;

        public DevolucionesController(RegistrarDevolucionUseCase devolucionUseCase)
        {
            _devolucionUseCase = devolucionUseCase;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(int prestamoId)
        {
            await _devolucionUseCase.EjecutarAsync(prestamoId);
            return RedirectToAction("Index");
        }
    }
}