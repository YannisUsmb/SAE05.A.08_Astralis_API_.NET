using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
        private readonly ICelestialBodyRepository _celestialBodyRepository;
        private readonly ISatelliteRepository _satelliteRepository;
        private readonly IUserRepository _userRepository;
        private readonly AstralisDbContext _context;
        
        public DiscoveriesController(
            IDiscoveryRepository repository,
            IAsteroidRepository asteroidRepository,
            IPlanetRepository planetRepository,
            IStarRepository starRepository,
            ICometRepository cometRepository,
            IGalaxyQuasarRepository galaxyRepository,
            ICelestialBodyRepository celestialBodyRepository,
            ISatelliteRepository satelliteRepository,
            IUserRepository userRepository,
            AstralisDbContext context,
            IMapper mapper)
            : base(repository, mapper)
        {
            _discoveryRepository = repository;
            _asteroidRepository = asteroidRepository;
            _planetRepository = planetRepository;
            _starRepository = starRepository;
            _cometRepository = cometRepository;
            _galaxyRepository = galaxyRepository;
            _celestialBodyRepository = celestialBodyRepository;
            _satelliteRepository = satelliteRepository;
            _userRepository = userRepository;
            _context = context;
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
        public override async Task<ActionResult<IEnumerable<DiscoveryDto>>> GetAll()
        {
            var validatedDiscoveries = await _discoveryRepository.SearchAsync(discoveryStatusId: 3);
    
            return Ok(_mapper.Map<IEnumerable<DiscoveryDto>>(validatedDiscoveries));
        }

        /// <summary>
        /// Retrieves a specific discovery by ID (public access).
        /// </summary>
        /// <param name="id">Discovery ID.</param>
        /// <returns>The discovery details.</returns>
        /// <response code="200">Discovery found.</response>
        /// <response code="404">Discovery not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<DiscoveryDto>> GetById(int id)
        {
            return base.GetById(id);
        }
        
        // GET: api/Discoveries/DetectionMethods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("DetectionMethods")]
        [AllowAnonymous] 
        public async Task<ActionResult<IEnumerable<DetectionMethodDto>>> GetDetectionMethods()
        {
            var methods = await _celestialBodyRepository.GetDetectionMethodsAsync();
            
            return Ok(_mapper.Map<IEnumerable<DetectionMethodDto>>(methods));
        }

        /// <summary>
        /// Searches for discoveries based on title, status, or approvals.
        /// </summary>
        /// <param name="filter">Search criteria.</param>
        /// <returns>A list of matching discoveries.</returns>
        /// <response code="200">Search successful.</response>
        /// <response code="500">Internal server error.</response>
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

        /// <summary>
        /// Submits a new Asteroid discovery.
        /// </summary>
        /// <param name="submission">The discovery title and asteroid details.</param>
        /// <returns>The created discovery.</returns>
        /// <response code="200">Discovery submitted successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("Asteroid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiscoveryDto>> PostAsteroid(DiscoveryAsteroidSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<Asteroid, AsteroidCreateDto>(
                submission.Title, submission.Details, _asteroidRepository.AddAsync, 3); // 3 = Astéroïde
        }

        /// <summary>
        /// Submits a new Planet discovery.
        /// </summary>
        /// <param name="submission">The discovery title and Planet details.</param>
        /// <returns>The created discovery.</returns>
        /// <response code="200">Discovery submitted successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("Planet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiscoveryDto>> PostPlanet(DiscoveryPlanetSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<Planet, PlanetCreateDto>(
                submission.Title, submission.Details, _planetRepository.AddAsync, 2); // 2 = Planète
        }

        /// <summary>
        /// Submits a new Star discovery.
        /// </summary>
        /// <param name="submission">The discovery title and Star details.</param>
        /// <returns>The created discovery.</returns>
        /// <response code="200">Discovery submitted successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("Star")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiscoveryDto>> PostStar(DiscoveryStarSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<Star, StarCreateDto>(
                submission.Title, submission.Details, _starRepository.AddAsync, 1); // 1 = Étoile
        }

        /// <summary>
        /// Submits a new Comet discovery.
        /// </summary>
        /// <param name="submission">The discovery title and Comet details.</param>
        /// <returns>The created discovery.</returns>
        /// <response code="200">Discovery submitted successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("Comet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiscoveryDto>> PostComet(DiscoveryCometSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<Comet, CometCreateDto>(
                submission.Title, submission.Details, _cometRepository.AddAsync, 4); // 4 = Comète
        }

        /// <summary>
        /// Submits a new GalaxyQuasar discovery.
        /// </summary>
        /// <param name="submission">The discovery title and GalaxyQuasar details.</param>
        /// <returns>The created discovery.</returns>
        /// <response code="200">Discovery submitted successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("GalaxyQuasar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiscoveryDto>> PostGalaxy(DiscoveryGalaxyQuasarSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<GalaxyQuasar, GalaxyQuasarCreateDto>(
                submission.Title, submission.Details, _galaxyRepository.AddAsync, 5); // 5 = Galaxie/Quasar
        }
        /// <summary>
        /// Submits a new Satellite discovery.
        /// </summary>
        /// <param name="submission">The discovery title and Satellite details.</param>
        /// <returns>The created discovery.</returns>
        /// <response code="200">Discovery submitted successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("Satellite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiscoveryDto>> PostSatellite(DiscoverySatelliteSubmissionDto submission)
        {
            return await ProcessDiscoverySubmission<Satellite, SatelliteCreateDto>(
                submission.Title, submission.Details, _satelliteRepository.AddAsync, 6); // 6 = Satellite
        }

        /// <summary>
        /// Generic creation endpoint (Disabled).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override Task<ActionResult<DiscoveryDto>> Post(DiscoveryCreateDto createDto)
        {
            return Task.FromResult<ActionResult<DiscoveryDto>>(
                BadRequest("Please use the specific endpoints (e.g., /api/Discoveries/Asteroid) to submit a discovery.")
            );
        }

        /// <summary>
        /// Updates a discovery title.
        /// </summary>
        /// <remarks>The owner can update the title only if the discovery is still a Draft (Status 1).</remarks>
        /// <param name="id">Discovery ID.</param>
        /// <param name="updateDto">New title.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Update successful.</response>
        /// <response code="400">Invalid input or Discovery is not a Draft.</response>
        /// <response code="403">User is not the owner.</response>
        /// <response code="404">Discovery not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> Put(int id, DiscoveryUpdateDto updateDto)
        {
            Discovery? entity = await _discoveryRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            if (entity.UserId != userId)
            {
                return Forbid();
            }

            if (entity.DiscoveryStatusId != 1)
            {
                return BadRequest("You can only modify a draft discovery.");
            }

            await base.Put(id, updateDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a discovery.
        /// </summary>
        /// <remarks>
        /// Admins can delete any discovery. 
        /// Owners can delete only if the discovery is a Draft or declined (Status 1 - 4).
        /// </remarks>
        /// <param name="id">Discovery ID.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Delete successful.</response>
        /// <response code="400">Invalid input or Discovery is not a Draft or declined.</response>
        /// <response code="403">User is not the owner.</response>
        /// <response code="404">Discovery not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> Delete(int id)
        {
            Discovery? entity = await _discoveryRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            if (entity.UserId == userId)
            {
                if (entity.DiscoveryStatusId != 1 && entity.DiscoveryStatusId != 4)
                {
                    return BadRequest("You can only delete a draft or declined discovery.");
                }
            }
            else if (userRole != "Admin")
            {
                return Forbid();
            }

            return await base.Delete(id);
        }

        /// <summary>
        /// Proposes an alias for an accepted discovery.
        /// Premium users: Free. Standard users: Mock payment check.
        /// </summary>
        /// <remarks>Only the owner can propose an alias for an Accepted discovery (Status 3).</remarks>
        /// <param name="id">The unique identifier of the discovery.</param>
        /// <param name="aliasDto">The proposed alias details.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The alias proposal was submitted successfully.</response>
        /// <response code="400">Invalid input or the discovery is not in 'Accepted' status.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not the owner of this discovery.</response>
        /// <response code="404">Discovery not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}/Alias")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProposeAlias(int id, DiscoveryAliasDto aliasDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Discovery? entity = await _discoveryRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            // 1. Verify ownership
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();
            if (entity.UserId != userId) return Forbid();

            // 2. Discovery status check (must be Accepted)
            if (entity.DiscoveryStatusId != 3)
                return BadRequest("La proposition d'alias n'est autorisée que pour les découvertes acceptées.");

            // 3. Vérification Premium
            User? user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return Unauthorized();

            if (!user.IsPremium)
            {
                // There would be a payment processing step here in a real application.
                return StatusCode(StatusCodes.Status402PaymentRequired, "La proposition d'alias est reservée aux utilisateurs Premium ou nécessite un paiement immédiat.");
            }

            // 4. Update the CelestialBody alias and set alias status to 'Pending Approval'
            CelestialBody? celestialBody = await _celestialBodyRepository.GetByIdAsync(entity.CelestialBodyId);
            if (celestialBody != null)
            {
                celestialBody.Alias = aliasDto.Alias;
                await _celestialBodyRepository.UpdateAsync(celestialBody, celestialBody);
            }

            entity.AliasStatusId = 1; 
            entity.AliasApprovalUserId = null;

            await _repository.UpdateAsync(entity, entity);

            return NoContent();
        }
        
        /// <summary>
        /// Removes an alias from a discovery (User Action).
        /// </summary>
        [HttpDelete("{id}/Alias")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveAlias(int id)
        {
            Discovery? entity = await _discoveryRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            // Only the owner can remove the alias 
            if (entity.UserId != userId) return Forbid();

            // Remove alias from CelestialBody
            CelestialBody? celestialBody = await _celestialBodyRepository.GetByIdAsync(entity.CelestialBodyId);
            if (celestialBody != null)
            {
                celestialBody.Alias = null;
                await _celestialBodyRepository.UpdateAsync(celestialBody, celestialBody);
            }

            // Reset alias status
            entity.AliasStatusId = null;
            entity.AliasApprovalUserId = null;

            await _repository.UpdateAsync(entity, entity);

            return NoContent();
        }

        /// <summary>
        /// Moderates an alias (Admin only).
        /// Accepts (Status 2) or Refuses (Status 3).
        /// Uses DiscoveryModerationDto to reuse existing DTO.
        /// </summary>
        [HttpPut("{id}/Alias/Moderate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModerateAlias(int id, DiscoveryModerationDto moderationDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Discovery? entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            if (entity.AliasStatusId != 1)
                return BadRequest("Cette découverte n'a pas d'alias en attente de modération.");

            string? adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(adminIdString, out int adminId))
            {
                entity.AliasApprovalUserId = adminId;
            }

            entity.AliasStatusId = moderationDto.AliasStatusId;

            // Si l'alias est refusé, le supprimer du corps céleste
            if (moderationDto.AliasStatusId == 3)
            {
                CelestialBody? celestialBody = await _celestialBodyRepository.GetByIdAsync(entity.CelestialBodyId);
                if (celestialBody != null)
                {
                    celestialBody.Alias = null; 
                    await _celestialBodyRepository.UpdateAsync(celestialBody, celestialBody);
                }
            }

            await _repository.UpdateAsync(entity, entity);

            return NoContent();
        }
        
        /// <summary>
        /// Updates the status of a discovery (Admin only).
        /// </summary>
        /// <param name="id">The unique identifier of the discovery.</param>
        /// <param name="moderationDto">The new status details.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The status was successfully updated.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized (requires Admin role).</response>
        /// <response code="404">Discovery not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}/Status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ModerateDiscovery(int id, DiscoveryModerationDto moderationDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Discovery? entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            
            string? adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(adminIdString, out int adminId))
            {
                entity.DiscoveryApprovalUserId = adminId;
            }

            entity.DiscoveryStatusId = moderationDto.DiscoveryStatusId;
            
            await _repository.UpdateAsync(entity, entity);
            
            try
            {
                if (moderationDto.DiscoveryStatusId == 3 || moderationDto.DiscoveryStatusId == 4)
                {
                    bool isApproved = moderationDto.DiscoveryStatusId == 3;
                    
                    int notificationTypeId = 6; 

                    string label = isApproved ? "Découverte Validée" : "Découverte Refusée";
                    
                    string description = isApproved
                        ? $"Félicitations ! Votre découverte '{entity.Title}' a été validée par nos équipes."
                        : $"Votre découverte '{entity.Title}' n'a pas été retenue.";
                    
                    string link = $"/corps-celestes?id={entity.CelestialBodyId}";
                    
                    var notification = new Notification
                    {
                        NotificationTypeId = notificationTypeId,
                        Label = label,
                        Description = description,
                        Link = link
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                    
                    var userNotification = new UserNotification
                    {
                        UserId = entity.UserId,
                        NotificationId = notification.Id,
                        IsRead = false,
                        ReceivedAt = DateTime.UtcNow
                    };

                    _context.UserNotifications.Add(userNotification);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur notification utilisateur : {ex.Message}");
            }
            return NoContent();
        }

        // --- HELPER ---
        private async Task<ActionResult<DiscoveryDto>> ProcessDiscoverySubmission<TEntity, TCreateDto>(
            string title, 
            TCreateDto detailsDto, 
            Func<TEntity, Task> addMethod,
            int celestialBodyTypeId) 
            where TEntity : class
            where TCreateDto : CelestialBodyCreateDto
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();
            
            detailsDto.CelestialBodyTypeId = celestialBodyTypeId;

            TEntity celestialEntity = _mapper.Map<TEntity>(detailsDto);
            await addMethod(celestialEntity);

            int celestialBodyId = (int)celestialEntity.GetType().GetProperty("CelestialBodyId")!.GetValue(celestialEntity)!;

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
            try 
            {
                var adminRole = await _context.UserRoles
                    .FirstOrDefaultAsync(r => r.Label == "Admin"); 

                if (adminRole != null)
                {
                    var notification = new Notification
                    {
                        NotificationTypeId = 5, 
                        Label = "Nouvelle soumission",
                        Description = $"Une nouvelle découverte '{title}' est en attente de validation.",
                        Link = "/admin/dashboard"
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                    
                    var admins = await _context.Users
                        .Where(u => u.UserRoleId == adminRole.Id) 
                        .ToListAsync();

                    if (admins.Any())
                    {
                        var userNotifications = admins.Select(admin => new UserNotification
                        {
                            UserId = admin.Id,
                            NotificationId = notification.Id,
                            IsRead = false,
                            ReceivedAt = DateTime.UtcNow
                        });

                        await _context.UserNotifications.AddRangeAsync(userNotifications);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'envoi de la notification admin : {ex.Message}");
            }
            return Ok(_mapper.Map<DiscoveryDto>(discovery));
        }
    }
}