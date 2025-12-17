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
    public class StarsControllerTests
        : CrudControllerTests<StarsController, Star, StarDto, StarDto, StarCreateDto, StarUpdateDto, int>
    {
        // --- CONSTANTES ---
        private const int USER_OWNER_ID = 5001;
        private const int USER_ADMIN_ID = 5002;
        private const int USER_STRANGER_ID = 5003;

        private const int STATUS_DRAFT = 1;
        private const int STATUS_ACCEPTED = 3;

        // IDs fixes
        private const int STAR_DRAFT_ID = 990401;
        private const int STAR_ACCEPTED_ID = 990402;

        // IDs Types et Classes
        private const int CBT_STAR = 3; // ID Type pour Star
        private int _spectralClassId = 1;

        // Variables membres
        private int _starDraftId;
        private int _starAcceptedId;

        protected override StarsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var starManager = new StarManager(context);
            var discoveryManager = new DiscoveryManager(context);

            var controller = new StarsController(
                starManager,
                discoveryManager,
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

        protected override List<Star> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            // ==============================================================================
            // 1. SETUP DES DEPENDANCES (Sauvegarde immédiate)
            // ==============================================================================

            // Users
            CreateUserIfNotExist(USER_OWNER_ID, 1);
            CreateUserIfNotExist(USER_ADMIN_ID, 2);
            CreateUserIfNotExist(USER_STRANGER_ID, 1);

            // Status
            EnsureStatusExists(STATUS_DRAFT, "Draft");
            EnsureStatusExists(STATUS_ACCEPTED, "Accepted");

            // CelestialBodyType (Star = 3)
            if (!_context.CelestialBodyTypes.AsNoTracking().Any(t => t.Id == CBT_STAR))
                _context.CelestialBodyTypes.Add(new CelestialBodyType { Id = CBT_STAR, Label = "Star" });

            // Spectral Class (Spécifique aux étoiles)
            if (!_context.SpectralClasses.AsNoTracking().Any(sc => sc.Id == _spectralClassId))
                _context.SpectralClasses.Add(new SpectralClass { Id = _spectralClassId, Label = "M"});

            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            // ==============================================================================
            // 2. CREATION DES ENTITES EN MÉMOIRE
            // ==============================================================================

            var s1 = CreateStarInMemory(
                STAR_DRAFT_ID,
                "Proxima Draft",
                USER_OWNER_ID,
                STATUS_DRAFT,
                temperature: 3000
            );

            var s2 = CreateStarInMemory(
                STAR_ACCEPTED_ID,
                "Sirius Accepted",
                USER_OWNER_ID,
                STATUS_ACCEPTED,
                temperature: 9900
            );

            _starDraftId = STAR_DRAFT_ID;
            _starAcceptedId = STAR_ACCEPTED_ID;

            return new List<Star> { s1, s2 };
        }

        // --- HELPER CONSTRUCTION (In-Memory) ---
        private Star CreateStarInMemory(int id, string name, int userId, int statusId, decimal temperature)
        {
            var celestialBody = new CelestialBody
            {
                Id = id,
                Name = name,
                CelestialBodyTypeId = CBT_STAR,

                // Navigation vers la découverte
                DiscoveryNavigation = new Discovery
                {
                    Title = $"Discovery of {name}",
                    UserId = userId,
                    DiscoveryStatusId = statusId
                }
            };

            var star = new Star
            {
                Id = id,
                CelestialBodyId = id,
                SpectralClassId = _spectralClassId,
                Temperature = temperature,
                Luminosity = 1.0m,
                Radius = 1.0m,
                Distance = 4.2m,
                Constellation = "Centaurus",

                // Important : Lien de navigation
                CelestialBodyNavigation = celestialBody
            };

            return star;
        }

        // --- HELPERS DEPENDANCES ---
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
            }
        }

        private void EnsureStatusExists(int id, string label)
        {
            if (!_context.DiscoveryStatuses.AsNoTracking().Any(s => s.Id == id))
                _context.DiscoveryStatuses.Add(new DiscoveryStatus { Id = id, Label = label });
        }

        // --- CONFIGURATION CRUD ---
        protected override int GetIdFromEntity(Star entity) => entity.Id;
        protected override int GetIdFromDto(StarDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override StarCreateDto GetValidCreateDto()
        {
            return new StarCreateDto
            {
                Name = "Generic Star Create",
                CelestialBodyTypeId = CBT_STAR,
                SpectralClassId = _spectralClassId,
                Temperature = 5000
            };
        }

        protected override StarUpdateDto GetValidUpdateDto(Star entityToUpdate)
        {
            return new StarUpdateDto
            {
                SpectralClassId = _spectralClassId,
                Temperature = 6000,
                Constellation = "Orion"
            };
        }

        protected override void SetIdInUpdateDto(StarUpdateDto dto, int id) { }


        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            var createDto = GetValidCreateDto();
            var result = await _controller.Post(createDto);
            // Le POST direct sur Star est interdit -> BadRequest
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreate_ReturnsCreated()
        {
            await Post_ValidObject_ShouldCreateAndReturn200();
        }

        [TestMethod]
        public async Task Search_ByTemperature_ShouldReturnCorrectStars()
        {
            // Given (Draft=3000, Accepted=9900)
            var filter = new StarFilterDto
            {
                MinTemperature = 8000 // On cherche les étoiles chaudes
            };

            // When
            var result = await _controller.Search(filter);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<StarDto>;

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any(), "Devrait trouver l'étoile acceptée");
            Assert.IsTrue(list.All(s => s.Temperature >= 8000), "Filtre MinTemperature incorrect");
            Assert.IsFalse(list.Any(s => s.Id == _starDraftId), "Ne devrait pas trouver l'étoile froide");
        }

        [TestMethod]
        public async Task Search_ByConstellation_ShouldReturnCorrectStars()
        {
            // Given (Les deux sont dans Centaurus par défaut via le Helper)
            var filter = new StarFilterDto
            {
                Constellation = "Centaurus"
            };

            var result = await _controller.Search(filter);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<StarDto>;
            Assert.IsTrue(list.Count() >= 2);
        }

        [TestMethod]
        public async Task Put_OwnerOnDraft_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var updateDto = GetValidUpdateDto(new Star());

            var result = await _controller.Put(_starDraftId, updateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updated = await _context.Stars.FindAsync(_starDraftId);
            Assert.AreEqual("Orion", updated.Constellation);
        }

        // --- TESTS DELETE (Avec logique de sécurité) ---

        [TestMethod]
        public async Task Delete_OwnerOnDraft_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var result = await _controller.Delete(_starDraftId);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_OwnerOnAccepted_ShouldFail()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var result = await _controller.Delete(_starAcceptedId);

            // Le contrôleur renvoie Forbid (403)
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_AdminOnAccepted_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var result = await _controller.Delete(_starAcceptedId);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        // Surcharges
        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            var result = await _controller.GetById(_starAcceptedId);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var dto = (result.Result as OkObjectResult).Value as StarDto;
            Assert.AreEqual(_starAcceptedId, dto.Id);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<StarDto>;
            Assert.IsTrue(list.Any(s => s.Id == _starDraftId));
        }
    }
}