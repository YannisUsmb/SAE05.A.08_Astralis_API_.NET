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
    public class PlanetControllerTests
        : CrudControllerTests<PlanetsController, Planet, PlanetDto, PlanetDto, PlanetCreateDto, PlanetUpdateDto, int>
    {
        private const int USER_OWNER_ID = 5001;
        private const int USER_ADMIN_ID = 5002;
        private const int USER_HACKER_ID = 6666;

        private const string REF_DRAFT = "Planet-Draft";
        private const string REF_ACCEPTED = "Planet-Accepted";
        private const string REF_OTHER = "Planet-Other";

        private int _planetDraftId;
        private int _planetAcceptedId;
        private int _planetOtherUserId;

        private DiscoveriesController _discoveryController;

        protected override PlanetsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var planetRepo = new PlanetManager(context);
            var discoveryRepo = new DiscoveryManager(context);
            var asteroidRepo = new AsteroidManager(context);
            var starRepo = new StarManager(context);
            var cometRepo = new CometManager(context);
            var galaxyRepo = new GalaxyQuasarManager(context);
            var celestialBodyRepo = new CelestialBodyManager(context);
            var satelliteRepo = new SatelliteManager(context);
            var planetController = new PlanetsController(planetRepo, discoveryRepo, mapper);
            SetupUserContext(planetController, USER_OWNER_ID, "Client");

            _discoveryController = new DiscoveriesController(
                discoveryRepo,
                asteroidRepo,
                planetRepo,
                starRepo,
                cometRepo,
                galaxyRepo,
                celestialBodyRepo,
                satelliteRepo,
                mapper
            );
            SetupUserContext(_discoveryController, USER_OWNER_ID, "Client");

            return planetController;
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

        protected override List<Planet> GetSampleEntities()
        {
            var roleClient = GetOrCreateRole("Client");
            var roleAdmin = GetOrCreateRole("Admin");

            CreateUserIfNotExist(USER_OWNER_ID, "Owner", roleClient.Id);
            CreateUserIfNotExist(USER_ADMIN_ID, "Admin", roleAdmin.Id);
            CreateUserIfNotExist(USER_HACKER_ID, "Hacker", roleClient.Id);

            var statusDraft = GetOrCreateStatus(1, "Draft");
            var statusAccepted = GetOrCreateStatus(3, "Accepted");

            var typeBodyPlanet = GetOrCreateBodyType("Planet");
            var typeGasGiant = GetOrCreatePlanetType("Gas Giant");
            var methodTransit = GetOrCreateDetectionMethod("Transit");

            _context.SaveChanges();

            var list = new List<Planet>();

            list.Add(PreparePlanetWithDiscovery(REF_DRAFT, "Body_P_Draft", typeBodyPlanet.Id, USER_OWNER_ID, statusDraft.Id, typeGasGiant.Id, methodTransit.Id));
            list.Add(PreparePlanetWithDiscovery(REF_ACCEPTED, "Body_P_Accepted", typeBodyPlanet.Id, USER_OWNER_ID, statusAccepted.Id, typeGasGiant.Id, methodTransit.Id));
            list.Add(PreparePlanetWithDiscovery(REF_OTHER, "Body_P_Other", typeBodyPlanet.Id, USER_HACKER_ID, statusDraft.Id, typeGasGiant.Id, methodTransit.Id));

            return list;
        }

        private Planet PreparePlanetWithDiscovery(string reference, string bodyName, int bodyTypeId, int userId, int statusId, int planetTypeId, int methodId)
        {
            var body = _context.CelestialBodies.FirstOrDefault(b => b.Name == bodyName);
            if (body == null)
            {
                body = new CelestialBody { Name = bodyName, CelestialBodyTypeId = bodyTypeId, Alias = reference };
                _context.CelestialBodies.Add(body);
                _context.SaveChanges();
            }

            if (!_context.Discoveries.Any(d => d.CelestialBodyId == body.Id))
            {
                var disc = new Discovery
                {
                    Title = $"Disc {reference}",
                    UserId = userId,
                    CelestialBodyId = body.Id,
                    DiscoveryStatusId = statusId
                };
                _context.Discoveries.Add(disc);
                _context.SaveChanges();
            }

            return new Planet
            {
                CelestialBodyId = body.Id,
                PlanetTypeId = planetTypeId,
                DetectionMethodId = methodId,
                Mass = 1.5m,
                Distance = 100.0m
            };
        }

        private UserRole GetOrCreateRole(string label)
        {
            var r = _context.UserRoles.FirstOrDefault(x => x.Label == label);
            if (r == null) { r = new UserRole { Label = label }; _context.UserRoles.Add(r); }
            return r;
        }
        private void CreateUserIfNotExist(int id, string name, int roleId)
        {
            if (!_context.Users.Any(u => u.Id == id))
                _context.Users.Add(new User { Id = id, Username = name, UserRoleId = roleId, Email = $"{name}@test.com", FirstName = name, LastName = "T", Password = "pwd", IsPremium = false });
        }
        private DiscoveryStatus GetOrCreateStatus(int id, string label)
        {
            var s = _context.DiscoveryStatuses.FirstOrDefault(x => x.Id == id);
            if (s == null) { s = new DiscoveryStatus { Id = id, Label = label }; _context.DiscoveryStatuses.Add(s); }
            return s;
        }
        private CelestialBodyType GetOrCreateBodyType(string label)
        {
            var t = _context.CelestialBodyTypes.FirstOrDefault(x => x.Label == label);
            if (t == null) { t = new CelestialBodyType { Label = label }; _context.CelestialBodyTypes.Add(t); }
            return t;
        }
        private PlanetType GetOrCreatePlanetType(string label)
        {
            var t = _context.PlanetTypes.FirstOrDefault(x => x.Label == label);
            if (t == null) { t = new PlanetType { Label = label, Description = "Test" }; _context.PlanetTypes.Add(t); }
            return t;
        }
        private DetectionMethod GetOrCreateDetectionMethod(string label)
        {
            var m = _context.DetectionMethods.FirstOrDefault(x => x.Label == label);
            if (m == null) { m = new DetectionMethod { Label = label, Description = "Test" }; _context.DetectionMethods.Add(m); }
            return m;
        }

        protected override int GetIdFromEntity(Planet entity) => entity.Id;
        protected override int GetIdFromDto(PlanetDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override PlanetCreateDto GetValidCreateDto() => new PlanetCreateDto();

        protected override void SetIdInUpdateDto(PlanetUpdateDto dto, int id) { }

        protected override PlanetUpdateDto GetValidUpdateDto(Planet entityToUpdate)
        {
            return new PlanetUpdateDto
            {
                PlanetTypeId = entityToUpdate.PlanetTypeId,
                DetectionMethodId = entityToUpdate.DetectionMethodId,
                Mass = 999.9m,
                Remark = "Updated via Test"
            };
        }


        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            var typePlanet = _context.PlanetTypes.First();
            var method = _context.DetectionMethods.First();
            var bodyType = _context.CelestialBodyTypes.First(x => x.Label == "Planet");

            var createDto = new PlanetCreateDto
            {
                Name = "Kepler-Test-Override",
                Alias = "KP-OVER-1",
                CelestialBodyTypeId = bodyType.Id,
                PlanetTypeId = typePlanet.Id,
                DetectionMethodId = method.Id,
                Distance = 150.5m,
                Mass = 12.0m,
                Radius = 2.5m,
                DiscoveryYear = 2023,
                Eccentricity = 0.05m,
                Temperature = "300K",
                Remark = "Created via Discovery Controller"
            };

            var submission = new DiscoveryPlanetSubmissionDto
            {
                Title = "Nouvelle découverte de planète",
                Details = createDto
            };

            var actionResult = await _discoveryController.PostPlanet(submission);

            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));

            var okResult = actionResult.Result as OkObjectResult;
            var discoveryDto = okResult?.Value as DiscoveryDto;

            Assert.IsNotNull(discoveryDto);
            Assert.IsTrue(discoveryDto.Id > 0);
        }

        [TestMethod]
        public async Task Delete_ExistingId_ShouldDeleteAndReturn204()
        {
            await RefreshIds();
            await base.Delete_ExistingId_ShouldDeleteAndReturn204();
        }

        private async Task RefreshIds()
        {
            var draft = await _context.Planets.Include(p => p.CelestialBodyNavigation).FirstOrDefaultAsync(p => p.CelestialBodyNavigation.Alias == REF_DRAFT);
            if (draft != null) _planetDraftId = draft.Id;

            var accepted = await _context.Planets.Include(p => p.CelestialBodyNavigation).FirstOrDefaultAsync(p => p.CelestialBodyNavigation.Alias == REF_ACCEPTED);
            if (accepted != null) _planetAcceptedId = accepted.Id;

            var other = await _context.Planets.Include(p => p.CelestialBodyNavigation).FirstOrDefaultAsync(p => p.CelestialBodyNavigation.Alias == REF_OTHER);
            if (other != null) _planetOtherUserId = other.Id;
        }

        [TestMethod]
        public async Task Put_Owner_AcceptedDiscovery_ShouldReturnForbidden()
        {
            await RefreshIds();
            var updateDto = new PlanetUpdateDto { Mass = 500m, PlanetTypeId = 1, DetectionMethodId = 1 };
            var actionResult = await _controller.Put(_planetAcceptedId, updateDto);
            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_OtherUser_ShouldReturnForbidden()
        {
            await RefreshIds();
            var updateDto = new PlanetUpdateDto { Mass = 500m, PlanetTypeId = 1, DetectionMethodId = 1 };
            var actionResult = await _controller.Put(_planetOtherUserId, updateDto);
            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_Admin_ShouldAlwaysSuccess()
        {
            await RefreshIds();
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");

            var currentEntity = await _context.Planets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == _planetAcceptedId);

            var updateDto = new PlanetUpdateDto
            {
                PlanetTypeId = currentEntity.PlanetTypeId,
                DetectionMethodId = currentEntity.DetectionMethodId,
                Remark = "Admin Override Remark"
            };

            var actionResult = await _controller.Put(_planetAcceptedId, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updatedEntity = await _context.Planets.FindAsync(_planetAcceptedId);
            Assert.AreEqual("Admin Override Remark", updatedEntity.Remark);
        }

        [TestMethod]
        public async Task Delete_Owner_AcceptedDiscovery_ShouldReturnForbidden()
        {
            await RefreshIds();
            var actionResult = await _controller.Delete(_planetAcceptedId);
            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Search_ShouldReturnOk()
        {
            var result = await _controller.Search(new PlanetFilterDto { Name = "Draft" });
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }
    }
}