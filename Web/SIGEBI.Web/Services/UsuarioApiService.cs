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
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<UsuarioDTO>>("api/Usuarios")
                       ?? Enumerable.Empty<UsuarioDTO>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<UsuarioDTO>();
            }
        }

        public async Task<UsuarioDTO?> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UsuarioDTO>($"api/Usuarios/{id}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<(bool Exito, string Mensaje)> RegistrarAsync(UsuarioDTO usuarioDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Usuarios", usuarioDto);
                if (response.IsSuccessStatusCode)
                    return (true, "Usuario registrado exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al registrar el usuario.");
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

        public async Task<(bool Exito, string Mensaje)> ActualizarAsync(int id, UsuarioDTO usuarioDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Usuarios/{id}", usuarioDto);
                if (response.IsSuccessStatusCode)
                    return (true, "Usuario actualizado exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al actualizar el usuario.");
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

        public async Task<(bool Exito, string Mensaje)> CambiarEstadoAsync(int id, string nuevoEstado)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync($"api/Usuarios/{id}/estado", nuevoEstado);
                if (response.IsSuccessStatusCode)
                    return (true, "Estado actualizado exitosamente.");

                return (false, "Error al cambiar el estado del usuario.");
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

        public async Task<(bool Exito, string Mensaje)> EliminarAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Usuarios/{id}");
                if (response.IsSuccessStatusCode)
                    return (true, "Usuario eliminado exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al eliminar el usuario.");
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