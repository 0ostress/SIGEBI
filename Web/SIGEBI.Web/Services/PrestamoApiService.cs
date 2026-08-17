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
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("http://localhost:5200/");
        }

        public async Task<IEnumerable<PrestamoDTO>> ObtenerTodosAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<PrestamoDTO>>("api/Prestamos")
                       ?? Enumerable.Empty<PrestamoDTO>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<PrestamoDTO>();
            }
        }

        public async Task<PrestamoDTO?> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<PrestamoDTO>($"api/Prestamos/{id}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<PrestamoDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<PrestamoDTO>>($"api/Prestamos/usuario/{usuarioId}")
                       ?? Enumerable.Empty<PrestamoDTO>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<PrestamoDTO>();
            }
        }

        public async Task NotificarVencimientosAsync()
        {
            try
            {
                await _httpClient.PostAsJsonAsync("api/Prestamos/notificar-vencimientos", new { });
            }
            catch (Exception) { }
        }

        public async Task<(bool Exito, string Mensaje)> SolicitarPrestamoAsync(int usuarioId, int recursoId)
        {
            try
            {
                var request = new { UsuarioId = usuarioId, RecursoId = recursoId };
                var response = await _httpClient.PostAsJsonAsync("api/Prestamos/solicitar", request);
                if (response.IsSuccessStatusCode)
                    return (true, "Prestamo solicitado exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al solicitar el prestamo.");
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

        public async Task<(bool Exito, string Mensaje)> RegistrarDevolucionAsync(int prestamoId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Prestamos/devolver/{prestamoId}", new { });
                if (response.IsSuccessStatusCode)
                    return (true, "Devolucion registrada exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al registrar la devolucion.");
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

        public async Task<(bool Exito, string Mensaje)> AprobarPrestamoAsync(int prestamoId, DateTime fechaVencimiento)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Prestamos/aprobar/{prestamoId}", new { FechaVencimiento = fechaVencimiento });
                if (response.IsSuccessStatusCode)
                    return (true, "Prestamo aprobado exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al aprobar el prestamo.");
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor.");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }

        public async Task<(bool Exito, string Mensaje)> RechazarPrestamoAsync(int prestamoId, string motivo)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Prestamos/rechazar/{prestamoId}", new { Motivo = motivo });
                if (response.IsSuccessStatusCode)
                    return (true, "Prestamo rechazado exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al rechazar el prestamo.");
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor.");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }

        public async Task<(bool Exito, string Mensaje)> RenovarPrestamoAsync(int prestamoId, int usuarioId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Prestamos/renovar/{prestamoId}", new { UsuarioId = usuarioId });
                if (response.IsSuccessStatusCode)
                    return (true, "Prestamo renovado exitosamente por 7 dias mas.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al renovar el prestamo.");
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor.");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }

    }
}