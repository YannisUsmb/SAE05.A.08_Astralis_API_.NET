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
    [DisplayName("Galaxy Quasar")]
    public class GalaxyQuasarsController : CrudController<GalaxyQuasar, GalaxyQuasarDto, GalaxyQuasarDto, GalaxyQuasarCreateDto, GalaxyQuasarUpdateDto, int>
    {
        private readonly IGalaxyQuasarRepository _galaxyQuasarRepository;
        private readonly IDiscoveryRepository _discoveryRepository;

        public GalaxyQuasarsController(
            IGalaxyQuasarRepository repository,
            IDiscoveryRepository discoveryRepository,
            IMapper mapper)
            : base(repository, mapper)
        {
            _galaxyQuasarRepository = repository;
            _discoveryRepository = discoveryRepository;
        }

        /// <summary>
        /// Retrieves all galaxies and quasars (public access).
        /// </summary>
        /// <returns>A list of all galaxies and quasars.</returns>
        /// <response code="200">Returns the list of objects.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<GalaxyQuasarDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific galaxy or quasar by ID (public access).
        /// </summary>
        /// <param name="id">The object ID.</param>
        /// <returns>The detailed galaxy/quasar.</returns>
        /// <response code="200">Returns the object.</response>
        /// <response code="404">Object not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<GalaxyQuasarDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for galaxies/quasars based on scientific parameters.
        /// </summary>
        /// <param name="filter">The search criteria (redshift, magnitude, coordinates).</param>
        /// <returns>A list of matching objects.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<GalaxyQuasarDto>>> Search([FromQuery] GalaxyQuasarFilterDto filter)
        {
            IEnumerable<GalaxyQuasar?> results = await _galaxyQuasarRepository.SearchAsync(
                filter.Reference,
                filter.GalaxyQuasarClassIds,
                filter.MinRightAscension, filter.MaxRightAscension,
                filter.MinDeclination, filter.MaxDeclination,
                filter.MinRedshift, filter.MaxRedshift,
                filter.MinRMagnitude, filter.MaxRMagnitude,
                filter.MinMjdObs, filter.MaxMjdObs
            );

            return Ok(_mapper.Map<IEnumerable<GalaxyQuasarDto>>(results));
        }

        /// <summary>
        /// Generic creation is disabled.
        /// </summary>
        /// <remarks>
        /// You cannot create a Galaxy/Quasar directly via this endpoint. 
        /// Please use the polymorphic endpoint 'POST /api/Discoveries/Galaxy' to submit a discovery dossier.
        /// </remarks>
        /// <param name="createDto">The creation data.</param>
        /// <returns>400 Bad Request.</returns>
        /// <response code="400">Creation is not allowed on this endpoint.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override Task<ActionResult<GalaxyQuasarDto>> Post(GalaxyQuasarCreateDto createDto)
        {
            return Task.FromResult<ActionResult<GalaxyQuasarDto>>(
                BadRequest("Cannot create a Galaxy/Quasar directly. Use 'POST /api/Discoveries/Galaxy' to submit a discovery.")
            );
        }

        /// <summary>
        /// Updates a galaxy/quasar's scientific data.
        /// </summary>
        /// <param name="id">The unique identifier of the object.</param>
        /// <param name="updateDto">The updated scientific data.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The object was successfully updated.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to edit this object.</response>
        /// <response code="404">Object not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, GalaxyQuasarUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            GalaxyQuasar? entity = await _galaxyQuasarRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            if (!await CanEditOrDeleteAsync(entity)) return Forbid();

            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes a galaxy or quasar.
        /// </summary>
        /// <param name="id">The unique identifier of the object.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The object was successfully deleted.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to delete this object.</response>
        /// <response code="404">Object not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id)
        {
            GalaxyQuasar? entity = await _galaxyQuasarRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            if (!await CanEditOrDeleteAsync(entity)) return Forbid();

            return await base.Delete(id);
        }

        // --- HELPER SECURITY ---
        private async Task<bool> CanEditOrDeleteAsync(GalaxyQuasar entity)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId))
                return false;

            if (userRole == "Admin")
                return true;

            var allDiscoveries = await _discoveryRepository.GetAllAsync();
            var discovery = allDiscoveries.FirstOrDefault(d => d.CelestialBodyId == entity.CelestialBodyId);

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