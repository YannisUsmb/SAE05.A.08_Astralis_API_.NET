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
    public class PlanetsControllerTests
        : CrudControllerTests<PlanetsController, Planet, PlanetDto, PlanetDto, PlanetCreateDto, PlanetUpdateDto, int>
    {
        private const int USER_OWNER_ID = 5001;
        private const int USER_ADMIN_ID = 5002;
        private const int USER_STRANGER_ID = 5003;

        private const int STATUS_DRAFT = 1;
        private const int STATUS_ACCEPTED = 3;

        private const int PLANET_DRAFT_ID = 990201;
        private const int PLANET_ACCEPTED_ID = 990202;

        private int _planetTypeId = 1;
        private int _detectionMethodId = 1;

        private int _planetDraftId;
        private int _planetAcceptedId;

        protected override PlanetsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var planetManager = new PlanetManager(context);
            var discoveryManager = new DiscoveryManager(context);

            var controller = new PlanetsController(
                planetManager,
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

        protected override List<Planet> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            CreateUserIfNotExist(USER_OWNER_ID, 1);
            CreateUserIfNotExist(USER_ADMIN_ID, 2);
            CreateUserIfNotExist(USER_STRANGER_ID, 1);

            EnsureStatusExists(STATUS_DRAFT, "Draft");
            EnsureStatusExists(STATUS_ACCEPTED, "Accepted");

            if (!_context.CelestialBodyTypes.AsNoTracking().Any(t => t.Id == 2))
                _context.CelestialBodyTypes.Add(new CelestialBodyType { Id = 2, Label = "Planet" });

            if (!_context.PlanetTypes.AsNoTracking().Any(pt => pt.Id == _planetTypeId))
                _context.PlanetTypes.Add(new PlanetType { Id = _planetTypeId, Label = "Terrestrial" });

            if (!_context.DetectionMethods.AsNoTracking().Any(dm => dm.Id == _detectionMethodId))
                _context.DetectionMethods.Add(new DetectionMethod { Id = _detectionMethodId, Label = "Transit" });

            _context.SaveChanges();

            _context.ChangeTracker.Clear();

            var p1 = CreatePlanetInMemory(
                PLANET_DRAFT_ID,
                "Planet Draft",
                USER_OWNER_ID,
                STATUS_DRAFT,
                mass: 1.5m
            );

            var p2 = CreatePlanetInMemory(
                PLANET_ACCEPTED_ID,
                "Planet Accepted",
                USER_OWNER_ID,
                STATUS_ACCEPTED,
                mass: 10.0m
            );

            _planetDraftId = PLANET_DRAFT_ID;
            _planetAcceptedId = PLANET_ACCEPTED_ID;

            return new List<Planet> { p1, p2 };
        }

        private Planet CreatePlanetInMemory(int id, string name, int userId, int statusId, decimal mass)
        {
            var celestialBody = new CelestialBody
            {
                Id = id,
                Name = name,
                CelestialBodyTypeId = 2,

                DiscoveryNavigation = new Discovery
                {
                    Title = $"Discovery of {name}",
                    UserId = userId,
                    DiscoveryStatusId = statusId
                }
            };

            var planet = new Planet
            {
                Id = id,
                CelestialBodyId = id,
                PlanetTypeId = _planetTypeId,
                DetectionMethodId = _detectionMethodId,
                Mass = mass,
                Radius = 1.0m,
                DiscoveryYear = 2023,

                CelestialBodyNavigation = celestialBody
            };

            return planet;
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
            }
        }

        private void EnsureStatusExists(int id, string label)
        {
            if (!_context.DiscoveryStatuses.AsNoTracking().Any(s => s.Id == id))
                _context.DiscoveryStatuses.Add(new DiscoveryStatus { Id = id, Label = label });
        }

        protected override int GetIdFromEntity(Planet entity) => entity.Id;
        protected override int GetIdFromDto(PlanetDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override PlanetCreateDto GetValidCreateDto()
        {
            return new PlanetCreateDto
            {
                Name = "Generic Create",
                CelestialBodyTypeId = 2,
                PlanetTypeId = _planetTypeId,
                DetectionMethodId = _detectionMethodId
            };
        }

        protected override PlanetUpdateDto GetValidUpdateDto(Planet entityToUpdate)
        {
            return new PlanetUpdateDto
            {
                PlanetTypeId = _planetTypeId,
                DetectionMethodId = _detectionMethodId,
                Mass = 999.9m,
                Remark = "Updated by Test"
            };
        }

        protected override void SetIdInUpdateDto(PlanetUpdateDto dto, int id) { }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            var createDto = GetValidCreateDto();
            var result = await _controller.Post(createDto);
            // Le POST direct sur Planet est interdit (BadRequest)
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreate_ReturnsCreated()
        {
            await Post_ValidObject_ShouldCreateAndReturn200();
        }

        [TestMethod]
        public async Task Search_ByMassRange_ShouldReturnCorrectPlanets()
        {
            var filter = new PlanetFilterDto
            {
                MinMass = 5.0m
            };

            var result = await _controller.Search(filter);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<PlanetDto>;

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any(), "Devrait trouver la planète acceptée");
            Assert.IsTrue(list.All(p => p.Mass >= 5.0m), "Filtre MinMass incorrect");
            Assert.IsFalse(list.Any(p => p.Id == _planetDraftId), "Ne devrait pas trouver la petite planète");
        }

        [TestMethod]
        public async Task Put_OwnerOnDraft_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var updateDto = GetValidUpdateDto(new Planet());

            var result = await _controller.Put(_planetDraftId, updateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updated = await _context.Planets.FindAsync(_planetDraftId);
            Assert.AreEqual("Updated by Test", updated.Remark);
        }

        [TestMethod]
        public async Task Put_OwnerOnAccepted_ShouldFail()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var updateDto = GetValidUpdateDto(new Planet());

            var result = await _controller.Put(_planetAcceptedId, updateDto);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_OwnerOnDraft_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var result = await _controller.Delete(_planetDraftId);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_OwnerOnAccepted_ShouldFail()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var result = await _controller.Delete(_planetAcceptedId);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_AdminOnAccepted_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var result = await _controller.Delete(_planetAcceptedId);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            var result = await _controller.GetById(_planetAcceptedId);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var dto = (result.Result as OkObjectResult).Value as PlanetDto;
            Assert.AreEqual(_planetAcceptedId, dto.Id);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<PlanetDto>;
            Assert.IsTrue(list.Any(p => p.Id == _planetDraftId));
        }
    }
}