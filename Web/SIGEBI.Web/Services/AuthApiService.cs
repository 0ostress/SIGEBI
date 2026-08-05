using System.Net.Http.Json;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;

        public AuthApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SigebiAPI");
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("http://localhost:5200/");
        }

        public async Task<(bool Exito, string Mensaje, string Token, string Rol, string Nombre)> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                    var token = resultado?["token"]?.ToString() ?? "";
                    var rol = resultado?["rol"]?.ToString() ?? "Estudiante";
                    var nombre = resultado?["nombre"]?.ToString() ?? "";
                    return (true, "Login exitoso.", token, rol, nombre);
                }

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?["mensaje"] ?? "Error al iniciar sesion.", "", "", "");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", "", "", "");
            }
        }

        public async Task<(bool Exito, string Mensaje)> RegistroAsync(RegisterDTO registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/registro", registerDto);
                if (response.IsSuccessStatusCode)
                    return (true, "Usuario registrado exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?["mensaje"] ?? "Error al registrar.");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
    }
}