using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ReporteApiService _reporteApiService;

        public ReportesController(ReporteApiService reporteApiService)
        {
            _reporteApiService = reporteApiService;
        }

        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Administrador")
                return RedirectToAction("Index", "Home");

            var reporte = await _reporteApiService.ObtenerReporteAsync();
            return View(reporte);
        }
    }
}