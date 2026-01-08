using Astralis.Shared.Enums;
using Astralis_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        public MediaController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        /// <summary>
        /// Uploads an image to Azure Blob Storage based on a specific category.
        /// </summary>
        /// <remarks>
        /// The file is stored in a specific container and folder determined by the category (e.g., "Avatars" -> container "avatars", "Planets" -> container "celestial-bodies/planets").
        /// </remarks>
        /// <param name="file">The image file to upload (FormFile).</param>
        /// <param name="category">The category of the image (enum) to determine the storage path.</param>
        /// <returns>A JSON object containing the absolute URL of the uploaded image.</returns>
        /// <response code="200">Image uploaded successfully.</response>
        /// <response code="400">File is empty or category is invalid.</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="500">Internal server error or Azure storage exception.</response>
        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] UploadCategory category)
        {
            if (file == null || file.Length == 0) return BadRequest("Fichier vide.");

            (string container, string folder) = GetStoragePath(category);

            try
            {
                string url = await _uploadService.UploadImageAsync(file, container, folder);
                return Ok(new { Url = url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        private (string container, string folder) GetStoragePath(UploadCategory category)
        {
            return category switch
            {
                UploadCategory.Avatars => ("avatars", ""),
                UploadCategory.Products => ("products", ""),
                UploadCategory.Articles => ("articles", ""),
                UploadCategory.Events => ("events", ""),

                UploadCategory.Asteroids => ("celestial-bodies", "asteroids"),
                UploadCategory.Comets => ("celestial-bodies", "comets"),
                UploadCategory.Galaxies => ("celestial-bodies", "galaxies"),
                UploadCategory.Planets => ("celestial-bodies", "planets"),
                UploadCategory.Quasars => ("celestial-bodies", "quasars"),
                UploadCategory.Satellites => ("celestial-bodies", "satellites"),
                UploadCategory.Stars => ("celestial-bodies", "stars"),

                _ => ("misc", "")
            };
        }
    }
}
