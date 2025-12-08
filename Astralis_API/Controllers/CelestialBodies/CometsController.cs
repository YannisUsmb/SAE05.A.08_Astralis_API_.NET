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
    [DisplayName("Comet")]
    public class CometsController : CrudController<Comet, CometDto, CometDto, CometCreateDto, CometUpdateDto, int>
    {
        private readonly ICometRepository _cometRepository;
        private readonly IDiscoveryRepository _discoveryRepository;

        public CometsController(
            ICometRepository repository,
            IDiscoveryRepository discoveryRepository,
            IMapper mapper)
            : base(repository, mapper)
        {
            _cometRepository = repository;
            _discoveryRepository = discoveryRepository;
        }

        // --- READ OPERATIONS ---

        /// <summary>
        /// Retrieves all comets (public access).
        /// </summary>
        /// <returns>A list of all comets.</returns>
        /// <response code="200">Returns the list of comets.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<CometDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific comet by ID (public access).
        /// </summary>
        /// <param name="id">The comet ID.</param>
        /// <returns>The detailed comet.</returns>
        /// <response code="200">Returns the comet.</response>
        /// <response code="404">Comet not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<CometDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for comets based on orbital parameters.
        /// </summary>
        /// <param name="filter">The search criteria (eccentricity, inclination, distances).</param>
        /// <returns>A list of matching comets.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CometDto>>> Search([FromQuery] CometFilterDto filter)
        {
            IEnumerable<Comet?> comets = await _cometRepository.SearchAsync(
                filter.Reference,
                filter.MinEccentricity, filter.MaxEccentricity,
                filter.MinInclination, filter.MaxInclination,
                filter.MinPerihelionAU, filter.MaxPerihelionAU,
                filter.MinAphelionAU, filter.MaxAphelionAU,
                filter.MinOrbitalPeriod, filter.MaxOrbitalPeriod,
                filter.MinMOID, filter.MaxMOID
            );

            return Ok(_mapper.Map<IEnumerable<CometDto>>(comets));
        }

        /// <summary>
        /// Generic creation is disabled.
        /// </summary>
        /// <remarks>
        /// You cannot create a Comet directly via this endpoint. 
        /// Please use the polymorphic endpoint 'POST /api/Discoveries/Comet' to submit a discovery dossier.
        /// </remarks>
        /// <param name="createDto">The comet creation data.</param>
        /// <returns>400 Bad Request.</returns>
        /// <response code="400">Creation is not allowed on this endpoint.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override Task<ActionResult<CometDto>> Post(CometCreateDto createDto)
        {
            return Task.FromResult<ActionResult<CometDto>>(
                BadRequest("Cannot create a Comet directly. Use 'POST /api/Discoveries/Comet' to submit a discovery.")
            );
        }

        /// <summary>
        /// Updates a comet's scientific data.
        /// </summary>
        /// <param name="id">The unique identifier of the comet.</param>
        /// <param name="updateDto">The updated scientific data.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The comet was successfully updated.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to edit this comet.</response>
        /// <response code="404">Comet not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, CometUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Comet? comet = await _cometRepository.GetByIdAsync(id);
            if (comet == null) return NotFound();

            if (!await CanEditOrDeleteAsync(comet)) return Forbid();

            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes a comet.
        /// </summary>
        /// <param name="id">The unique identifier of the comet.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The comet was successfully deleted.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to delete this comet.</response>
        /// <response code="404">Comet not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id)
        {
            Comet? comet = await _cometRepository.GetByIdAsync(id);
            if (comet == null) return NotFound();

            if (!await CanEditOrDeleteAsync(comet)) return Forbid();

            return await base.Delete(id);
        }

        // --- HELPER SECURITY ---
        private async Task<bool> CanEditOrDeleteAsync(Comet comet)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId))
                return false;

            if (userRole == "Admin")
                return true;

            var allDiscoveries = await _discoveryRepository.GetAllAsync();
            var discovery = allDiscoveries.FirstOrDefault(d => d.CelestialBodyId == comet.CelestialBodyId);

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