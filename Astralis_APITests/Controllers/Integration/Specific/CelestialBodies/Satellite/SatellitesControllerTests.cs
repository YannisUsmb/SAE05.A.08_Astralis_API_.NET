//WAIT FOR THE SATELLITE POST IN DISCOVERY CONTROLLER


//using Astralis.Shared.DTOs;
//using Astralis_API.Controllers;
//using Astralis_API.Models.DataManager;
//using Astralis_API.Models.EntityFramework;
//using AutoMapper;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;

//namespace Astralis_APITests.Controllers
//{
//    [TestClass]
//    public class SatelliteControllerTests
//        : CrudControllerTests<SatellitesController, Satellite, SatelliteDto, SatelliteDto, SatelliteCreateDto, SatelliteUpdateDto, int>
//    {
//        private const int USER_OWNER_ID = 5001;
//        private const int USER_ADMIN_ID = 5002;
//        private const int USER_HACKER_ID = 6586;

//        private const string REF_DRAFT = "Sat-Draft";
//        private const string REF_ACCEPTED = "Sat-Accepted";
//        private const string REF_OTHER = "Sat-Other";

//        private int _satDraftId;
//        private int _satAcceptedId;
//        private int _satOtherUserId;
//        private int _hostPlanetId;

//        protected override SatellitesController CreateController(AstralisDbContext context, IMapper mapper)
//        {
//            var satelliteRepo = new SatelliteManager(context);
//            var discoveryRepo = new DiscoveryManager(context);

//            var controller = new SatellitesController(satelliteRepo, discoveryRepo, mapper);
//            SetupUserContext(controller, USER_OWNER_ID, "Client");

//            return controller;
//        }

//        private void SetupUserContext(ControllerBase controller, int userId, string role)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
//                new Claim(ClaimTypes.Role, role),
//                new Claim(ClaimTypes.Name, "TestUser")
//            };
//            var identity = new ClaimsIdentity(claims, "TestAuth");
//            var principal = new ClaimsPrincipal(identity);

//            controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = new DefaultHttpContext { User = principal }
//            };
//        }

//        protected override List<Satellite> GetSampleEntities()
//        {
//            var roleClient = GetOrCreateRole("Client");
//            var roleAdmin = GetOrCreateRole("Admin");

//            CreateUserIfNotExist(USER_OWNER_ID, "Owner", roleClient.Id);
//            CreateUserIfNotExist(USER_ADMIN_ID, "Admin", roleAdmin.Id);
//            CreateUserIfNotExist(USER_HACKER_ID, "Hacker", roleClient.Id);

//            var statusDraft = GetOrCreateStatus(1, "Draft");
//            var statusAccepted = GetOrCreateStatus(3, "Accepted");

//            var typeBodySatellite = GetOrCreateBodyType("Satellite");
//            var typeBodyPlanet = GetOrCreateBodyType("Planet");

//            _context.SaveChanges();
//            var hostPlanet = GetOrCreateHostPlanet("Saturn-Test", typeBodyPlanet.Id);
//            _hostPlanetId = hostPlanet.Id;

//            var list = new List<Satellite>();

//            list.Add(PrepareSatelliteWithDiscovery(REF_DRAFT, "Moon_Draft", typeBodySatellite.Id, USER_OWNER_ID, statusDraft.Id, hostPlanet.Id));
//            list.Add(PrepareSatelliteWithDiscovery(REF_ACCEPTED, "Moon_Accepted", typeBodySatellite.Id, USER_OWNER_ID, statusAccepted.Id, hostPlanet.Id));
//            list.Add(PrepareSatelliteWithDiscovery(REF_OTHER, "Moon_Other", typeBodySatellite.Id, USER_HACKER_ID, statusDraft.Id, hostPlanet.Id));

//            return list;
//        }


//        private Planet GetOrCreateHostPlanet(string name, int typeId)
//        {
//            var body = _context.CelestialBodies.FirstOrDefault(b => b.Name == name);
//            if (body == null)
//            {
//                body = new CelestialBody { Name = name, CelestialBodyTypeId = typeId, Alias = name };
//                _context.CelestialBodies.Add(body);
//                _context.SaveChanges();
//            }

//            var planet = _context.Planets.FirstOrDefault(p => p.CelestialBodyId == body.Id);
//            if (planet == null)
//            {
//                planet = new Planet { CelestialBodyId = body.Id, Mass = 100, Distance = 100, PlanetTypeId = 1, DetectionMethodId = 1 };
//                _context.Planets.Add(planet);
//                _context.SaveChanges();
//            }
//            return planet;
//        }

//        private Satellite PrepareSatelliteWithDiscovery(string reference, string bodyName, int bodyTypeId, int userId, int statusId, int planetId)
//        {
//            var body = _context.CelestialBodies.FirstOrDefault(b => b.Name == bodyName);
//            if (body == null)
//            {
//                body = new CelestialBody { Name = bodyName, CelestialBodyTypeId = bodyTypeId, Alias = reference };
//                _context.CelestialBodies.Add(body);
//                _context.SaveChanges();
//            }

//            if (!_context.Discoveries.Any(d => d.CelestialBodyId == body.Id))
//            {
//                var disc = new Discovery
//                {
//                    Title = $"Disc {reference}",
//                    UserId = userId,
//                    CelestialBodyId = body.Id,
//                    DiscoveryStatusId = statusId
//                };
//                _context.Discoveries.Add(disc);
//                _context.SaveChanges();
//            }

//            return new Satellite
//            {
//                CelestialBodyId = body.Id,
//                PlanetId = planetId,
//                Gravity = 1.62m,
//                Radius = 1737.4m,
//                Density = 3.34m
//            };
//        }

