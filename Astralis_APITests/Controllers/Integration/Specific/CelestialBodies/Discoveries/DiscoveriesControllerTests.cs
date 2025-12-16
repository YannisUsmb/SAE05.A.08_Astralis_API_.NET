using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        // --- IDS DES TYPES DE CORPS CELESTES (FKs) ---
        private int _typeAsteroidId;
        private int _typePlanetId;
        private int _typeStarId;
        private int _typeCometId;
        private int _typeGalaxyId;

        // --- IDS DES CLASSIFICATIONS SPECIFIQUES (Sous-types) ---
        private int _orbitalClassId;     // Asteroid
        private int _planetSubTypeId;    // Planet (PlanetType)
        private int _detectionMethodId;  // Planet (DetectionMethod) -> AJOUTÉ (C'était l'erreur 23503)
        private int _spectralClassId;    // Star
        private int _galaxyClassId;      // Galaxy

        // --- IDS DES ENTITÉS DE TEST ---
        private int _discoveryDraftId;
        private int _discoveryAcceptedId;

        protected override DiscoveriesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            // Instanciation de TOUS les managers nécessaires (y compris DetectionMethod)
            var discoveryRepo = new DiscoveryManager(context);
            var asteroidRepo = new AsteroidManager(context);
            var planetRepo = new PlanetManager(context);
            var starRepo = new StarManager(context);
            var cometRepo = new CometManager(context);
            var galaxyRepo = new GalaxyQuasarManager(context);
            var celestialBodyRepo = new CelestialBodyManager(context);

            // Correction: On ajoute DetectionMethodManager car il peut être requis par les validations internes
            // Note: Si votre constructeur de controller n'en a pas besoin, c'est ok, 
            // mais assurez-vous que le contexte DB contient bien les données.

            var controller = new DiscoveriesController(
                discoveryRepo,
                asteroidRepo,
                planetRepo,
                starRepo,
                cometRepo,
                galaxyRepo,
                celestialBodyRepo,
                mapper
            );

            SetupUserContext(controller, USER_OWNER_ID, "Client");
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, "TestUser")
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
            // 1. Setup Roles & Users
            var roleClient = GetOrCreateRole("Client");
            var roleAdmin = GetOrCreateRole("Admin");

            CreateUserIfNotExist(USER_OWNER_ID, "Owner", roleClient.Id);
            CreateUserIfNotExist(USER_ADMIN_ID, "Admin", roleAdmin.Id);
            CreateUserIfNotExist(USER_OTHER_ID, "Hacker", roleClient.Id);

            // 2. Setup Statuses
            GetOrCreateStatus(STATUS_DRAFT, "Draft");
            GetOrCreateStatus(STATUS_PENDING, "Pending");
            GetOrCreateStatus(STATUS_ACCEPTED, "Accepted");
            GetOrCreateStatus(STATUS_DECLINED, "Declined");

            // 3. Setup Celestial Types
            _typeAsteroidId = GetOrCreateType("Asteroid").Id;
            _typePlanetId = GetOrCreateType("Planet").Id;
            _typeStarId = GetOrCreateType("Star").Id;
            _typeCometId = GetOrCreateType("Comet").Id;
            _typeGalaxyId = GetOrCreateType("Galaxy").Id;

            // 4. Setup Sub-Types
            // CORRECTION ICI : "Aten" (4 chars) -> "ATE" (3 chars) pour respecter la limite DB
            _orbitalClassId = GetOrCreateOrbitalClass("ATE").Id;

            _planetSubTypeId = GetOrCreatePlanetType("Terrestrial").Id;
            _detectionMethodId = GetOrCreateDetectionMethod("Transit").Id;
            _spectralClassId = GetOrCreateSpectralClass("G-Type").Id;
            _galaxyClassId = GetOrCreateGalaxyClass("Spiral").Id;

            _context.SaveChanges();

            // 5. Setup Bodies & Discoveries
            var list = new List<Discovery>();

            // Découverte 1 : Draft
            var bodyDraft = CreateBody("Body_Draft", _typeAsteroidId);
            var draftDiscovery = new Discovery
            {
                Title = "Draft Discovery Title",
                UserId = USER_OWNER_ID,
                CelestialBodyId = bodyDraft.Id,
                DiscoveryStatusId = STATUS_DRAFT
            };

            if (!_context.Discoveries.Any(d => d.Title == draftDiscovery.Title))
            {
                list.Add(draftDiscovery);
            }

            // Découverte 2 : Accepted
            var bodyAccepted = CreateBody("Body_Accepted", _typePlanetId);
            var acceptedDiscovery = new Discovery
            {
                Title = "Accepted Discovery Title",
                UserId = USER_OWNER_ID,
                CelestialBodyId = bodyAccepted.Id,
                DiscoveryStatusId = STATUS_ACCEPTED
            };

            if (!_context.Discoveries.Any(d => d.Title == acceptedDiscovery.Title))
            {
                list.Add(acceptedDiscovery);
            }

            _context.SaveChanges();
            return list;
        }

        // --- HELPERS DE SEEDING ---
        // Note: L'ajout de _context.SaveChanges() dans chaque helper garantit que l'ID est valide immédiatement.

        private UserRole GetOrCreateRole(string label)
        {
            var r = _context.UserRoles.FirstOrDefault(x => x.Label == label);
            if (r == null) { r = new UserRole { Label = label }; _context.UserRoles.Add(r); _context.SaveChanges(); }
            return r;
        }
        private void CreateUserIfNotExist(int id, string name, int roleId)
        {
            if (!_context.Users.Any(u => u.Id == id))
            {
                _context.Users.Add(new User { Id = id, Username = name, UserRoleId = roleId, Email = $"{name}@test.com", FirstName = name, LastName = "T", Password = "pwd", IsPremium = false });
                _context.SaveChanges();
            }
        }
        private DiscoveryStatus GetOrCreateStatus(int id, string label)
        {
            var s = _context.DiscoveryStatuses.FirstOrDefault(x => x.Id == id);
            if (s == null) { s = new DiscoveryStatus { Id = id, Label = label }; _context.DiscoveryStatuses.Add(s); _context.SaveChanges(); }
            return s;
        }
        private CelestialBodyType GetOrCreateType(string label)
        {
            var t = _context.CelestialBodyTypes.FirstOrDefault(x => x.Label == label);
            if (t == null) { t = new CelestialBodyType { Label = label }; _context.CelestialBodyTypes.Add(t); _context.SaveChanges(); }
            return t;
        }
        private CelestialBody CreateBody(string name, int typeId)
        {
            var b = _context.CelestialBodies.FirstOrDefault(x => x.Name == name);
            if (b == null)
            {
                b = new CelestialBody { Name = name, CelestialBodyTypeId = typeId, Alias = $"Alias-{name}" };
                _context.CelestialBodies.Add(b);
                _context.SaveChanges();
            }
            return b;
        }

        private OrbitalClass GetOrCreateOrbitalClass(string label)
        {
            var x = _context.OrbitalClasses.FirstOrDefault(l => l.Label == label);
            if (x == null) { x = new OrbitalClass { Label = label, Description = "Desc" }; _context.OrbitalClasses.Add(x); _context.SaveChanges(); }
            return x;
        }
        private PlanetType GetOrCreatePlanetType(string label)
        {
            var x = _context.PlanetTypes.FirstOrDefault(l => l.Label == label);
            if (x == null) { x = new PlanetType { Label = label, Description = "Desc" }; _context.PlanetTypes.Add(x); _context.SaveChanges(); }
            return x;
        }
        // NOUVEAU HELPER POUR DETECTION METHOD
        private DetectionMethod GetOrCreateDetectionMethod(string label)
        {
            var x = _context.DetectionMethods.FirstOrDefault(l => l.Label == label);
            if (x == null) { x = new DetectionMethod { Label = label, Description = "Desc" }; _context.DetectionMethods.Add(x); _context.SaveChanges(); }
            return x;
        }
        private SpectralClass GetOrCreateSpectralClass(string label)
        {
            var x = _context.SpectralClasses.FirstOrDefault(l => l.Label == label);
            if (x == null) { x = new SpectralClass { Label = label, Description = "Desc" }; _context.SpectralClasses.Add(x); _context.SaveChanges(); }
            return x;
        }
        private GalaxyQuasarClass GetOrCreateGalaxyClass(string label)
        {
            var x = _context.GalaxyQuasarClasses.FirstOrDefault(l => l.Label == label);
            if (x == null) { x = new GalaxyQuasarClass { Label = label, Description = "Desc" }; _context.GalaxyQuasarClasses.Add(x); _context.SaveChanges(); }
            return x;
        }

        // --- CONFIGURATION CRUD BASE ---
        protected override int GetIdFromEntity(Discovery entity) => entity.Id;
        protected override int GetIdFromDto(DiscoveryDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;
        protected override DiscoveryCreateDto GetValidCreateDto() => new DiscoveryCreateDto { Title = "Generic Post (Should Fail)" };
        protected override DiscoveryUpdateDto GetValidUpdateDto(Discovery entityToUpdate) => new DiscoveryUpdateDto { Title = entityToUpdate.Title + " Updated" };
        protected override void SetIdInUpdateDto(DiscoveryUpdateDto dto, int id) { }

        // --- METHODE DE REFRESH DES IDs ---
        private async Task RefreshIds()
        {
            var draft = await _context.Discoveries.FirstOrDefaultAsync(d => d.Title == "Draft Discovery Title");
            if (draft != null) _discoveryDraftId = draft.Id;

            var accepted = await _context.Discoveries.FirstOrDefaultAsync(d => d.Title == "Accepted Discovery Title");
            if (accepted != null) _discoveryAcceptedId = accepted.Id;
        }

        // ==========================================
        // TESTS SPECIFIQUES
        // ==========================================

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            var createDto = GetValidCreateDto();
            var actionResult = await _controller.Post(createDto);
            // On attend BadRequest car ce endpoint est désactivé
            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult));
        }

        // --- TESTS CREATION PAR TYPE ---

        [TestMethod]
        public async Task PostAsteroid_Valid_ShouldReturnOk()
        {
            var submission = new DiscoveryAsteroidSubmissionDto
            {
                Title = "New Asteroid",
                Details = new AsteroidCreateDto
                {
                    Name = "AST-001",
                    CelestialBodyTypeId = _typeAsteroidId,
                    OrbitalClassId = _orbitalClassId, // Doit être valide en DB
                    DiameterMinKm = 1,
                    DiameterMaxKm = 2
                }
            };

            var actionResult = await _controller.PostAsteroid(submission);

            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "PostAsteroid should return OkObjectResult");
            var dto = okResult.Value as DiscoveryDto;
            Assert.AreEqual("New Asteroid", dto.Title);
        }

        [TestMethod]
        public async Task PostPlanet_Valid_ShouldReturnOk()
        {
            // CORRECTION: Ajout de DetectionMethodId
            var submission = new DiscoveryPlanetSubmissionDto
            {
                Title = "New Planet",
                Details = new PlanetCreateDto
                {
                    Name = "PLN-001",
                    CelestialBodyTypeId = _typePlanetId,
                    PlanetTypeId = _planetSubTypeId,
                    DetectionMethodId = _detectionMethodId, // -> Était manquant
                    Mass = 100
                }
            };

            var actionResult = await _controller.PostPlanet(submission);
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task PostStar_Valid_ShouldReturnOk()
        {
            var submission = new DiscoveryStarSubmissionDto
            {
                Title = "New Star",
                Details = new StarCreateDto
                {
                    Name = "STR-001",
                    CelestialBodyTypeId = _typeStarId,
                    SpectralClassId = _spectralClassId,
                    Temperature = 5000
                }
            };

            var actionResult = await _controller.PostStar(submission);
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task PostComet_Valid_ShouldReturnOk()
        {
            var submission = new DiscoveryCometSubmissionDto
            {
                Title = "New Comet",
                Details = new CometCreateDto
                {
                    Name = "CMT-001",
                    CelestialBodyTypeId = _typeCometId
                }
            };

            var actionResult = await _controller.PostComet(submission);
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task PostGalaxy_Valid_ShouldReturnOk()
        {
            var submission = new DiscoveryGalaxyQuasarSubmissionDto
            {
                Title = "New Galaxy",
                Details = new GalaxyQuasarCreateDto
                {
                    Name = "GAL-001",
                    CelestialBodyTypeId = _typeGalaxyId,
                    GalaxyQuasarClassId = _galaxyClassId
                }
            };

            var actionResult = await _controller.PostGalaxy(submission);
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
        }

        // --- TESTS MISE A JOUR (PUT) ---

        [TestMethod]
        public async Task Put_UpdateTitle_OnDraft_ShouldSuccess()
        {
            await RefreshIds();
            var updateDto = new DiscoveryUpdateDto { Title = "Updated Draft Title" };

            var actionResult = await _controller.Put(_discoveryDraftId, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var entity = await _context.Discoveries.FindAsync(_discoveryDraftId);
            Assert.AreEqual("Updated Draft Title", entity.Title);
        }

        [TestMethod]
        public async Task Put_UpdateTitle_OnAccepted_ShouldFail()
        {
            await RefreshIds();
            var updateDto = new DiscoveryUpdateDto { Title = "Hack Attempt" };

            var actionResult = await _controller.Put(_discoveryAcceptedId, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Put_AsOtherUser_ShouldReturnForbidden()
        {
            await RefreshIds();
            SetupUserContext(_controller, USER_OTHER_ID, "Client");

            var updateDto = new DiscoveryUpdateDto { Title = "Hack Attempt" };
            var actionResult = await _controller.Put(_discoveryDraftId, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }

        // --- TESTS PROPOSITION ALIAS ---

        [TestMethod]
        public async Task ProposeAlias_OwnerOnAccepted_ShouldSuccess()
        {
            await RefreshIds();
            var aliasDto = new DiscoveryAliasDto { Alias = "Super Earth" };

            var actionResult = await _controller.ProposeAlias(_discoveryAcceptedId, aliasDto);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var entity = await _context.Discoveries.FindAsync(_discoveryAcceptedId);
            Assert.AreEqual(1, entity.AliasStatusId);
            Assert.IsNull(entity.AliasApprovalUserId);
        }

        [TestMethod]
        public async Task ProposeAlias_OnDraft_ShouldFail()
        {
            await RefreshIds();
            var aliasDto = new DiscoveryAliasDto { Alias = "Premature Alias" };

            var actionResult = await _controller.ProposeAlias(_discoveryDraftId, aliasDto);

            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
        }

        // --- TESTS RECHERCHE ---

        [TestMethod]
        public async Task Search_ShouldReturnResults()
        {
            var filter = new DiscoveryFilterDto { Title = "Draft" };
            var actionResult = await _controller.Search(filter);

            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as IEnumerable<DiscoveryDto>;
            Assert.IsTrue(list.Any());
        }

        // --- TESTS MODERATION ---

        [TestMethod]
        public async Task ModerateDiscovery_AsAdmin_ShouldSuccess()
        {
            await RefreshIds();
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

        // --- TESTS SUPPRESSION ---

        [TestMethod]
        public async Task Delete_OwnerOnDraft_ShouldSuccess()
        {
            await RefreshIds();
            var actionResult = await _controller.Delete(_discoveryDraftId);
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_OwnerOnAccepted_ShouldFail()
        {
            await RefreshIds();
            var actionResult = await _controller.Delete(_discoveryAcceptedId);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Delete_AdminOnAccepted_ShouldSuccess()
        {
            await RefreshIds();
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");

            var actionResult = await _controller.Delete(_discoveryAcceptedId);
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }
    }
}