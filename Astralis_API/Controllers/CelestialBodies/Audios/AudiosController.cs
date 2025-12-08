using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DisplayName("Audio")]
    public class AudiosController : ReadableController<Audio, AudioDto, AudioDto, int>
    {
        private readonly IAudioRepository _audioRepository;

        public AudiosController(IAudioRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _audioRepository = repository;
        }


        /// <summary>
        /// Searches for audios based on text, and celestial body category, or premium status.
        /// </summary>
        /// <param name="filter">The search criteria.</param>
        /// <returns>A list of matching audios.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AudioDto>>> Search([FromQuery] AudioFilterDto filter)
        {
            IEnumerable<Audio?> audios = await _audioRepository.SearchAsync(filter.SearchTerm, filter.CelestialBodyTypeIds);
            return Ok(_mapper.Map<IEnumerable<AudioDto>>(audios));
        }
    }
}