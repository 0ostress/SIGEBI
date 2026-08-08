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

        public IActionResult CrearPrivilegiado()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Administrador")
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearPrivilegiado(UsuarioDTO usuarioDto)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Administrador")
                return RedirectToAction("Index", "Home");

            var (exito, mensaje) = await _usuarioApiService.RegistrarAsync(usuarioDto);
            if (exito)
            {
                TempData["Exito"] = mensaje;
                return RedirectToAction("Index");
            }
            TempData["Error"] = mensaje;
            return View(usuarioDto);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Administrador")
                return RedirectToAction("Index", "Home");

            var (exito, mensaje) = await _usuarioApiService.EliminarAsync(id);
            if (exito)
                TempData["Exito"] = mensaje;
            else
                TempData["Error"] = mensaje;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Administrador")
                return RedirectToAction("Index", "Home");

            var (exito, mensaje) = await _usuarioApiService.CambiarEstadoAsync(id, nuevoEstado);
            if (exito)
                TempData["Exito"] = mensaje;
            else
                TempData["Error"] = mensaje;

            return RedirectToAction("Index");
        }
    }
}