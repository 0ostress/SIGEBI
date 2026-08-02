using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.Interfaces;

namespace SIGEBI.Web.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var usuario = await _usuarioService.ObtenerPorIdAsync(id);
            return View(usuario);
        }
    }
}