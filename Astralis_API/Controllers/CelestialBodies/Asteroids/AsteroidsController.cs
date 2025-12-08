using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("Asteroid")]
    public class AsteroidsController : CrudController<Asteroid, AsteroidDto, AsteroidDto, AsteroidCreateDto, AsteroidUpdateDto, int>
    {
        private readonly IAsteroidRepository _asteroidRepository;
        private readonly IDiscoveryRepository _discoveryRepository;

        public AsteroidsController(
            IAsteroidRepository repository,
            IDiscoveryRepository discoveryRepository,
            IMapper mapper)
            : base(repository, mapper)
        {
            _asteroidRepository = repository;
            _discoveryRepository = discoveryRepository;
        }

        /// <summary>
        /// Retrieves all asteroids (public access).
        /// </summary>
        /// <returns>A list of all asteroids.</returns>
        /// <response code="200">Returns the list of asteroids.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<AsteroidDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific asteroid by ID (public access).
        /// </summary>
        /// <param name="id">The asteroid ID.</param>
        /// <returns>The detailed asteroid.</returns>
        /// <response code="200">Returns the asteroid.</response>
        /// <response code="404">Asteroid not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<AsteroidDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for asteroids based on physical and orbital parameters.
        /// </summary>
        /// <param name="filter">The search criteria (e.g., hazard status, magnitude, diameter).</param>
        /// <returns>A list of matching asteroids.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AsteroidDto>>> Search([FromQuery] AsteroidFilterDto filter)
        {
            IEnumerable<Asteroid?> asteroids = await _asteroidRepository.SearchAsync(
                filter.Reference,
                filter.OrbitalClassIds,
                filter.IsPotentiallyHazardous,
                filter.OrbitId,
                filter.MinAbsoluteMagnitude,
                filter.MaxAbsoluteMagnitude,
                filter.MinDiameter,
                filter.MaxDiameter,
                filter.MinInclination,
                filter.MaxInclination,
                filter.MinSemiMajorAxis,
                filter.MaxSemiMajorAxis
            );

            return Ok(_mapper.Map<IEnumerable<AsteroidDto>>(asteroids));
        }

        /// <summary>
        /// Generic creation is disabled.
        /// </summary>
        /// <remarks>
        /// You cannot create an Asteroid directly via this endpoint. 
        /// Please use the polymorphic endpoint 'POST /api/Discoveries/Asteroid' to submit a discovery dossier.
        /// </remarks>
        /// <param name="createDto">The asteroid creation data.</param>
        /// <returns>400 Bad Request.</returns>
        /// <response code="400">Creation is not allowed on this endpoint.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override Task<ActionResult<AsteroidDto>> Post(AsteroidCreateDto createDto)
        {
            return Task.FromResult<ActionResult<AsteroidDto>>(
                BadRequest("Cannot create an Asteroid directly. Use 'POST /api/Discoveries/Asteroid' to submit a discovery.")
            );
        }

        /// <summary>
        /// Updates an asteroid's scientific data.
        /// </summary>
        /// <param name="id">The unique identifier of the asteroid.</param>
        /// <param name="updateDto">The updated scientific data.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The asteroid was successfully updated.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to edit this asteroid (not owner or discovery is locked).</response>
        /// <response code="404">Asteroid not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, AsteroidUpdateDto updateDto)
        {
            Asteroid? asteroid = await _asteroidRepository.GetByIdAsync(id);
            if (asteroid == null)
            {
                return NotFound();
            }

            if (!await CanEditOrDeleteAsync(asteroid))
            {
                return Forbid();
            }

            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes an asteroid.
        /// </summary>
        /// <param name="id">The unique identifier of the asteroid.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The asteroid was successfully deleted.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to delete this asteroid.</response>
        /// <response code="404">Asteroid not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id)
        {
            Asteroid? asteroid = await _asteroidRepository.GetByIdAsync(id);
            if (asteroid == null)
            {
                return NotFound();
            }

            if (!await CanEditOrDeleteAsync(asteroid))
            {
                return Forbid();
            }

            return await base.Delete(id);
        }

        // --- HELPER SECURITY ---
        private async Task<bool> CanEditOrDeleteAsync(Asteroid asteroid)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId))
                return false;

            if (userRole == "Admin")
                return true;

            var allDiscoveries = await _discoveryRepository.GetAllAsync();
            var discovery = allDiscoveries.FirstOrDefault(d => d.CelestialBodyId == asteroid.CelestialBodyId);

            if (discovery == null)
                return false;

            if (discovery.UserId == userId && discovery.DiscoveryStatusId == 1 && discovery.DiscoveryStatusId == 4)
            {
                return true;
            }

            return false;
        }
    }
}