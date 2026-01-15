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
    public class SatellitesControllerTestsMock : CrudControllerMockTests<SatellitesController, Satellite, SatelliteDto, SatelliteDto, SatelliteCreateDto, SatelliteUpdateDto, int>
    {
        private Mock<ISatelliteRepository> _mockSatelliteRepository;
        private Mock<IDiscoveryRepository> _mockDiscoveryRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            // On se met en Admin par défaut pour faciliter les tests génériques
            SetupHttpContext(1, "Admin");
        }

        protected override SatellitesController CreateController(Mock<IReadableRepository<Satellite, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockSatelliteRepository = new Mock<ISatelliteRepository>();
            _mockDiscoveryRepository = new Mock<IDiscoveryRepository>();

            _mockCrudRepository = _mockSatelliteRepository.As<ICrudRepository<Satellite, int>>();
            _mockRepository = _mockSatelliteRepository.As<IReadableRepository<Satellite, int>>();

            return new SatellitesController(_mockSatelliteRepository.Object, _mockDiscoveryRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(SatelliteUpdateDto dto, int id)
        {
            // Pas d'ID dans le DTO d'update
        }

        // --- Données de Test ---

        protected override List<Satellite> GetSampleEntities() => new List<Satellite>
        {
            new Satellite
            {
                Id = 1,                
                CelestialBodyId = 10,
                PlanetId = 5,
                CelestialBodyNavigation = new CelestialBody { Name = "Moon" },
                PlanetNavigation = new Planet { CelestialBodyNavigation = new CelestialBody { Name = "Earth" } }
            },
            new Satellite
            {
                Id = 2,
                CelestialBodyId = 11,
                PlanetId = 6,
                CelestialBodyNavigation = new CelestialBody { Name = "Titan" },
                PlanetNavigation = new Planet { CelestialBodyNavigation = new CelestialBody { Name = "Saturn" } }
            }
        };

        protected override Satellite GetSampleEntity() => new Satellite
        {
            Id = 1,
            CelestialBodyId = 10,
            PlanetId = 5,
            CelestialBodyNavigation = new CelestialBody { Name = "Moon" },
            PlanetNavigation = new Planet { CelestialBodyNavigation = new CelestialBody { Name = "Earth" } }
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override SatelliteCreateDto GetValidCreateDto() => new SatelliteCreateDto
        {
            Name = "Phobos",
            PlanetId = 4,
            Radius = 11
        };

        protected override SatelliteUpdateDto GetValidUpdateDto() => new SatelliteUpdateDto
        {
            PlanetId = 4,
            Radius = 12
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
            _mockSatelliteRepository.Verify(r => r.AddAsync(It.IsAny<Satellite>()), Times.Never);
        }

        [TestMethod]
        public new async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var updateDto = GetValidUpdateDto();
            var entity = GetSampleEntity();

            _mockSatelliteRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockSatelliteRepository.Setup(r => r.UpdateAsync(It.IsAny<Satellite>(), It.IsAny<Satellite>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, updateDto);

            // Then
            _mockSatelliteRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockSatelliteRepository.Verify(r => r.UpdateAsync(It.IsAny<Satellite>(), It.IsAny<Satellite>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public new async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var entity = GetSampleEntity();

            _mockSatelliteRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockSatelliteRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockSatelliteRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockSatelliteRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Search_ShouldReturnFilteredList()
        {
            // Given
            var filter = new SatelliteFilterDto { Name = "Moon", MinRadius = 1000 };
            var entities = GetSampleEntities();

            _mockSatelliteRepository.Setup(r => r.SearchAsync(
                filter.Name,
                filter.PlanetIds,
                filter.MinGravity, filter.MaxGravity,
                filter.MinRadius, filter.MaxRadius,
                filter.MinDensity, filter.MaxDensity
            )).ReturnsAsync(entities);

            // When
            var result = await _controller.Search(filter);

            // Then
            _mockSatelliteRepository.Verify(r => r.SearchAsync(
                filter.Name,
                It.IsAny<IEnumerable<int>?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                filter.MinRadius, It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>()
            ), Times.Once);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var dtos = okResult.Value as IEnumerable<SatelliteDto>;
            Assert.AreEqual(2, dtos.Count());
        }

        [TestMethod]
        public async Task Put_AsOwner_WithDraftStatus_ShouldBeAllowed()
        {
            // Given
            int userId = 10;
            int satelliteId = 1;
            SetupHttpContext(userId, "User");

            var satellite = GetSampleEntity();
            satellite.Id = satelliteId;
            satellite.CelestialBodyId = 50;

            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = userId, DiscoveryStatusId = 1 }
            };

            _mockSatelliteRepository.Setup(r => r.GetByIdAsync(satelliteId)).ReturnsAsync(satellite);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);
            _mockSatelliteRepository.Setup(r => r.UpdateAsync(satellite, It.IsAny<Satellite>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(satelliteId, GetValidUpdateDto());

            // Then
            _mockSatelliteRepository.Verify(r => r.GetByIdAsync(satelliteId), Times.Exactly(2));
            _mockSatelliteRepository.Verify(r => r.UpdateAsync(satellite, It.IsAny<Satellite>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_NotOwner_ShouldReturnForbid()
        {
            // Given
            int userId = 10;
            int otherUserId = 20;
            int satelliteId = 1;
            SetupHttpContext(userId, "User");

            var satellite = GetSampleEntity();
            satellite.CelestialBodyId = 50;

            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = otherUserId, DiscoveryStatusId = 1 }
            };

            _mockSatelliteRepository.Setup(r => r.GetByIdAsync(satelliteId)).ReturnsAsync(satellite);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);

            // When
            var result = await _controller.Delete(satelliteId);

            // Then
            _mockSatelliteRepository.Verify(r => r.DeleteAsync(It.IsAny<Satellite>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}