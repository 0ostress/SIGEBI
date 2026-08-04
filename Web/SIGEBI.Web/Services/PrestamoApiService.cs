using System.Net.Http.Json;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public class PrestamoApiService
    {
        private readonly HttpClient _httpClient;

        public PrestamoApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SigebiAPI");
        }

        public async Task<IEnumerable<PrestamoDTO>> ObtenerTodosAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PrestamoDTO>>("api/Prestamos");
        }

        public async Task<PrestamoDTO> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<PrestamoDTO>($"api/Prestamos/{id}");
        }

        public async Task<IEnumerable<PrestamoDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PrestamoDTO>>($"api/Prestamos/usuario/{usuarioId}");
        }

        public async Task<bool> SolicitarPrestamoAsync(int usuarioId, int recursoId)
        {
            var request = new { UsuarioId = usuarioId, RecursoId = recursoId };
            var response = await _httpClient.PostAsJsonAsync("api/Prestamos/solicitar", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RegistrarDevolucionAsync(int prestamoId)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Prestamos/devolver/{prestamoId}", new { });
            return response.IsSuccessStatusCode;
        }
    }
}