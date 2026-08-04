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
            return await _httpClient.GetFromJsonAsync<IEnumerable<PenalizacionDTO>>("api/Penalizaciones");
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerActivasAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PenalizacionDTO>>("api/Penalizaciones/activas");
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PenalizacionDTO>>($"api/Penalizaciones/usuario/{usuarioId}");
        }

        public async Task<bool> AplicarPenalizacionAsync(int prestamoId)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Penalizaciones/aplicar/{prestamoId}", new { });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ResolverPenalizacionAsync(int penalizacionId)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Penalizaciones/resolver/{penalizacionId}", new { });
            return response.IsSuccessStatusCode;
        }
    }
}