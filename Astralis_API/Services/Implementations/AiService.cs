using Astralis.Shared.DTOs;
using Astralis_API.Services.Interfaces;
using System.Net.Http.Headers;

namespace Astralis_API.Services.Implementations
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiService> _logger;

        public AiService(HttpClient httpClient, IConfiguration config, ILogger<AiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var baseUrl = config["ExternalApis:AiApiUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        public async Task<PredictionResultDto?> PredictImageAsync(IFormFile imageFile)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                using var stream = imageFile.OpenReadStream();
                using var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);

                content.Add(streamContent, "file", imageFile.FileName);

                var response = await _httpClient.PostAsync("api/v1/predict-image", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PredictionResultDto>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Erreur API Python: {response.StatusCode} - {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception lors de l'appel AI: {ex.Message}");
                throw;
            }
        }
    }
}