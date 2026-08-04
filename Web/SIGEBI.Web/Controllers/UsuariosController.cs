using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioApiService _usuarioApiService;

        public UsuariosController(UsuarioApiService usuarioApiService)
        {
            _usuarioApiService = usuarioApiService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioApiService.ObtenerTodosAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var usuario = await _usuarioApiService.ObtenerPorIdAsync(id);
            return View(usuario);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(UsuarioDTO usuarioDto)
        {
            var resultado = await _usuarioApiService.RegistrarAsync(usuarioDto);
            if (resultado)
                return RedirectToAction("Index");
            return View(usuarioDto);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _usuarioApiService.ObtenerPorIdAsync(id);
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, UsuarioDTO usuarioDto)
        {
            var resultado = await _usuarioApiService.ActualizarAsync(id, usuarioDto);
            if (resultado)
                return RedirectToAction("Index");
            return View(usuarioDto);
        }
    }
}