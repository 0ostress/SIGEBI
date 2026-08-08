namespace SIGEBI.Web.Services
{
    public class ImagenApiService
    {
        private readonly HttpClient _httpClient;

        public ImagenApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SigebiAPI");
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("http://localhost:5200/");
        }

        public async Task<string?> SubirImagenAsync(int recursoId, IFormFile imagen)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var stream = imagen.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imagen.ContentType);
                content.Add(fileContent, "imagen", imagen.FileName);

                var response = await _httpClient.PostAsync($"api/Imagenes/subir/{recursoId}", content);
                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    return resultado?.GetValueOrDefault("imagenUrl");
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}