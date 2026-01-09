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
    [Authorize]
    [DisplayName("Celestial Body")]
    public class CelestialBodiesController : CrudController<CelestialBody, CelestialBodyListDto, CelestialBodyListDto, CelestialBodyCreateDto, CelestialBodyUpdateDto, int>
    {
        private readonly ICelestialBodyRepository _celestialBodyRepository;

        public CelestialBodiesController(ICelestialBodyRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _celestialBodyRepository = repository;
        }

        /// <summary>
        /// Retrieves all celestial bodies registered in the database (public access).
        /// </summary>
        /// <returns>A list of celestial bodies with their types.</returns>
        /// <response code="200">The list was successfully retrieved.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<CelestialBodyListDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific celestial body by its unique identifier (public access).
        /// </summary>
        /// <param name="id">The unique identifier of the celestial body.</param>
        /// <returns>The requested celestial body details.</returns>
        /// <response code="200">The celestial body was found.</response>
        /// <response code="404">The celestial body does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<CelestialBodyListDto>> GetById(int id)
        {
            return base.GetById(id);
        }
        
        /// <summary>
        /// Retrieves detailed information about a specific celestial body by its unique identifier (public access).
        /// Includes all type-specific properties (Planet, Star, Asteroid, etc.).
        /// </summary>
        /// <param name="id">The unique identifier of the celestial body.</param>
        /// <returns>The complete celestial body details including type-specific information.</returns>
        /// <response code="200">The celestial body was found.</response>
        /// <response code="404">The celestial body does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet("{id}/Details")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CelestialBodyDetailDto>> GetDetails(int id)
        {
            CelestialBody? entity = await _celestialBodyRepository.GetByIdAsync(id);
    
            if (entity == null)
            {
                return NotFound($"Celestial body with ID {id} not found.");
            }

            CelestialBodyDetailDto detailDto = _mapper.Map<CelestialBodyDetailDto>(entity);
            return Ok(detailDto);
        }
        
        /// <summary>
        /// Retrieves the subtypes associated with a specific main celestial body type.
        /// </summary>
        /// <param name="mainTypeId">The ID of the main type (e.g., 1 for Planet, 2 for Star).</param>
        /// <returns>A list of subtypes (ID and Label).</returns>
        /// <response code="200">The subtypes were retrieved successfully.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet("Subtypes/{mainTypeId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CelestialBodySubtypeDto>>> GetSubtypes(int mainTypeId)
        {
            var subtypesDto = await _celestialBodyRepository.GetSubtypesByMainTypeAsync(mainTypeId);
            return Ok(subtypesDto);
        }

        /// <summary>
        /// Searches for celestial bodies based on name, specific types, or discovery status.
        /// </summary>
        /// <param name="filter">The search criteria (text, types, isDiscovery).</param>
        /// <returns>A list of matching celestial bodies.</returns>
        /// <response code="200">The search results were retrieved successfully.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPost("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CelestialBodyListDto>>> Search(
            [FromBody] CelestialBodyFilterDto filter,
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 30) 
        {
            IEnumerable<CelestialBody> results = await _celestialBodyRepository.SearchAsync(
                filter,
                pageNumber,
                pageSize   
            );

            return Ok(_mapper.Map<IEnumerable<CelestialBodyListDto>>(results));
        }

        /// <summary>
        /// Generic creation is disabled to enforce strict typing.
        /// </summary>
        /// <remarks>
        /// You cannot create a generic "Celestial Body". 
        /// Please use specific endpoints (e.g., POST /api/Discoveries/Planet) to ensure data integrity.
        /// </remarks>
        /// <param name="createDto">The generic creation DTO.</param>
        /// <returns>400 Bad Request.</returns>
        /// <response code="400">Creation is not allowed on this endpoint.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override Task<ActionResult<CelestialBodyListDto>> Post(CelestialBodyCreateDto createDto)
        {
            return Task.FromResult<ActionResult<CelestialBodyListDto>>(
                BadRequest("Cannot create a generic Celestial Body directly. Please use specific endpoints (e.g., /api/Discoveries/Planet or /api/Asteroids) to ensure proper data structure.")
            );
        }

        /// <summary>
        /// Updates a celestial body's generic information (Name, Alias).
        /// </summary>
        /// <remarks>Reserved for Administrators.</remarks>
        /// <param name="id">The ID of the body to update.</param>
        /// <param name="updateDto">The updated information.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The update was successful.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized (requires Admin role).</response>
        /// <response code="404">The celestial body does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<IActionResult> Put(int id, CelestialBodyUpdateDto updateDto)
        {
            return base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes a celestial body and all its specific data.
        /// </summary>
        /// <remarks>Reserved for Administrators. Warning: This cascades to all linked data.</remarks>
        /// <param name="id">The ID of the body to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The deletion was successful.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized (requires Admin role).</response>
        /// <response code="404">The celestial body does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<IActionResult> Delete(int id)
        {
            return base.Delete(id);
        }
    }
}