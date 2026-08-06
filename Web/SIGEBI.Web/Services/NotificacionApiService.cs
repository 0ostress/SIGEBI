using System.Net.Http.Json;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Web.Services
{
    public class NotificacionApiService
    {
        private readonly HttpClient _httpClient;

        public NotificacionApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SigebiAPI");
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("http://localhost:5200/");
        }

        public async Task<IEnumerable<Notificacion>> ObtenerNoLeidasAsync(int usuarioId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Notificacion>>($"api/Notificaciones/noleidas/{usuarioId}")
                       ?? Enumerable.Empty<Notificacion>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<Notificacion>();
            }
        }

        public async Task MarcarComoLeidaAsync(int notificacionId)
        {
            try
            {
                await _httpClient.PutAsJsonAsync($"api/Notificaciones/leer/{notificacionId}", new { });
            }
            catch (Exception) { }
        }

        public async Task<IEnumerable<Notificacion>> ObtenerTodasAsync(int usuarioId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Notificacion>>($"api/Notificaciones/usuario/{usuarioId}")
                       ?? Enumerable.Empty<Notificacion>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<Notificacion>();
            }
        }
    }
}