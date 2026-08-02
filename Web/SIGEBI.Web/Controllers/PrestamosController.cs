using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.Interfaces;

namespace SIGEBI.Web.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamosController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoService.ObtenerTodosAsync();
            return View(prestamos);
        }

        public async Task<IActionResult> PorUsuario(int usuarioId)
        {
            var prestamos = await _prestamoService.ObtenerPorUsuarioAsync(usuarioId);
            return View(prestamos);
        }

        public async Task<IActionResult> Solicitar()
        {
            return View();
        }
    }
}