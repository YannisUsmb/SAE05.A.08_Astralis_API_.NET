using Astralis.Shared.DTOs;

namespace Astralis_API.Services.Interfaces
{
    public interface IAiService
    {
        Task<PredictionResultDto?> PredictImageAsync(IFormFile imageFile);
    }
}