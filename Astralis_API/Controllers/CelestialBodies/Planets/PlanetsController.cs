using Astralis.Shared.DTOs;
using Astralis_API.Configuration;
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
    [DisplayName("Planet")]
    public class PlanetsController : CrudController<Planet, PlanetDto, PlanetDto, PlanetCreateDto, PlanetUpdateDto, int>
    {
        private readonly IPlanetRepository _planetRepository;
        private readonly IDiscoveryRepository _discoveryRepository;

        public PlanetsController(
            IPlanetRepository repository,
            IDiscoveryRepository discoveryRepository,
            IMapper mapper)
            : base(repository, mapper)
        {
            _planetRepository = repository;
            _discoveryRepository = discoveryRepository;
        }

        /// <summary>
        /// Retrieves all planets (public access).
        /// </summary>
        /// <returns>A list of planets.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<PlanetDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific planet by ID (public access).
        /// </summary>
        /// <param name="id">The planet ID.</param>
        /// <returns>The detailed planet.</returns>
        /// <response code="200">Returns the planet.</response>
        /// <response code="404">Planet not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<PlanetDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for planets using physical and orbital criteria.
        /// </summary>
        /// <param name="filter">Search filters (Name, Mass, Temp, etc.).</param>
        /// <returns>A list of matching planets.</returns>
        /// <response code="200">Search successful.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PlanetDto>>> Search([FromQuery] PlanetFilterDto filter)
        {
            var planets = await _planetRepository.SearchAsync(
                filter.Name,
                filter.PlanetTypeIds,
                filter.DetectionMethodIds,
                filter.MinDistance, filter.MaxDistance,
                filter.MinMass, filter.MaxMass,
                filter.MinRadius, filter.MaxRadius,
                filter.MinDiscoveryYear, filter.MaxDiscoveryYear,
                filter.MinEccentricity, filter.MaxEccentricity,
                filter.MinStellarMagnitude, filter.MaxStellarMagnitude
            );

            return Ok(_mapper.Map<IEnumerable<PlanetDto>>(planets));
        }

        // --- WRITE OPERATIONS ---

        /// <summary>
        /// Generic creation is disabled.
        /// </summary>
        /// <remarks>
        /// Cannot create a Planet directly. Please use 'POST /api/Discoveries/Planet' to submit a discovery.
        /// </remarks>
        /// <param name="createDto">Creation data.</param>
        /// <returns>400 Bad Request.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override Task<ActionResult<PlanetDto>> Post(PlanetCreateDto createDto)
        {
            return Task.FromResult<ActionResult<PlanetDto>>(
                BadRequest("Cannot create a Planet directly. Use 'POST /api/Discoveries/Planet' to submit a discovery.")
            );
        }

        /// <summary>
        /// Updates an planet's scientific data.
        /// </summary>
        /// <param name="id">The unique identifier of the planet.</param>
        /// <param name="updateDto">The updated scientific data.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The planet was successfully updated.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to edit this planet (not owner or discovery is locked).</response>
        /// <response code="404">Planet not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, PlanetUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Planet? planet = await _planetRepository.GetByIdAsync(id);
            if (planet == null) return NotFound();

            if (!await CanEditOrDeleteAsync(planet)) return Forbid();

            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes an planet.
        /// </summary>
        /// <param name="id">The unique identifier of the planet.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The planet was successfully deleted.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to delete this planet.</response>
        /// <response code="404">Planet not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id)
        {
            Planet? planet = await _planetRepository.GetByIdAsync(id);
            if (planet == null)
                return NotFound();

            if (!await CanEditOrDeleteAsync(planet))
                return Forbid();

            return await base.Delete(id);
        }

        // --- HELPER ---
        private async Task<bool> CanEditOrDeleteAsync(Planet planet)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId)) return false;
            if (userRole == "Admin") return true;

            var allDiscoveries = await _discoveryRepository.GetAllAsync();
            var discovery = allDiscoveries.FirstOrDefault(d => d.CelestialBodyId == planet.CelestialBodyId);

            if (discovery == null) return false;

            if (discovery.UserId == userId && discovery.DiscoveryStatusId == 1)
            {
                return true;
            }

            return false;
        }
    }
}