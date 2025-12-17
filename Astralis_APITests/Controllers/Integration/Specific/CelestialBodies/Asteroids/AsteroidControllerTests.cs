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
    public class AsteroidsControllerTests
        : CrudControllerTests<AsteroidsController, Asteroid, AsteroidDto, AsteroidDto, AsteroidCreateDto, AsteroidUpdateDto, int>
    {
        private const int TEST_ADMIN_ID = 1000;
        private const int TEST_OWNER_ID = 2000;
        private const int TEST_STRANGER_ID = 3000;

        private const int ROLE_ID_ADMIN = 2;
        private const int ROLE_ID_USER = 1;

        private const int STATUS_DRAFT = 1;
        private const int STATUS_VALIDATED = 2;

        private const int ASTEROID_DRAFT_ID = 990901;
        private const int ASTEROID_LOCKED_ID = 999002;

        private int _asteroidDraftId;
        private int _asteroidLockedId;
        private int _targetOrbitalClassId;

        protected override AsteroidsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var asteroidManager = new AsteroidManager(context);
            var discoveryManager = new DiscoveryManager(context);

            var controller = new AsteroidsController(asteroidManager, discoveryManager, mapper);
            SetupUserContext(controller, TEST_ADMIN_ID, "Admin");
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

        protected override List<Asteroid> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            // 1. Setup Utilisateurs
            CreateUserIfNotExist(TEST_ADMIN_ID, ROLE_ID_ADMIN);
            CreateUserIfNotExist(TEST_OWNER_ID, ROLE_ID_USER);
            CreateUserIfNotExist(TEST_STRANGER_ID, ROLE_ID_USER);

            // 2. Setup Status
            if (!_context.DiscoveryStatuses.AsNoTracking().Any(s => s.Id == STATUS_DRAFT))
                _context.DiscoveryStatuses.Add(new DiscoveryStatus { Id = STATUS_DRAFT, Label = "Draft" });

            if (!_context.DiscoveryStatuses.AsNoTracking().Any(s => s.Id == STATUS_VALIDATED))
                _context.DiscoveryStatuses.Add(new DiscoveryStatus { Id = STATUS_VALIDATED, Label = "Validated" });

            // 3. Setup CelestialBodyType
            if (!_context.CelestialBodyTypes.AsNoTracking().Any(t => t.Id == 1))
                _context.CelestialBodyTypes.Add(new CelestialBodyType { Id = 1, Label = "Asteroid" });

            // 4. Setup Orbital Class
            var ocTarget = _context.OrbitalClasses.AsNoTracking().FirstOrDefault(o => o.Label == "TGT");
            if (ocTarget == null)
            {
                ocTarget = new OrbitalClass { Label = "TGT", Description = "Target" };
                _context.OrbitalClasses.Add(ocTarget);
                _context.SaveChanges();
            }
            _targetOrbitalClassId = ocTarget.Id;

            _context.SaveChanges();

            // 5. SETUP DES ASTEROIDES
            // On utilise les ID comme CelestialBodyId aussi
            Asteroid a1 = GetOrCreateAsteroid(ASTEROID_DRAFT_ID, ASTEROID_DRAFT_ID, "Ref_Draft", true, ocTarget.Id);
            Asteroid a2 = GetOrCreateAsteroid(ASTEROID_LOCKED_ID, ASTEROID_LOCKED_ID, "Ref_Locked", false, ocTarget.Id);

            if (_context.ChangeTracker.HasChanges()) _context.SaveChanges();
            if (!_context.Discoveries.Any(d => d.CelestialBodyId == ASTEROID_DRAFT_ID))
            {
                _context.Discoveries.Add(new Discovery
                {
                    Title = "Discovery Draft",
                    CelestialBodyId = ASTEROID_DRAFT_ID,
                    UserId = TEST_OWNER_ID,
                    DiscoveryStatusId = STATUS_DRAFT
                });
            }

            if (!_context.Discoveries.Any(d => d.CelestialBodyId == ASTEROID_LOCKED_ID))
            {
                _context.Discoveries.Add(new Discovery
                {
                    Title = "Discovery Validated",
                    CelestialBodyId = ASTEROID_LOCKED_ID,
                    UserId = TEST_OWNER_ID,
                    DiscoveryStatusId = STATUS_VALIDATED
                });
            }

            _context.SaveChanges();

            _asteroidDraftId = ASTEROID_DRAFT_ID;
            _asteroidLockedId = ASTEROID_LOCKED_ID;

            return new List<Asteroid> { a1, a2 };
        }

        private Asteroid GetOrCreateAsteroid(int id, int bodyId, string reference, bool hazardous, int orbitalClassId)
        {
            var existingCheck = _context.Asteroids
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefault(a => a.Id == id);

            if (existingCheck != null)
            {
                var asteroidToUpdate = new Asteroid
                {
                    Id = id,
                    CelestialBodyId = bodyId,
                    Reference = reference,
                    OrbitalClassId = orbitalClassId,
                    IsPotentiallyHazardous = hazardous,
                    AbsoluteMagnitude = 10,
                    DiameterMinKm = 1,
                    DiameterMaxKm = 2
                };

                _context.Asteroids.Attach(asteroidToUpdate);
                _context.Entry(asteroidToUpdate).State = EntityState.Modified;

                return asteroidToUpdate;
            }
            else
            {
                var body = _context.CelestialBodies.FirstOrDefault(b => b.Id == id);
                if (body == null)
                {
                    body = new CelestialBody { Id = id, Name = reference, CelestialBodyTypeId = 1 };
                    _context.CelestialBodies.Add(body);
                    _context.SaveChanges();
                }

                var newAsteroid = new Asteroid
                {
                    Id = id,
                    CelestialBodyId = body.Id,
                    CelestialBodyNavigation = body,
                    Reference = reference,
                    OrbitalClassId = orbitalClassId,
                    IsPotentiallyHazardous = hazardous,
                    AbsoluteMagnitude = 10,
                    DiameterMinKm = 1,
                    DiameterMaxKm = 2
                };
                return newAsteroid;
            }
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

        protected override int GetIdFromEntity(Asteroid entity) => entity.Id;
        protected override int GetIdFromDto(AsteroidDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override AsteroidCreateDto GetValidCreateDto()
        {
            return new AsteroidCreateDto
            {
                Name = "Direct Create",
                CelestialBodyTypeId = 1,
                Reference = "Direct_Ref",
                OrbitalClassId = _targetOrbitalClassId,
                IsPotentiallyHazardous = false
            };
        }

        protected override AsteroidUpdateDto GetValidUpdateDto(Asteroid entityToUpdate)
        {
            bool hazardous = entityToUpdate.IsPotentiallyHazardous ?? false;

            return new AsteroidUpdateDto
            {
                Reference = entityToUpdate.Reference + "_Updated",
                OrbitalClassId = entityToUpdate.OrbitalClassId,
                IsPotentiallyHazardous = !hazardous,
                DiameterMinKm = 5,
                DiameterMaxKm = 10
            };
        }

        protected override void SetIdInUpdateDto(AsteroidUpdateDto dto, int id) { }

        [TestMethod]
        public async Task Search_ByReference_ShouldReturnMatchingAsteroid()
        {
            // Given
            var filter = new AsteroidFilterDto { Reference = "Ref_Draft" };

            // When
            var result = await _controller.Search(filter);

            // Then
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var items = okResult.Value as IEnumerable<AsteroidDto>;
            Assert.IsTrue(items.Any(a => a.Reference == "Ref_Draft"));
        }

        [TestMethod]
        public async Task Search_ByHazardous_ShouldReturnTrueOnly()
        {
            // Given
            var filter = new AsteroidFilterDto { IsPotentiallyHazardous = true };

            // When
            var result = await _controller.Search(filter);

            // Then
            var okResult = result.Result as OkObjectResult;
            var items = okResult.Value as IEnumerable<AsteroidDto>;
            Assert.IsTrue(items.All(a => a.IsPotentiallyHazardous == true));
            Assert.IsTrue(items.Any());
        }

        [TestMethod]
        public async Task Put_AsAdmin_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, TEST_ADMIN_ID, "Admin");
            _context.ChangeTracker.Clear();
            var asteroid = _context.Asteroids.First(a => a.Id == _asteroidLockedId);
            var updateDto = GetValidUpdateDto(asteroid);

            // When
            var result = await _controller.Put(_asteroidLockedId, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Put_AsOwner_DraftStatus_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, TEST_OWNER_ID, "User");
            _context.ChangeTracker.Clear();
            var asteroid = _context.Asteroids.First(a => a.Id == _asteroidDraftId);
            var updateDto = GetValidUpdateDto(asteroid);

            // When
            var result = await _controller.Put(_asteroidDraftId, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Put_AsOwner_ValidatedStatus_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, TEST_OWNER_ID, "User");
            _context.ChangeTracker.Clear();
            var asteroid = _context.Asteroids.AsNoTracking().First(a => a.Id == _asteroidLockedId);
            var updateDto = GetValidUpdateDto(asteroid);

            // When
            var result = await _controller.Put(_asteroidLockedId, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_AsStranger_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, TEST_STRANGER_ID, "User");
            _context.ChangeTracker.Clear();
            var asteroid = _context.Asteroids.AsNoTracking().First(a => a.Id == _asteroidDraftId);
            var updateDto = GetValidUpdateDto(asteroid);

            // When
            var result = await _controller.Put(_asteroidDraftId, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_AsAdmin_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, TEST_ADMIN_ID, "Admin");

            // When
            var result = await _controller.Delete(_asteroidLockedId);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_AsOwner_DraftStatus_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, TEST_OWNER_ID, "User");

            // When
            var result = await _controller.Delete(_asteroidDraftId);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_AsOwner_ValidatedStatus_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, TEST_OWNER_ID, "User");

            // When
            var result = await _controller.Delete(_asteroidLockedId);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_AsStranger_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, TEST_STRANGER_ID, "User");

            // When
            var result = await _controller.Delete(_asteroidDraftId);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Post_InvalidObject_ShouldReturn400()
        {
            // Post method is in the celestial body controller tests
        }
        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            // Post method is in the celestial body controller tests
        }
    }
}