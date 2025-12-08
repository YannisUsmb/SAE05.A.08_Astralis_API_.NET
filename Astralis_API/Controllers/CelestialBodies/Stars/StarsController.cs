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
    [DisplayName("Star")]
    public class StarsController : CrudController<Star, StarDto, StarDto, StarCreateDto, StarUpdateDto, int>
    {
        private readonly IStarRepository _starRepository;
        private readonly IDiscoveryRepository _discoveryRepository;

        public StarsController(
            IStarRepository repository,
            IDiscoveryRepository discoveryRepository,
            IMapper mapper)
            : base(repository, mapper)
        {
            _starRepository = repository;
            _discoveryRepository = discoveryRepository;
        }

        /// <summary>
        /// Retrieves all stars (public access).
        /// </summary>
        /// <returns>A list of all stars.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<StarDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific star by ID (public access).
        /// </summary>
        /// <param name="id">The star ID.</param>
        /// <returns>The detailed star.</returns>
        /// <response code="200">Returns the star.</response>
        /// <response code="404">Star not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<StarDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for stars based on spectral class, constellation, or physical parameters.
        /// </summary>
        /// <param name="filter">The search criteria.</param>
        /// <returns>A list of matching stars.</returns>
        /// <response code="200">Search successful.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StarDto>>> Search([FromQuery] StarFilterDto filter)
        {
            IEnumerable<Star?> stars = await _starRepository.SearchAsync(
                filter.Name,
                filter.SpectralClassIds,
                filter.Constellation,
                filter.Designation,
                filter.BayerDesignation,
                filter.MinDistance, filter.MaxDistance,
                filter.MinLuminosity, filter.MaxLuminosity,
                filter.MinRadius, filter.MaxRadius,
                filter.MinTemperature, filter.MaxTemperature
            );

            return Ok(_mapper.Map<IEnumerable<StarDto>>(stars));
        }

        /// <summary>
        /// Generic creation is disabled.
        /// </summary>
        /// <remarks>
        /// You cannot create a Star directly via this endpoint. 
        /// Please use the polymorphic endpoint 'POST /api/Discoveries/Star' to submit a discovery dossier.
        /// </remarks>
        /// <param name="createDto">The star creation data.</param>
        /// <returns>400 Bad Request.</returns>
        /// <response code="400">Creation is not allowed on this endpoint.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override Task<ActionResult<StarDto>> Post(StarCreateDto createDto)
        {
            return Task.FromResult<ActionResult<StarDto>>(
                BadRequest("Cannot create a Star directly. Use 'POST /api/Discoveries/Star' to submit a discovery.")
            );
        }

        /// <summary>
        /// Updates a star's scientific data.
        /// </summary>
        /// <remarks>
        /// Security Rules:
        /// <list type="bullet">
        /// <item><strong>Admins:</strong> Can update any star at any time.</item>
        /// <item><strong>Owner (User):</strong> Can update only if the associated Discovery is still a Draft or declined (Status 1 or 4).</item>
        /// </list>
        /// </remarks>
        /// <param name="id">The unique identifier of the star.</param>
        /// <param name="updateDto">The updated scientific data.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The star was successfully updated.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to edit this star.</response>
        /// <response code="404">Star not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, StarUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Star? star = await _starRepository.GetByIdAsync(id);
            if (star == null)
                return NotFound();

            if (!await CanEditOrDeleteAsync(star))
                return Forbid();

            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes a star.
        /// </summary>
        /// <remarks>
        /// Security Rules:
        /// <list type="bullet">
        /// <item><strong>Admins:</strong> Can delete any star at any time.</item>
        /// <item><strong>Owner (User):</strong> Can delete only if the associated Discovery is still a Draft or declined (Status 1 or 4).</item>
        /// </list>
        /// </remarks>
        /// <param name="id">The unique identifier of the star.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The star was successfully deleted.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to delete this star.</response>
        /// <response code="404">Star not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id)
        {
            Star? star = await _starRepository.GetByIdAsync(id);
            if (star == null)
                return NotFound();

            if (!await CanEditOrDeleteAsync(star))
                return Forbid();

            return await base.Delete(id);
        }

        // --- HELPER SECURITY ---
        private async Task<bool> CanEditOrDeleteAsync(Star star)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId))
                return false;

            if (userRole == "Admin")
                return true;

            var allDiscoveries = await _discoveryRepository.GetAllAsync();
            var discovery = allDiscoveries.FirstOrDefault(d => d.CelestialBodyId == star.CelestialBodyId);

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