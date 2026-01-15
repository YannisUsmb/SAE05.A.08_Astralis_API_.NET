using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class PlanetsControllerTestsMock : CrudControllerMockTests<PlanetsController, Planet, PlanetDto, PlanetDto, PlanetCreateDto, PlanetUpdateDto, int>
    {
        private Mock<IPlanetRepository> _mockPlanetRepository;
        private Mock<IDiscoveryRepository> _mockDiscoveryRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "Admin");
        }

        protected override PlanetsController CreateController(Mock<IReadableRepository<Planet, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockPlanetRepository = new Mock<IPlanetRepository>();
            _mockDiscoveryRepository = new Mock<IDiscoveryRepository>();

            _mockCrudRepository = _mockPlanetRepository.As<ICrudRepository<Planet, int>>();
            _mockRepository = _mockPlanetRepository.As<IReadableRepository<Planet, int>>();

            return new PlanetsController(_mockPlanetRepository.Object, _mockDiscoveryRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(PlanetUpdateDto dto, int id)
        {
        }

        protected override List<Planet> GetSampleEntities() => new List<Planet>
        {
            new Planet
            {
                Id = 1,
                CelestialBodyId = 10,
                PlanetTypeId = 1,
                DetectionMethodId = 1,
                CelestialBodyNavigation = new CelestialBody { Name = "Mars" },
                PlanetTypeNavigation = new PlanetType { Label = "Terrestrial" },
                DetectionMethodNavigation = new DetectionMethod { Label = "Direct Imaging" }
            },
            new Planet
            {
                Id = 2,
                CelestialBodyId = 11,
                PlanetTypeId = 2,
                DetectionMethodId = 2,
                CelestialBodyNavigation = new CelestialBody { Name = "Jupiter" },
                PlanetTypeNavigation = new PlanetType { Label = "Gas Giant" },
                DetectionMethodNavigation = new DetectionMethod { Label = "Transit" }
            }
        };

        protected override Planet GetSampleEntity() => new Planet
        {
            Id = 1,
            CelestialBodyId = 10,
            PlanetTypeId = 1,
            DetectionMethodId = 1,
            CelestialBodyNavigation = new CelestialBody { Name = "Mars" },
            PlanetTypeNavigation = new PlanetType { Label = "Terrestrial" },
            DetectionMethodNavigation = new DetectionMethod { Label = "Direct Imaging" }
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override PlanetCreateDto GetValidCreateDto() => new PlanetCreateDto
        {
            Name = "Kepler-186f",
            PlanetTypeId = 1,
            DetectionMethodId = 1,
            Mass = 1.1m
        };

        protected override PlanetUpdateDto GetValidUpdateDto() => new PlanetUpdateDto
        {
            PlanetTypeId = 2,
            DetectionMethodId = 1,
            Mass = 1.2m
        };

        private void SetupHttpContext(int userId, string role = "User")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            if (_controller != null)
                _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        }

        [TestMethod]
        public new async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            var createDto = GetValidCreateDto();

            // When
            var result = await _controller.Post(createDto);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            _mockPlanetRepository.Verify(r => r.AddAsync(It.IsAny<Planet>()), Times.Never);
        }

        [TestMethod]
        public new async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var updateDto = GetValidUpdateDto();
            var entity = GetSampleEntity();

            _mockPlanetRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockPlanetRepository.Setup(r => r.UpdateAsync(It.IsAny<Planet>(), It.IsAny<Planet>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, updateDto);

            // Then
            _mockPlanetRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockPlanetRepository.Verify(r => r.UpdateAsync(It.IsAny<Planet>(), It.IsAny<Planet>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public new async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var entity = GetSampleEntity();

            _mockPlanetRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockPlanetRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockPlanetRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockPlanetRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Search_ShouldReturnFilteredList()
        {
            // Given
            var filter = new PlanetFilterDto { Name = "Mars", MinMass = 0.5m };
            var entities = GetSampleEntities();

            _mockPlanetRepository.Setup(r => r.SearchAsync(
                filter.Name,
                filter.PlanetTypeIds,
                filter.DetectionMethodIds,
                filter.MinDistance, filter.MaxDistance,
                filter.MinMass, filter.MaxMass,
                filter.MinRadius, filter.MaxRadius,
                filter.MinDiscoveryYear, filter.MaxDiscoveryYear,
                filter.MinEccentricity, filter.MaxEccentricity,
                filter.MinStellarMagnitude, filter.MaxStellarMagnitude
            )).ReturnsAsync(entities);

            // When
            var result = await _controller.Search(filter);

            // Then
            _mockPlanetRepository.Verify(r => r.SearchAsync(
                filter.Name,
                It.IsAny<IEnumerable<int>?>(), It.IsAny<IEnumerable<int>?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                filter.MinMass, It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>()
            ), Times.Once);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var dtos = okResult.Value as IEnumerable<PlanetDto>;
            Assert.AreEqual(2, dtos.Count());
        }

        [TestMethod]
        public async Task Put_AsOwner_WithDraftStatus_ShouldBeAllowed()
        {
            // Given
            int userId = 10;
            int planetId = 1;
            SetupHttpContext(userId, "User");

            var planet = GetSampleEntity();
            planet.Id = planetId;
            planet.CelestialBodyId = 50;

            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = userId, DiscoveryStatusId = 1 }
            };

            _mockPlanetRepository.Setup(r => r.GetByIdAsync(planetId)).ReturnsAsync(planet);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);
            _mockPlanetRepository.Setup(r => r.UpdateAsync(planet, It.IsAny<Planet>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(planetId, GetValidUpdateDto());

            // Then
            _mockPlanetRepository.Verify(r => r.GetByIdAsync(planetId), Times.Exactly(2));
            _mockPlanetRepository.Verify(r => r.UpdateAsync(planet, It.IsAny<Planet>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_NotOwner_ShouldReturnForbid()
        {
            // Given
            int userId = 10;
            int otherUserId = 20;
            int planetId = 1;
            SetupHttpContext(userId, "User");

            var planet = GetSampleEntity();
            planet.CelestialBodyId = 50;
            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = otherUserId, DiscoveryStatusId = 1 }
            };

            _mockPlanetRepository.Setup(r => r.GetByIdAsync(planetId)).ReturnsAsync(planet);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);

            // When
            var result = await _controller.Delete(planetId);

            // Then
            _mockPlanetRepository.Verify(r => r.DeleteAsync(It.IsAny<Planet>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}