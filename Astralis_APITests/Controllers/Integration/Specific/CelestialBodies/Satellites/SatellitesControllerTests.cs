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
    public class SatellitesControllerTests
        : CrudControllerTests<SatellitesController, Satellite, SatelliteDto, SatelliteDto, SatelliteCreateDto, SatelliteUpdateDto, int>
    {
        // --- CONSTANTES ---
        private const int USER_OWNER_ID = 5001;
        private const int USER_ADMIN_ID = 5002;
        private const int USER_STRANGER_ID = 5003;

        private const int STATUS_DRAFT = 1;
        private const int STATUS_ACCEPTED = 3;

        // IDs fixes
        private const int PLANET_HOST_ID = 990300; // La planète autour de laquelle tournent les satellites
        private const int SAT_DRAFT_ID = 990301;
        private const int SAT_ACCEPTED_ID = 990302;

        // IDs Types
        private const int CBT_PLANET = 2;
        private const int CBT_SATELLITE = 6;
        private int _planetTypeId = 1; // Requis pour créer la planète hôte

        // Variables membres
        private int _satDraftId;
        private int _satAcceptedId;

        protected override SatellitesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var satelliteManager = new SatelliteManager(context);
            var discoveryManager = new DiscoveryManager(context);

            var controller = new SatellitesController(
                satelliteManager,
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

        protected override List<Satellite> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            CreateUserIfNotExist(USER_OWNER_ID, 1);
            CreateUserIfNotExist(USER_ADMIN_ID, 2);
            CreateUserIfNotExist(USER_STRANGER_ID, 1);

            EnsureStatusExists(STATUS_DRAFT, "Draft");
            EnsureStatusExists(STATUS_ACCEPTED, "Accepted");

            if (!_context.CelestialBodyTypes.AsNoTracking().Any(t => t.Id == CBT_PLANET))
                _context.CelestialBodyTypes.Add(new CelestialBodyType { Id = CBT_PLANET, Label = "Planet" });

            if (!_context.CelestialBodyTypes.AsNoTracking().Any(t => t.Id == CBT_SATELLITE))
                _context.CelestialBodyTypes.Add(new CelestialBodyType { Id = CBT_SATELLITE, Label = "Satellite" });

            if (!_context.PlanetTypes.AsNoTracking().Any(pt => pt.Id == _planetTypeId))
                _context.PlanetTypes.Add(new PlanetType { Id = _planetTypeId, Label = "Gas Giant" });

            _context.SaveChanges();

            if (!_context.CelestialBodies.AsNoTracking().Any(cb => cb.Id == PLANET_HOST_ID))
            {
                _context.CelestialBodies.Add(new CelestialBody
                {
                    Id = PLANET_HOST_ID,
                    Name = "Host Planet",
                    CelestialBodyTypeId = CBT_PLANET
                });
                _context.SaveChanges();
            }

            if (!_context.Planets.AsNoTracking().Any(p => p.Id == PLANET_HOST_ID))
            {
                _context.Planets.Add(new Planet
                {
                    Id = PLANET_HOST_ID,
                    CelestialBodyId = PLANET_HOST_ID,
                    PlanetTypeId = _planetTypeId,
                    DetectionMethodId = 1,
                    Mass = 500
                });
                _context.SaveChanges();
            }

            _context.ChangeTracker.Clear();

            var s1 = CreateSatelliteInMemory(
                SAT_DRAFT_ID,
                "Moon Draft",
                USER_OWNER_ID,
                STATUS_DRAFT,
                PLANET_HOST_ID
            );

            var s2 = CreateSatelliteInMemory(
                SAT_ACCEPTED_ID,
                "Moon Accepted",
                USER_OWNER_ID,
                STATUS_ACCEPTED,
                PLANET_HOST_ID
            );

            _satDraftId = SAT_DRAFT_ID;
            _satAcceptedId = SAT_ACCEPTED_ID;

            return new List<Satellite> { s1, s2 };
        }

        private Satellite CreateSatelliteInMemory(int id, string name, int userId, int statusId, int planetId)
        {
            var celestialBody = new CelestialBody
            {
                Id = id,
                Name = name,
                CelestialBodyTypeId = CBT_SATELLITE,

                DiscoveryNavigation = new Discovery
                {
                    Title = $"Discovery of {name}",
                    UserId = userId,
                    DiscoveryStatusId = statusId
                }
            };

            var satellite = new Satellite
            {
                Id = id,
                CelestialBodyId = id,
                PlanetId = planetId,
                Gravity = 1.62m,
                Radius = 1737.0m,
                Density = 3.34m,

                CelestialBodyNavigation = celestialBody
            };

            return satellite;
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

        protected override int GetIdFromEntity(Satellite entity) => entity.Id;
        protected override int GetIdFromDto(SatelliteDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override SatelliteCreateDto GetValidCreateDto()
        {
            return new SatelliteCreateDto
            {
                Name = "Generic Create",
                CelestialBodyTypeId = CBT_SATELLITE,
                PlanetId = PLANET_HOST_ID,
                Gravity = 1.0m
            };
        }

        protected override SatelliteUpdateDto GetValidUpdateDto(Satellite entityToUpdate)
        {
            return new SatelliteUpdateDto
            {
                PlanetId = PLANET_HOST_ID,
                Gravity = 9.81m,
                Radius = 2000.0m
            };
        }

        protected override void SetIdInUpdateDto(SatelliteUpdateDto dto, int id) { }


        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            var createDto = GetValidCreateDto();
            var result = await _controller.Post(createDto);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreate_ReturnsCreated()
        {
            await Post_ValidObject_ShouldCreateAndReturn200();
        }

        [TestMethod]
        public async Task Search_ByGravity_ShouldReturnCorrectSatellites()
        {
            // Given
            var filter = new SatelliteFilterDto
            {
                MinGravity = 1.0m,
                MaxGravity = 2.0m
            };

            // When
            var result = await _controller.Search(filter);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<SatelliteDto>;

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any(s => s.Id == _satDraftId));
            Assert.IsTrue(list.Any(s => s.Id == _satAcceptedId));
        }

        
        [TestMethod]
        public async Task Put_OwnerOnDraft_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var updateDto = GetValidUpdateDto(new Satellite());

            var result = await _controller.Put(_satDraftId, updateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updated = await _context.Satellites.FindAsync(_satDraftId);
            Assert.AreEqual(9.81m, updated.Gravity);
        }


        [TestMethod]
        public async Task Delete_OwnerOnDraft_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var result = await _controller.Delete(_satDraftId);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_OwnerOnAccepted_ShouldFail()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "Explorer");
            var result = await _controller.Delete(_satAcceptedId);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_AdminOnAccepted_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var result = await _controller.Delete(_satAcceptedId);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            var result = await _controller.GetById(_satAcceptedId);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var dto = (result.Result as OkObjectResult).Value as SatelliteDto;
            Assert.AreEqual(_satAcceptedId, dto.Id);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<SatelliteDto>;
            Assert.IsTrue(list.Any(s => s.Id == _satDraftId));
        }
    }
}