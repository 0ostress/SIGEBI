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
            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(UsuarioDTO usuarioDto)
        {
            var (exito, mensaje) = await _usuarioApiService.RegistrarAsync(usuarioDto);
            if (exito)
            {
                TempData["Exito"] = mensaje;
                return RedirectToAction("Index");
            }
            TempData["Error"] = mensaje;
            return View(usuarioDto);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _usuarioApiService.ObtenerPorIdAsync(id);
            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, UsuarioDTO usuarioDto)
        {
            var (exito, mensaje) = await _usuarioApiService.ActualizarAsync(id, usuarioDto);
            if (exito)
            {
                TempData["Exito"] = mensaje;
                return RedirectToAction("Index");
            }
            TempData["Error"] = mensaje;
            return View(usuarioDto);
        }
    }
}