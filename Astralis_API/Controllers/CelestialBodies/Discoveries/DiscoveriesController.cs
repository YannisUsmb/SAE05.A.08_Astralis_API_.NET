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
    [DisplayName("Discovery")]
    public class DiscoveriesController : CrudController<Discovery, DiscoveryDto, DiscoveryDto, DiscoveryCreateDto, DiscoveryUpdateDto, int>
    {
        private readonly IDiscoveryRepository _discoveryRepository;

        private readonly IAsteroidRepository _asteroidRepository;
        private readonly IPlanetRepository _planetRepository;
        private readonly IStarRepository _starRepository;
        private readonly ICometRepository _cometRepository;
        private readonly IGalaxyQuasarRepository _galaxyRepository;

        public DiscoveriesController(
            IDiscoveryRepository repository,
            IAsteroidRepository asteroidRepository,
            IPlanetRepository planetRepository,
            IStarRepository starRepository,
            ICometRepository cometRepository,
            IGalaxyQuasarRepository galaxyRepository,
            IMapper mapper)
            : base(repository, mapper)
        {
            _discoveryRepository = repository;
            _asteroidRepository = asteroidRepository;
            _planetRepository = planetRepository;
            _starRepository = starRepository;
            _cometRepository = cometRepository;
            _galaxyRepository = galaxyRepository;
        }

        /// <summary>
        /// Retrieves all discoveries (public access).
        /// </summary>
        /// <returns>A list of discoveries.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<DiscoveryDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific discovery by ID (public access).
        /// </summary>
        /// <param name="id">Discovery ID.</param>
        /// <returns>The discovery details.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<DiscoveryDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for discoveries based on title, status, or approvals.
        /// </summary>
        /// <param name="filter">Search criteria.</param>
        /// <returns>A list of matching discoveries.</returns>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DiscoveryDto>>> Search([FromQuery] DiscoveryFilterDto filter)
        {
            IEnumerable<Discovery?> discoveries = await _discoveryRepository.SearchAsync(
                filter.Title,
                filter.DiscoveryStatusId,
                filter.AliasStatusId,
                filter.DiscoveryApprovalUserId,
                filter.AliasApprovalUserId
            );

            return Ok(_mapper.Map<IEnumerable<DiscoveryDto>>(discoveries));
        }

        // --- SUBMISSION ENDPOINTS (POLYMORPHIC CREATION) ---

        /// <summary>
        /// Submits a new Asteroid discovery.
        /// </summary>
        /// <param name="submission">The discovery title and asteroid details.</param>
        /// <returns>The created discovery.</returns>
        /// <response code="200">Discovery submitted successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User not authenticated.</response>
        [HttpPost("Asteroid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiscoveryDto>> PostAsteroid(DiscoveryAsteroidSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<Asteroid, AsteroidCreateDto>(
                submission.Title,
                submission.Details,
                _asteroidRepository.AddAsync
            );
        }

        /// <summary>
        /// Submits a new Planet discovery.
        /// </summary>
        /// <param name="submission">The discovery title and planet details.</param>
        /// <returns>The created discovery.</returns>
        [HttpPost("Planet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiscoveryDto>> PostPlanet(DiscoveryPlanetSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<Planet, PlanetCreateDto>(
                submission.Title,
                submission.Details,
                _planetRepository.AddAsync
            );
        }

        /// <summary>
        /// Submits a new Star discovery.
        /// </summary>
        /// <param name="submission">The discovery title and star details.</param>
        /// <returns>The created discovery.</returns>
        [HttpPost("Star")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiscoveryDto>> PostStar(DiscoveryStarSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<Star, StarCreateDto>(
                submission.Title,
                submission.Details,
                _starRepository.AddAsync
            );
        }

        /// <summary>
        /// Submits a new Comet discovery.
        /// </summary>
        /// <param name="submission">The discovery title and comet details.</param>
        /// <returns>The created discovery.</returns>
        [HttpPost("Comet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiscoveryDto>> PostComet(DiscoveryCometSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<Comet, CometCreateDto>(
                submission.Title,
                submission.Details,
                _cometRepository.AddAsync
            );
        }

        /// <summary>
        /// Submits a new Galaxy or Quasar discovery.
        /// </summary>
        /// <param name="submission">The discovery title and galaxy details.</param>
        /// <returns>The created discovery.</returns>
        [HttpPost("GalaxyQuasar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiscoveryDto>> PostGalaxyQuasar(DiscoveryGalaxyQuasarSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<GalaxyQuasar, GalaxyQuasarCreateDto>(
                submission.Title,
                submission.Details,
                _galaxyRepository.AddAsync
            );
        }

        /// <summary>
        /// Generic creation endpoint (Disable direct use).
        /// </summary>
        /// <remarks>Please use specific endpoints (e.g., POST api/Discoveries/Asteroid) instead.</remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override Task<ActionResult<DiscoveryDto>> Post(DiscoveryCreateDto createDto)
        {
            return Task.FromResult<ActionResult<DiscoveryDto>>(
                BadRequest("Please use the specific endpoints (e.g., /api/Discoveries/Asteroid) to submit a discovery.")
            );
        }

        /// <summary>
        /// Updates a discovery title (Owner only).
        /// </summary>
        /// <param name="id">Discovery ID.</param>
        /// <param name="updateDto">New title.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> Put(int id, DiscoveryUpdateDto updateDto)
        {
            Discovery? entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId) || entity.UserId != userId)
            {
                return Forbid();
            }

            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes a discovery (Admin/Moderator only).
        /// </summary>
        /// <remarks>Regular users cannot delete a discovery once submitted.</remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Moderator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }

        // --- MODERATION ENDPOINT ---

        /// <summary>
        /// Updates the status of a discovery (Admin/Moderator only).
        /// </summary>
        /// <param name="id">Discovery ID.</param>
        /// <param name="moderationDto">New status details.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}/Status")]
        [Authorize(Roles = "Admin,Moderator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ModerateDiscovery(int id, DiscoveryModerationDto moderationDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Discovery? entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            string? adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(adminIdString, out int adminId))
            {
                // On enregistre qui a validé/refusé
                entity.DiscoveryApprovalUserId = adminId;
                // Si le statut d'alias change, on enregistre aussi (simplification)
                if (moderationDto.AliasStatusId != entity.AliasStatusId)
                {
                    entity.AliasApprovalUserId = adminId;
                }
            }

            // Mise à jour manuelle des statuts
            entity.DiscoveryStatusId = moderationDto.DiscoveryStatusId;
            entity.AliasStatusId = moderationDto.AliasStatusId;

            await _repository.UpdateAsync(entity, entity);

            return NoContent();
        }

        // --- HELPER METHOD FOR POLYMORPHISM ---
        // Cette méthode privée évite de dupliquer la logique pour chaque type d'astre
        private async Task<ActionResult<DiscoveryDto>> ProcessDiscoverySubmission<TEntity, TCreateDto>(
            string title,
            TCreateDto detailsDto,
            Func<TEntity, Task> addMethod)
            where TEntity : class
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            // 1. Mapper et Créer l'astre (Cela va créer le CelestialBody parent grâce à EF Core)
            TEntity celestialEntity = _mapper.Map<TEntity>(detailsDto);
            await addMethod(celestialEntity);

            // Note: Après le AddAsync, celestialEntity devrait avoir un ID et 
            // surtout son parent CelestialBody devrait avoir été généré avec un ID.
            // Il faut parfois récupérer l'ID du CelestialBody généré.
            // On assume ici que la propriété CelestialBodyId de l'enfant est remplie après le SaveChanges interne au repo.

            // Pour être sûr d'avoir l'ID du parent, on utilise la réflexion ou on suppose que le repo a fait le job.
            // Dans ton modèle, Asteroid a "int CelestialBodyId". EF le remplit automatiquement.
            int celestialBodyId = (int)celestialEntity.GetType().GetProperty("CelestialBodyId")!.GetValue(celestialEntity)!;

            // 2. Créer la Découverte
            Discovery discovery = new Discovery
            {
                Title = title,
                UserId = userId,
                CelestialBodyId = celestialBodyId,
                DiscoveryStatusId = 2,
                AliasStatusId = null,
                DiscoveryApprovalUserId = null,
                AliasApprovalUserId = null
            };

            await _repository.AddAsync(discovery);

            // 3. Retourner le DTO
            return Ok(_mapper.Map<DiscoveryDto>(discovery));
        }
    }
}