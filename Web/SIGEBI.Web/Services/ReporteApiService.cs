using System.Net.Http.Json;

namespace SIGEBI.Web.Services
{
    public class ReporteApiService
    {
        private readonly HttpClient _httpClient;

        public ReporteApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SigebiAPI");
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("http://localhost:5200/");
        }

        public async Task<ReporteDTO?> ObtenerReporteAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ReporteDTO>("api/Reportes");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class RecursoMasPrestadoDTO
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public int TotalPrestamos { get; set; }
    }

    public class ReporteDTO
    {
        public int TotalPrestamosActivos { get; set; }
        public int TotalPrestamosPendientes { get; set; }
        public int TotalRecursosDisponibles { get; set; }
        public int UsuariosConPenalizaciones { get; set; }
        public List<RecursoMasPrestadoDTO> RecursosMasPrestados { get; set; }
    }
}