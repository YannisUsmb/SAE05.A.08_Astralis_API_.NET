using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class DiscoveriesControllerTests
        : CrudControllerTests<DiscoveriesController, Discovery, DiscoveryDto, DiscoveryDto, DiscoveryCreateDto, DiscoveryUpdateDto, int>
    {
        // --- CONSTANTES ---
        private const int USER_OWNER_ID = 5001;
        private const int USER_ADMIN_ID = 5002;
        private const int USER_OTHER_ID = 5003;

        private const int STATUS_DRAFT = 1;
        private const int STATUS_PENDING = 2;
        private const int STATUS_ACCEPTED = 3;
        private const int STATUS_DECLINED = 4;

        private const int DISCOVERY_DRAFT_ID = 990001;
        private const int DISCOVERY_ACCEPTED_ID = 990002;
        private const int PLANET_HOST_ID = 990110;


        // --- IDs TYPES CORPS CELESTES ---
        private const int CBT_ASTEROID = 1;
        private const int CBT_PLANET = 2;
        private const int CBT_STAR = 3;
        private const int CBT_GALAXY = 5;
        private const int CBT_SATELLITE = 6; // Ajout du type Satellite

        // --- IDs CLASSES SPECIFIQUES ---
        private int _spectralClassId = 1;
        private int _galaxyQuasarClassId = 1;
        private int _orbitalClassId = 1;
        private int _planetTypeId = 1;

        // Variables membres
        private int _discoveryDraftId;
        private int _discoveryAcceptedId;

        protected override DiscoveriesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            // Instanciation des managers
            var discoveryManager = new DiscoveryManager(context);
            var asteroidManager = new AsteroidManager(context);
            var planetManager = new PlanetManager(context);
            var starManager = new StarManager(context);
            var cometManager = new CometManager(context);
            var galaxyManager = new GalaxyQuasarManager(context);
            var celestialBodyManager = new CelestialBodyManager(context);
            var satelliteManager = new SatelliteManager(context); // Nouveau Manager
            var userManager = new UserManager(context);
            var controller = new DiscoveriesController(
                discoveryManager,
                asteroidManager,
                planetManager,
                starManager,
                cometManager,
                galaxyManager,
                celestialBodyManager,
                satelliteManager,
                userManager,
                mapper
            );

            SetupUserContext(controller, USER_OWNER_ID, "Explorer");
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, $"User_{userId}")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        protected override List<Discovery> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            // 1. SETUP UTILISATEURS
            CreateUserIfNotExist(USER_OWNER_ID, 1);
            CreateUserIfNotExist(USER_ADMIN_ID, 2);
            CreateUserIfNotExist(USER_OTHER_ID, 1);

            // 2. SETUP STATUS
            EnsureStatusExists(STATUS_DRAFT, "Draft");
            EnsureStatusExists(STATUS_PENDING, "Pending");
            EnsureStatusExists(STATUS_ACCEPTED, "Accepted");
            EnsureStatusExists(STATUS_DECLINED, "Declined");

            // 3. SETUP TYPES DE CORPS CELESTES
            EnsureBodyTypeExists(CBT_ASTEROID, "Asteroid");
            EnsureBodyTypeExists(CBT_PLANET, "Planet");
            EnsureBodyTypeExists(CBT_STAR, "Star");
            EnsureBodyTypeExists(CBT_GALAXY, "Galaxy");
            EnsureBodyTypeExists(CBT_SATELLITE, "Satellite"); // Setup Satellite Type

            // 4. SETUP CLASSES SPECIFIQUES
            if (!_context.SpectralClasses.Any(sc => sc.Id == _spectralClassId))
                _context.SpectralClasses.Add(new SpectralClass { Id = _spectralClassId, Label = "M" });

            if (!_context.GalaxyQuasarClasses.Any(gt => gt.Id == _galaxyQuasarClassId))
                _context.GalaxyQuasarClasses.Add(new GalaxyQuasarClass { Id = _galaxyQuasarClassId, Label = "Spiral" });

            if (!_context.OrbitalClasses.Any(oc => oc.Id == _orbitalClassId))
                _context.OrbitalClasses.Add(new OrbitalClass { Id = _orbitalClassId, Label = "TGT", Description = "Target" });

            if (!_context.PlanetTypes.Any(pt => pt.Id == _planetTypeId))
                _context.PlanetTypes.Add(new PlanetType { Id = _planetTypeId, Label = "Terrestrial" });

            _context.SaveChanges();

            // 5. SETUP ENTITÉS DE TEST
            var bodyDraft = GetOrCreateBody(DISCOVERY_DRAFT_ID, "Body Draft", CBT_ASTEROID);
            var bodyAccepted = GetOrCreateBody(DISCOVERY_ACCEPTED_ID, "Body Accepted", CBT_ASTEROID);

            var d1 = GetOrCreateDiscovery(DISCOVERY_DRAFT_ID, "Discovery Draft", bodyDraft.Id, USER_OWNER_ID, STATUS_DRAFT);
            var d2 = GetOrCreateDiscovery(DISCOVERY_ACCEPTED_ID, "Discovery Accepted", bodyAccepted.Id, USER_OWNER_ID, STATUS_ACCEPTED);

            _discoveryDraftId = DISCOVERY_DRAFT_ID;
            _discoveryAcceptedId = DISCOVERY_ACCEPTED_ID;

            return new List<Discovery> { d1, d2 };
        }

        // --- HELPERS ---
        private void EnsureStatusExists(int id, string label)
        {
            if (!_context.DiscoveryStatuses.AsNoTracking().Any(s => s.Id == id))
                _context.DiscoveryStatuses.Add(new DiscoveryStatus { Id = id, Label = label });
        }

        private void EnsureBodyTypeExists(int id, string label)
        {
            if (!_context.CelestialBodyTypes.AsNoTracking().Any(t => t.Id == id))
                _context.CelestialBodyTypes.Add(new CelestialBodyType { Id = id, Label = label });
        }

        private void CreateUserIfNotExist(int id, int roleId)
        {
            if (!_context.Users.AsNoTracking().Any(u => u.Id == id))
            {
                _context.Users.Add(new User
                {
                    Id = id,
                    UserRoleId = roleId,
                    Username = $"User{id}",
                    Email = $"user{id}@test.com",
                    FirstName = "Test",
                    LastName = "User",
                    Password = "Pwd"
                });
                _context.SaveChanges();
            }
        }

        private CelestialBody GetOrCreateBody(int id, string name, int typeId)
        {
            var body = _context.CelestialBodies.IgnoreQueryFilters().AsNoTracking().FirstOrDefault(b => b.Id == id);
            if (body == null)
            {
                body = new CelestialBody { Id = id, Name = name, CelestialBodyTypeId = typeId };
                _context.CelestialBodies.Add(body);
                _context.SaveChanges();
            }
            return body;
        }

        private Discovery GetOrCreateDiscovery(int id, string title, int bodyId, int userId, int statusId)
        {
            var discovery = _context.Discoveries.IgnoreQueryFilters().FirstOrDefault(d => d.Id == id);

            if (discovery != null)
            {
                discovery.Title = title;
                discovery.UserId = userId;
                discovery.CelestialBodyId = bodyId;
                discovery.DiscoveryStatusId = statusId;
                _context.Entry(discovery).State = EntityState.Modified;
                return discovery;
            }
            else
            {
                var newDiscovery = new Discovery
                {
                    Id = id,
                    Title = title,
                    CelestialBodyId = bodyId,
                    UserId = userId,
                    DiscoveryStatusId = statusId
                };
                _context.Discoveries.Add(newDiscovery);
                return newDiscovery;
            }
        }

        protected override int GetIdFromEntity(Discovery entity) => entity.Id;
        protected override int GetIdFromDto(DiscoveryDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override DiscoveryCreateDto GetValidCreateDto()
        {
            return new DiscoveryCreateDto
            {
                Title = "Generic Create",
                CelestialBodyId = _discoveryDraftId
            };
        }

        protected override DiscoveryUpdateDto GetValidUpdateDto(Discovery entityToUpdate)
        {
            return new DiscoveryUpdateDto { Title = entityToUpdate.Title + " Updated" };
        }

        protected override void SetIdInUpdateDto(DiscoveryUpdateDto dto, int id) { }

        // =========================================================================================
        // TESTS
        // =========================================================================================

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            var createDto = GetValidCreateDto();
            var result = await _controller.Post(createDto);
            // Le Post générique renvoie BadRequest dans DiscoveriesController
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreate_ReturnsCreated()
        {
            await Post_ValidObject_ShouldCreateAndReturn200();
        }

        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            var expectedId = _discoveryDraftId;
            var result = await _controller.GetById(expectedId);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var dto = (result.Result as OkObjectResult).Value as DiscoveryDto;
            Assert.IsNotNull(dto);
            Assert.AreEqual(expectedId, dto.Id);
            Assert.AreEqual("Discovery Draft", dto.Title);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk()
        {
            // When
            var result = await _controller.GetAll();

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<DiscoveryDto>;

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any(), "La liste ne doit pas être vide.");
            // On vérifie que nos données de test (Draft et Accepted) sont bien présentes
            Assert.IsTrue(list.Any(d => d.Id == _discoveryAcceptedId), "La découverte Accepted devrait être présente.");
        }

        // --- TESTS SPECIFIQUES POST ---

        [TestMethod]
        public async Task PostPlanet_ValidObject_ShouldCreateDiscoveryAndPlanet()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");

            var submissionDto = new DiscoveryPlanetSubmissionDto
            {
                Title = "New Earth Discovery",
                Details = new PlanetCreateDto
                {
                    Name = "Kepler-186f",
                    Mass = 1.1m,
                    CelestialBodyTypeId = CBT_PLANET,
                    PlanetTypeId = _planetTypeId,
                    DetectionMethodId = 1
                }
            };

            var actionResult = await _controller.PostPlanet(submissionDto);

            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
            var resultDto = (actionResult.Result as OkObjectResult).Value as DiscoveryDto;
            Assert.AreEqual("New Earth Discovery", resultDto.Title);

            var dbDiscovery = await _context.Discoveries
                .Include(d => d.CelestialBodyNavigation)
                .FirstOrDefaultAsync(d => d.Id == resultDto.Id);

            Assert.IsNotNull(dbDiscovery);
            Assert.AreEqual(CBT_PLANET, dbDiscovery.CelestialBodyNavigation.CelestialBodyTypeId);
        }

        [TestMethod]
        public async Task PostStar_ValidObject_ShouldCreateDiscoveryAndStar()
        {
            // Given
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            if (!_context.GalaxiesQuasars.Any(g => g.Id == 1))
            {
                _context.GalaxiesQuasars.Add(new GalaxyQuasar { Id = 1 });
                _context.SaveChanges();
            }

            var submissionDto = new DiscoveryStarSubmissionDto
            {
                Title = "Bright Star Discovery",
                Details = new StarCreateDto
                {
                    Name = "Betelgeuse II",
                    Temperature = 3500,
                    Distance = 500,
                    Luminosity = 1000,
                    Radius = 800,

                    CelestialBodyTypeId = CBT_STAR,
                    SpectralClassId = _spectralClassId
                }
            };

            // When
            var actionResult = await _controller.PostStar(submissionDto);

            // Then
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
            var resultDto = (actionResult.Result as OkObjectResult).Value as DiscoveryDto;
            Assert.IsNotNull(resultDto);
            Assert.AreEqual("Bright Star Discovery", resultDto.Title);

            _context.ChangeTracker.Clear();

            var dbDiscovery = await _context.Discoveries
                .Include(d => d.CelestialBodyNavigation)
                .ThenInclude(cb => cb.StarNavigation)
                .FirstOrDefaultAsync(d => d.Id == resultDto.Id);

            Assert.IsNotNull(dbDiscovery, "La découverte doit être sauvegardée en BDD.");
            Assert.IsNotNull(dbDiscovery.CelestialBodyNavigation.StarNavigation, "L'étoile liée doit exister.");
            Assert.AreEqual("Betelgeuse II", dbDiscovery.CelestialBodyNavigation.Name);

        }

        [TestMethod]
        public async Task PostGalaxy_ValidObject_ShouldCreateDiscoveryAndGalaxy()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");

            var submissionDto = new DiscoveryGalaxyQuasarSubmissionDto
            {
                Title = "Andromeda Neighbor",
                Details = new GalaxyQuasarCreateDto
                {
                    Name = "M32",
                    RightAscension = 10.5m,
                    Declination = 41.2m,
                    CelestialBodyTypeId = CBT_GALAXY,
                    GalaxyQuasarClassId = _galaxyQuasarClassId
                }
            };

            var actionResult = await _controller.PostGalaxy(submissionDto);

            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task PostSatellite_ValidObject_ShouldCreateDiscoveryAndSatellite()
        {
            // Given
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");

            // 1. Création ou récupération du BODY parent
            var hostBody = GetOrCreateBody(PLANET_HOST_ID, "Host Planet", CBT_PLANET);

            // 2. Création de la PLANETE spécifique (si elle n'existe pas)
            var existingPlanet = _context.Planets.AsNoTracking().FirstOrDefault(p => p.Id == PLANET_HOST_ID);

            if (existingPlanet == null)
            {
                var hostPlanet = new Planet
                {
                    Id = PLANET_HOST_ID,
                    CelestialBodyId = PLANET_HOST_ID,
                    PlanetTypeId = _planetTypeId,
                    DetectionMethodId = 1,
                    Mass = 500
                };
                _context.Planets.Add(hostPlanet);
                _context.SaveChanges();
            }

            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");

            var submissionDto = new DiscoverySatelliteSubmissionDto
            {
                Title = "Andromeda Neighbor",                
                Details = new SatelliteCreateDto
                {                    
                    PlanetId= PLANET_HOST_ID,
                    Name = "Satellite test",
                    Gravity = 1m,
                    Radius = 1,
                    Density = 1.5m,
                    CelestialBodyTypeId =1
                }
            };

            var actionResult = await _controller.PostSatellite(submissionDto);

            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task ProposeAlias_AsOwner_ShouldUpdateStatusToPending()
        {
            // Given
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");

            var user = _context.Users.FirstOrDefault(u => u.Id == USER_OWNER_ID);
            if (user != null)
            {
                user.IsPremium = true;
                _context.SaveChanges();
            }
            _context.ChangeTracker.Clear();

            var aliasDto = new DiscoveryAliasDto { Alias = "The Lonely Rock" };

            // When
            var actionResult = await _controller.ProposeAlias(_discoveryAcceptedId, aliasDto);

            // Then
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var discovery = await _context.Discoveries
                .Include(d => d.CelestialBodyNavigation)
                .FirstOrDefaultAsync(d => d.Id == _discoveryAcceptedId);

            Assert.AreEqual("The Lonely Rock", discovery.CelestialBodyNavigation.Alias);
        }

        [TestMethod]
        public async Task ModerateDiscovery_AsAdmin_ShouldUpdateStatus()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var modDto = new DiscoveryModerationDto
            {
                DiscoveryStatusId = STATUS_ACCEPTED,
                AliasStatusId = STATUS_ACCEPTED
            };

            var actionResult = await _controller.ModerateDiscovery(_discoveryDraftId, modDto);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var entity = await _context.Discoveries.FindAsync(_discoveryDraftId);
            Assert.AreEqual(STATUS_ACCEPTED, entity.DiscoveryStatusId);
            Assert.AreEqual(USER_ADMIN_ID, entity.DiscoveryApprovalUserId);
        }

        [TestMethod]
        public async Task Search_FilterByStatus_ShouldReturnOnlyMatchingDiscoveries()
        {
            var filter = new DiscoveryFilterDto
            {
                Title = null,
                DiscoveryStatusId = STATUS_DRAFT,
                AliasStatusId = null
            };

            var results = await _controller.Search(filter);

            Assert.IsInstanceOfType(results.Result, typeof(OkObjectResult));
            var list = (results.Result as OkObjectResult).Value as IEnumerable<DiscoveryDto>;

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any(), "Devrait trouver le brouillon");
            Assert.IsTrue(list.All(d => d.DiscoveryStatusId == STATUS_DRAFT), "Ne doit contenir que des brouillons");
        }

        [TestMethod]
        public async Task Delete_OwnerOnDraft_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var actionResult = await _controller.Delete(_discoveryDraftId);
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_OwnerOnAccepted_ShouldFail()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var actionResult = await _controller.Delete(_discoveryAcceptedId);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Delete_AdminOnAccepted_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var actionResult = await _controller.Delete(_discoveryAcceptedId);
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }
    }
}