//        private UserRole GetOrCreateRole(string label)
//        {
//            var r = _context.UserRoles.FirstOrDefault(x => x.Label == label);
//            if (r == null) { r = new UserRole { Label = label }; _context.UserRoles.Add(r); }
//            return r;
//        }
//        private void CreateUserIfNotExist(int id, string name, int roleId)
//        {
//            if (!_context.Users.Any(u => u.Id == id))
//                _context.Users.Add(new User { Id = id, Username = name, UserRoleId = roleId, Email = $"{name}@test.com", FirstName = name, LastName = "T", Password = "pwd", IsPremium = false });
//        }
//        private DiscoveryStatus GetOrCreateStatus(int id, string label)
//        {
//            var s = _context.DiscoveryStatuses.FirstOrDefault(x => x.Id == id);
//            if (s == null) { s = new DiscoveryStatus { Id = id, Label = label }; _context.DiscoveryStatuses.Add(s); }
//            return s;
//        }
//        private CelestialBodyType GetOrCreateBodyType(string label)
//        {
//            var t = _context.CelestialBodyTypes.FirstOrDefault(x => x.Label == label);
//            if (t == null) { t = new CelestialBodyType { Label = label }; _context.CelestialBodyTypes.Add(t); }
//            return t;
//        }

//        protected override int GetIdFromEntity(Satellite entity) => entity.Id;
//        protected override int GetIdFromDto(SatelliteDto dto) => dto.Id;
//        protected override int GetNonExistingId() => 9999999;

//        protected override SatelliteCreateDto GetValidCreateDto()
//        {
//            return new SatelliteCreateDto
//            {
//                Name = "Enceladus-New",
//                Alias = "SAT-NEW",
//                CelestialBodyTypeId = 1,
//                PlanetId = _hostPlanetId,
//                Gravity = 0.113m
//            };
//        }

//        protected override void SetIdInUpdateDto(SatelliteUpdateDto dto, int id) { }

//        protected override SatelliteUpdateDto GetValidUpdateDto(Satellite entityToUpdate)
//        {
//            return new SatelliteUpdateDto
//            {
//                PlanetId = entityToUpdate.PlanetId,
//                Gravity = 9.81m,
//                Density = 5.51m
//            };
//        }

//        [TestMethod]
//        public async Task Post_ValidObject_ShouldCreateAndReturn200()
//        {
//            var createDto = GetValidCreateDto();

//            var actionResult = await _controller.Post(createDto);

//            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult));

//            var badRequest = actionResult.Result as BadRequestObjectResult;
//            Assert.AreEqual("Cannot create a Satellite directly. Use the Discovery process.", badRequest?.Value);
//        }


//        [TestMethod]
//        public async Task Delete_ExistingId_ShouldDeleteAndReturn204()
//        {
//            await RefreshIds();
//            await base.Delete_ExistingId_ShouldDeleteAndReturn204();
//        }

//        private async Task RefreshIds()
//        {
//            var draft = await _context.Satellites.Include(s => s.CelestialBodyNavigation).FirstOrDefaultAsync(s => s.CelestialBodyNavigation.Alias == REF_DRAFT);
//            if (draft != null) _satDraftId = draft.Id;

//            var accepted = await _context.Satellites.Include(s => s.CelestialBodyNavigation).FirstOrDefaultAsync(s => s.CelestialBodyNavigation.Alias == REF_ACCEPTED);
//            if (accepted != null) _satAcceptedId = accepted.Id;

//            var other = await _context.Satellites.Include(s => s.CelestialBodyNavigation).FirstOrDefaultAsync(s => s.CelestialBodyNavigation.Alias == REF_OTHER);
//            if (other != null) _satOtherUserId = other.Id;

//            if (_hostPlanetId == 0)
//            {
//                var planet = await _context.Planets.FirstOrDefaultAsync();
//                if (planet != null) _hostPlanetId = planet.Id;
//            }
//        }

//        [TestMethod]
//        public async Task Put_Owner_AcceptedDiscovery_ShouldReturnForbidden()
//        {
//            await RefreshIds();
//            var updateDto = new SatelliteUpdateDto { PlanetId = _hostPlanetId, Gravity = 2.0m };
//            var actionResult = await _controller.Put(_satAcceptedId, updateDto);
//            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
//        }

//        [TestMethod]
//        public async Task Put_OtherUser_ShouldReturnForbidden()
//        {
//            await RefreshIds();
//            var updateDto = new SatelliteUpdateDto { PlanetId = _hostPlanetId, Gravity = 2.0m };
//            var actionResult = await _controller.Put(_satOtherUserId, updateDto);
//            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
//        }

//        [TestMethod]
//        public async Task Put_Admin_ShouldAlwaysSuccess()
//        {
//            await RefreshIds();
//            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");

//            var currentEntity = await _context.Satellites.AsNoTracking().FirstOrDefaultAsync(x => x.Id == _satAcceptedId);

//            var updateDto = new SatelliteUpdateDto
//            {
//                PlanetId = currentEntity.PlanetId,
//                Gravity = 99.99m,
//                Radius = currentEntity.Radius
//            };

//            var actionResult = await _controller.Put(_satAcceptedId, updateDto);

//            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

//            _context.ChangeTracker.Clear();
//            var updatedEntity = await _context.Satellites.FindAsync(_satAcceptedId);
//            Assert.AreEqual(99.99m, updatedEntity.Gravity);
//        }

//        [TestMethod]
//        public async Task Delete_Owner_AcceptedDiscovery_ShouldReturnForbidden()
//        {
//            await RefreshIds();
//            var actionResult = await _controller.Delete(_satAcceptedId);
//            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
//        }

//        [TestMethod]
//        public async Task Search_ShouldReturnOk()
//        {
//            var result = await _controller.Search(new SatelliteFilterDto { Name = "Draft" });
//            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
//        }
//    }
//}