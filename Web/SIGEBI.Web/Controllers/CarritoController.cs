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
        public IActionResult Agregar(int recursoId, string titulo, string autor)
        {
            var carrito = ObtenerCarrito();

            if (!carrito.Any(c => c.RecursoId == recursoId))
            {
                carrito.Add(new CarritoItem { RecursoId = recursoId, Titulo = titulo, Autor = autor });
                GuardarCarrito(carrito);
            }

            return Json(new { exito = true, cantidad = carrito.Count });
        }

        [HttpPost]
        public IActionResult Quitar(int recursoId)
        {
            var carrito = ObtenerCarrito();
            carrito.RemoveAll(c => c.RecursoId == recursoId);
            GuardarCarrito(carrito);
            return Json(new { exito = true, cantidad = carrito.Count });
        }

        [HttpGet]
        public IActionResult ObtenerItems()
        {
            var carrito = ObtenerCarrito();
            return Json(carrito);
        }

        [HttpPost]
        public async Task<IActionResult> Confirmar()
        {
            var carrito = ObtenerCarrito();
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdStr) || !carrito.Any())
                return Json(new { exito = false, mensaje = "No hay items en el carrito." });

            var usuarioId = int.Parse(usuarioIdStr);
            int exitosos = 0;
            int fallidos = 0;

            foreach (var item in carrito)
            {
                var (exito, _) = await _prestamoApiService.SolicitarPrestamoAsync(usuarioId, item.RecursoId);
                if (exito) exitosos++;
                else fallidos++;
            }

            GuardarCarrito(new List<CarritoItem>());

            return Json(new { exito = true, exitosos, fallidos });
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