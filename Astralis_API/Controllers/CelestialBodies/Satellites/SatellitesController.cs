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
    [DisplayName("Satellite")]
    public class SatellitesController : CrudController<Satellite, SatelliteDto, SatelliteDto, SatelliteCreateDto, SatelliteUpdateDto, int>
    {
        private readonly ISatelliteRepository _satelliteRepository;
        private readonly IDiscoveryRepository _discoveryRepository;

        public SatellitesController(
            ISatelliteRepository repository,
            IDiscoveryRepository discoveryRepository,
            IMapper mapper)
            : base(repository, mapper)
        {
            _satelliteRepository = repository;
            _discoveryRepository = discoveryRepository;
        }

        /// <summary>
        /// Retrieves all satellites (public access).
        /// </summary>
        /// <returns>A list of all satellites.</returns>
        /// <response code="200">Returns the list of satellites.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<SatelliteDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific satellite by ID (public access).
        /// </summary>
        /// <param name="id">The satellite ID.</param>
        /// <returns>The detailed satellite.</returns>
        /// <response code="200">Returns the satellite.</response>
        /// <response code="404">Satellite not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<SatelliteDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for satellites based on physical characteristics and host planet.
        /// </summary>
        /// <param name="filter">The search criteria (gravity, radius, density, planet).</param>
        /// <returns>A list of matching satellites.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<SatelliteDto>>> Search([FromQuery] SatelliteFilterDto filter)
        {
            IEnumerable<Satellite?> satellites = await _satelliteRepository.SearchAsync(
                filter.Name,
                filter.PlanetIds,
                filter.MinGravity, filter.MaxGravity,
                filter.MinRadius, filter.MaxRadius,
                filter.MinDensity, filter.MaxDensity
            );

            return Ok(_mapper.Map<IEnumerable<SatelliteDto>>(satellites));
        }

        /// <summary>
        /// Generic creation is disabled.
        /// </summary>
        /// <remarks>
        /// You cannot create a Satellite directly via this endpoint. 
        /// Please use the polymorphic endpoint 'POST /api/Discoveries/Satellite' (if available) or related discovery processes.
        /// </remarks>
        /// <param name="createDto">The satellite creation data.</param>
        /// <returns>400 Bad Request.</returns>
        /// <response code="400">Creation is not allowed on this endpoint.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override Task<ActionResult<SatelliteDto>> Post(SatelliteCreateDto createDto)
        {
            return Task.FromResult<ActionResult<SatelliteDto>>(
                BadRequest("Cannot create a Satellite directly. Use the Discovery process.")
            );
        }

        /// <summary>
        /// Updates a satellite's scientific data.
        /// </summary>
        /// <param name="id">The unique identifier of the satellite.</param>
        /// <param name="updateDto">The updated scientific data.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The satellite was successfully updated.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to edit this satellite.</response>
        /// <response code="404">Satellite not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, SatelliteUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Satellite? satellite = await _satelliteRepository.GetByIdAsync(id);
            if (satellite == null)
                return NotFound();

            if (!await CanEditOrDeleteAsync(satellite))
                return Forbid();

            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes a satellite.
        /// </summary>
        /// <remarks>
        /// Security Rules:
        /// <list type="bullet">
        /// <item><strong>Admins:</strong> Can delete any satellite at any time.</item>
        /// <item><strong>Owner (User):</strong> Can delete only if the associated Discovery is still a Draft or declined (Status 1 or 4).</item>
        /// </list>
        /// </remarks>
        /// <param name="id">The unique identifier of the satellite.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The satellite was successfully deleted.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to delete this satellite.</response>
        /// <response code="404">Satellite not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id)
        {
            Satellite? satellite = await _satelliteRepository.GetByIdAsync(id);
            if (satellite == null)
                return NotFound();

            if (!await CanEditOrDeleteAsync(satellite))
                return Forbid();

            return await base.Delete(id);
        }

        // --- HELPER SECURITY ---
        private async Task<bool> CanEditOrDeleteAsync(Satellite satellite)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId))
                return false;

            if (userRole == "Admin")
                return true;

            var allDiscoveries = await _discoveryRepository.GetAllAsync();
            var discovery = allDiscoveries.FirstOrDefault(d => d.CelestialBodyId == satellite.CelestialBodyId);

            if (discovery == null)
                return false;

            if (discovery.UserId == userId && (discovery.DiscoveryStatusId == 1 || discovery.DiscoveryStatusId == 4))
            {
                return true;
            }

            return false;
        }
    }
}