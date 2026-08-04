using System.Net.Http.Json;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public class RecursoApiService
    {
        private readonly HttpClient _httpClient;

        public RecursoApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SigebiAPI");
        }

        public async Task<IEnumerable<RecursoDTO>> ObtenerTodosAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<RecursoDTO>>("api/Recursos")
                       ?? Enumerable.Empty<RecursoDTO>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<RecursoDTO>();
            }
        }

        public async Task<IEnumerable<RecursoDTO>> ObtenerDisponiblesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<RecursoDTO>>("api/Recursos/disponibles")
                       ?? Enumerable.Empty<RecursoDTO>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<RecursoDTO>();
            }
        }

        public async Task<RecursoDTO?> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<RecursoDTO>($"api/Recursos/{id}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<(bool Exito, string Mensaje)> RegistrarAsync(RecursoDTO recursoDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Recursos", recursoDto);
                if (response.IsSuccessStatusCode)
                    return (true, "Recurso registrado exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al registrar el recurso.");
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

        public async Task<(bool Exito, string Mensaje)> ActualizarAsync(RecursoDTO recursoDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Recursos/{recursoDto.Id}", recursoDto);
                if (response.IsSuccessStatusCode)
                    return (true, "Recurso actualizado exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al actualizar el recurso.");
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

        public async Task<(bool Exito, string Mensaje)> EliminarAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Recursos/{id}");
                if (response.IsSuccessStatusCode)
                    return (true, "Recurso eliminado exitosamente.");

                return (false, "Error al eliminar el recurso.");
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