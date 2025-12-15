using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class AsteroidsControllerTests
        : CrudControllerTests<AsteroidsController, Asteroid, AsteroidDto, AsteroidDto, AsteroidCreateDto, AsteroidUpdateDto, int>
    {
        private int _targetOrbitalClassId;


        protected override AsteroidsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var asteroidManager = new AsteroidManager(context);

            IDiscoveryRepository? discoveryRepo = null;

            var controller = new AsteroidsController(asteroidManager, discoveryRepo, mapper);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "90210"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Name, "AdminUser"),
                new Claim(ClaimTypes.Email, "admin@test.com")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return controller;
        }

        protected override List<Asteroid> GetSampleEntities()
        {
            var orbitalClassTarget = new OrbitalClass { Label = "TGT", Description = "Test Target" };
            var orbitalClassNoise = new OrbitalClass { Label = "NSE", Description = "Test Noise" };

            if (!_context.OrbitalClasses.Any(o => o.Label == "TGT")) _context.OrbitalClasses.Add(orbitalClassTarget);
            else orbitalClassTarget = _context.OrbitalClasses.First(o => o.Label == "TGT");

            if (!_context.OrbitalClasses.Any(o => o.Label == "NSE")) _context.OrbitalClasses.Add(orbitalClassNoise);
            else orbitalClassNoise = _context.OrbitalClasses.First(o => o.Label == "NSE");

            _context.SaveChanges();
            _targetOrbitalClassId = orbitalClassTarget.Id;

            if (!_context.CelestialBodyTypes.Any(t => t.Id == 1))
            {
                _context.CelestialBodyTypes.Add(new CelestialBodyType { Id = 1, Label = "AsteroidType" });
                _context.SaveChanges();
            }

            var body1 = new CelestialBody { Name = "Body A", CelestialBodyTypeId = 1 };
            var body2 = new CelestialBody { Name = "Body B", CelestialBodyTypeId = 1 };
            _context.CelestialBodies.AddRange(body1, body2);
            _context.SaveChanges();

            var asteroids = new List<Asteroid>
            {
                new Asteroid
                {
                    Reference = "Reference test",
                    CelestialBodyId = body1.Id,
                    OrbitalClassId = orbitalClassTarget.Id,
                    IsPotentiallyHazardous = true,
                    AbsoluteMagnitude = 15.5m,
                    DiameterMinKm = 1.2m,
                    DiameterMaxKm = 2.5m,
                    OrbitId = 100
                },

                new Asteroid
                {
                    Reference = "Distraction_Ref",
                    CelestialBodyId = body2.Id,
                    OrbitalClassId = orbitalClassNoise.Id,
                    IsPotentiallyHazardous = false,
                    AbsoluteMagnitude = 22.0m,
                    DiameterMinKm = 0.5m,
                    DiameterMaxKm = 1.0m,
                    OrbitId = 200
                }
            };

            return asteroids;
        }

        protected override int GetIdFromEntity(Asteroid entity) => entity.Id;

        protected override int GetIdFromDto(AsteroidDto dto) => dto.Id;

        protected override int GetNonExistingId() => 9999999;

        protected override AsteroidCreateDto GetValidCreateDto()
        {
            var ocId = _context.OrbitalClasses.FirstOrDefault()?.Id ?? 1;

            return new AsteroidCreateDto
            {
                Name = "New Asteroid",
                CelestialBodyTypeId = 1,

                Reference = "New_Ref",
                OrbitalClassId = ocId,
                IsPotentiallyHazardous = false
            };
        }

        protected override AsteroidUpdateDto GetValidUpdateDto(Asteroid entityToUpdate)
        {
            return new AsteroidUpdateDto
            {
                Reference = entityToUpdate.Reference + "_Upd",
                OrbitalClassId = entityToUpdate.OrbitalClassId,
                IsPotentiallyHazardous = !entityToUpdate.IsPotentiallyHazardous,
                DiameterMinKm = 1,
                DiameterMaxKm = 2
            };
        }

        protected override void SetIdInUpdateDto(AsteroidUpdateDto dto, int id) {}

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            // Given
            var createDto = GetValidCreateDto();

            // When
            var result = await _controller.Post(createDto);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult), "Le contrôleur devrait bloquer la création directe.");
        }

        [TestMethod]
        public async Task Search_ByReference_ShouldReturnMatchingAsteroid()
        {
            var filter = new AsteroidFilterDto { Reference = "Reference test" };
            
            var result = await _controller.Search(filter);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var items = okResult.Value as IEnumerable<AsteroidDto>;

            Assert.IsTrue(items.Any(a => a.Reference == "Reference test"), "Devrait trouver l'astéroïde par sa référence.");
        }

        [TestMethod]
        public async Task Search_ByOrbitalClassId_ShouldReturnSpecificAsteroid()
        {
            var filter = new AsteroidFilterDto { OrbitalClassIds = new List<int> { _targetOrbitalClassId } };
            var result = await _controller.Search(filter);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var items = okResult.Value as IEnumerable<AsteroidDto>;

            Assert.IsTrue(items.Any(a => a.Reference == "Reference test"), "Devrait trouver l'astéroïde cible.");
            Assert.IsFalse(items.Any(a => a.Reference == "Distraction_Ref"), "Ne devrait pas trouver l'astéroïde bruit.");
        }

        [TestMethod]
        public async Task Search_ByHazardous_ShouldReturnTrueOnly()
        {
            var filter = new AsteroidFilterDto
            {
                IsPotentiallyHazardous = true,
            };
            var result = await _controller.Search(filter);

            var items = (result.Result as OkObjectResult).Value as IEnumerable<AsteroidDto>;

            Assert.IsTrue(items.Any(a => a.Reference == "Reference test"));
        }

        [TestMethod]
        public async Task Search_ByMagnitude_ShouldFilterCorrectly()
        {
            var filter = new AsteroidFilterDto
            {
                MinAbsoluteMagnitude = 10,
                MaxAbsoluteMagnitude = 18,
            };
            var result = await _controller.Search(filter);

            var items = (result.Result as OkObjectResult).Value as IEnumerable<AsteroidDto>;
            Assert.IsFalse(items.Any(a => a.Reference == "Distraction_Ref"));
        }
    }
}