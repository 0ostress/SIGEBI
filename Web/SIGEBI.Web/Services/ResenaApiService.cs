using System.Net.Http.Json;

namespace SIGEBI.Web.Services
{
    public class ResenaDTO
    {
        public int Id { get; set; }
        public int RecursoId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public int Estrellas { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class ResenaApiService
    {
        private readonly HttpClient _httpClient;

        public ResenaApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SigebiAPI");
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("http://localhost:5200/");
        }

        public async Task<IEnumerable<ResenaDTO>> ObtenerPorRecursoAsync(int recursoId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<ResenaDTO>>($"api/Resenas/recurso/{recursoId}")
                       ?? Enumerable.Empty<ResenaDTO>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<ResenaDTO>();
            }
        }

        public async Task<(bool Exito, string Mensaje)> CrearAsync(int recursoId, int usuarioId, int estrellas, string comentario)
        {
            try
            {
                var resena = new
                {
                    RecursoId = recursoId,
                    UsuarioId = usuarioId,
                    Estrellas = estrellas,
                    Comentario = comentario
                };

                var response = await _httpClient.PostAsJsonAsync("api/Resenas", resena);
                if (response.IsSuccessStatusCode)
                    return (true, "Reseña agregada exitosamente.");

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return (false, error?.GetValueOrDefault("mensaje") ?? "Error al agregar la reseña.");
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