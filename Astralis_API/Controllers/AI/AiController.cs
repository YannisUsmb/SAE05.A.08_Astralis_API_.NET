using Astralis.Shared.DTOs;
using Astralis_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DisplayName("Artificial Intelligence")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("Predict")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<PredictionResultDto>> PredictImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucune image fournie.");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Le fichier doit être une image.");

            var result = await _aiService.PredictImageAsync(file);

            if (result == null)
                return StatusCode(502, "Impossible d'obtenir une prédiction de l'IA.");

            return Ok(result);
        }
    }
}