using System.Net.Http.Json;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public class PenalizacionApiService
    {
        private readonly HttpClient _httpClient;

        public PenalizacionApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SigebiAPI");
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerTodasAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<PenalizacionDTO>>("api/Penalizaciones")
                       ?? Enumerable.Empty<PenalizacionDTO>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<PenalizacionDTO>();
            }
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerActivasAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<PenalizacionDTO>>("api/Penalizaciones/activas")
                       ?? Enumerable.Empty<PenalizacionDTO>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<PenalizacionDTO>();
            }
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<PenalizacionDTO>>($"api/Penalizaciones/usuario/{usuarioId}")
                       ?? Enumerable.Empty<PenalizacionDTO>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<PenalizacionDTO>();
            }
        }

        public async Task<(bool Exito, string Mensaje)> AplicarPenalizacionAsync(int prestamoId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Penalizaciones/aplicar/{prestamoId}", new { });
                if (response.IsSuccessStatusCode)
                    return (true, "Penalizacion aplicada exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al aplicar la penalizacion.");
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor. Verifique que la API este en ejecucion.");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }

        public async Task<(bool Exito, string Mensaje)> ResolverPenalizacionAsync(int penalizacionId)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Penalizaciones/resolver/{penalizacionId}", new { });
                if (response.IsSuccessStatusCode)
                    return (true, "Penalizacion resuelta exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al resolver la penalizacion.");
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor. Verifique que la API este en ejecucion.");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }
    }
}