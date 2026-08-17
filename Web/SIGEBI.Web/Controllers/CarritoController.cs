using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.Controllers
{
    public class CarritoController : Controller
    {
        private readonly PrestamoApiService _prestamoApiService;

        public CarritoController(PrestamoApiService prestamoApiService)     
        {
            _prestamoApiService = prestamoApiService;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Agregar(int recursoId, string titulo, string autor)
        {
            var carrito = ObtenerCarrito();

            if (carrito.Any(c => c.RecursoId == recursoId))
                return Json(new { exito = false, mensaje = "Este libro ya está en tu carrito.", cantidad = carrito.Count });

            carrito.Add(new CarritoItem { RecursoId = recursoId, Titulo = titulo, Autor = autor });
            GuardarCarrito(carrito);
            return Json(new { exito = true, cantidad = carrito.Count });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Quitar(int recursoId)
        {
            var carrito = ObtenerCarrito();
            carrito.RemoveAll(c => c.RecursoId == recursoId);
            GuardarCarrito(carrito);
            return Json(new { exito = true, cantidad = carrito.Count });
        }

        [HttpGet]
        [IgnoreAntiforgeryToken]
        public IActionResult ObtenerItems()
        {
            var carrito = ObtenerCarrito();
            return Json(carrito);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Confirmar()
        {
            var carrito = ObtenerCarrito();
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdStr) || !carrito.Any())
                return Json(new { exito = false, mensaje = "No hay items en el carrito.", exitosos = 0, fallidos = 0 });

            var usuarioId = int.Parse(usuarioIdStr);
            int exitosos = 0;
            int fallidos = 0;
            var errores = new List<string>();

            foreach (var item in carrito)
            {
                var (exito, mensaje) = await _prestamoApiService.SolicitarPrestamoAsync(usuarioId, item.RecursoId);
                if (exito) exitosos++;
                else
                {
                    fallidos++;
                    errores.Add(mensaje);
                }
            }

            if (exitosos > 0)
                GuardarCarrito(new List<CarritoItem>());

            return Json(new { exito = exitosos > 0, exitosos, fallidos, errores });
        }

        private List<CarritoItem> ObtenerCarrito()
        {
            var json = HttpContext.Session.GetString("Carrito");
            return string.IsNullOrEmpty(json) ? new List<CarritoItem>() : JsonConvert.DeserializeObject<List<CarritoItem>>(json);
        }

        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(carrito));
        }
    }

    public class CarritoItem
    {
        public int RecursoId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
    }
}