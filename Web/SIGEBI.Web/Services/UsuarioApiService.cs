using System.Net.Http.Json;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public class UsuarioApiService
    {
        private readonly HttpClient _httpClient;

        public UsuarioApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SigebiAPI");
        }

        public async Task<IEnumerable<UsuarioDTO>> ObtenerTodosAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<UsuarioDTO>>("api/Usuarios");
        }

        public async Task<UsuarioDTO> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<UsuarioDTO>($"api/Usuarios/{id}");
        }

        public async Task<bool> RegistrarAsync(UsuarioDTO usuarioDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Usuarios", usuarioDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarAsync(int id, UsuarioDTO usuarioDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Usuarios/{id}", usuarioDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CambiarEstadoAsync(int id, string nuevoEstado)
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/Usuarios/{id}/estado", nuevoEstado);
            return response.IsSuccessStatusCode;
        }
    }
}