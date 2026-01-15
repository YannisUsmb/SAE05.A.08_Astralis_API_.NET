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
    public class AsteroidsControllerTestsMock : CrudControllerMockTests<AsteroidsController, Asteroid, AsteroidDto, AsteroidDto, AsteroidCreateDto, AsteroidUpdateDto, int>
    {
        private Mock<IAsteroidRepository> _mockAsteroidRepository;
        private Mock<IDiscoveryRepository> _mockDiscoveryRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "Admin");
        }

        protected override AsteroidsController CreateController(Mock<IReadableRepository<Asteroid, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockAsteroidRepository = new Mock<IAsteroidRepository>();
            _mockDiscoveryRepository = new Mock<IDiscoveryRepository>();

            _mockCrudRepository = _mockAsteroidRepository.As<ICrudRepository<Asteroid, int>>();
            _mockRepository = _mockAsteroidRepository.As<IReadableRepository<Asteroid, int>>();

            return new AsteroidsController(_mockAsteroidRepository.Object, _mockDiscoveryRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(AsteroidUpdateDto dto, int id)
        {
        }


        protected override List<Asteroid> GetSampleEntities() => new List<Asteroid>
        {
            new Asteroid
            {
                Id = 1,
                CelestialBodyId = 10,
                OrbitalClassId = 1,
                Reference = "Ceres",
                IsPotentiallyHazardous = false,
                CelestialBodyNavigation = new CelestialBody { Name = "Ceres" },
                OrbitalClassNavigation = new OrbitalClass { Label = "MBA" }
            },
            new Asteroid
            {
                Id = 2,
                CelestialBodyId = 11,
                OrbitalClassId = 2,
                Reference = "Eros",
                IsPotentiallyHazardous = true,
                CelestialBodyNavigation = new CelestialBody { Name = "Eros" },
                OrbitalClassNavigation = new OrbitalClass { Label = "NEA" }
            }
        };

        protected override Asteroid GetSampleEntity() => new Asteroid
        {
            Id = 1,
            CelestialBodyId = 10,
            OrbitalClassId = 1,
            Reference = "Ceres",
            IsPotentiallyHazardous = false,
            CelestialBodyNavigation = new CelestialBody { Name = "Ceres" },
            OrbitalClassNavigation = new OrbitalClass { Label = "MBA" }
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override AsteroidCreateDto GetValidCreateDto() => new AsteroidCreateDto
        {
            Name = "Vesta",
            OrbitalClassId = 1,
            DiameterMinKm = 500,
            DiameterMaxKm = 550
        };

        protected override AsteroidUpdateDto GetValidUpdateDto() => new AsteroidUpdateDto
        {
            OrbitalClassId = 2,
            Reference = "Vesta Updated",
            IsPotentiallyHazardous = false
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
            _mockAsteroidRepository.Verify(r => r.AddAsync(It.IsAny<Asteroid>()), Times.Never);
        }

        [TestMethod]
        public new async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var updateDto = GetValidUpdateDto();
            var entity = GetSampleEntity();

            _mockAsteroidRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockAsteroidRepository.Setup(r => r.UpdateAsync(It.IsAny<Asteroid>(), It.IsAny<Asteroid>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, updateDto);

            // Then
            _mockAsteroidRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockAsteroidRepository.Verify(r => r.UpdateAsync(It.IsAny<Asteroid>(), It.IsAny<Asteroid>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public new async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var entity = GetSampleEntity();

            _mockAsteroidRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockAsteroidRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockAsteroidRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockAsteroidRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Search_ShouldReturnFilteredList()
        {
            // Given
            var filter = new AsteroidFilterDto
            {
                Reference = "Ceres",
                IsPotentiallyHazardous = false,
                MinDiameter = 100
            };
            var entities = GetSampleEntities();

            _mockAsteroidRepository.Setup(r => r.SearchAsync(
                filter.Reference,
                filter.OrbitalClassIds,
                filter.IsPotentiallyHazardous,
                filter.OrbitId,
                filter.MinAbsoluteMagnitude, filter.MaxAbsoluteMagnitude,
                filter.MinDiameter, filter.MaxDiameter,
                filter.MinInclination, filter.MaxInclination,
                filter.MinSemiMajorAxis, filter.MaxSemiMajorAxis
            )).ReturnsAsync(entities);

            // When
            var result = await _controller.Search(filter);

            // Then
            _mockAsteroidRepository.Verify(r => r.SearchAsync(
                filter.Reference,
                It.IsAny<IEnumerable<int>?>(),
                filter.IsPotentiallyHazardous,
                It.IsAny<int?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                filter.MinDiameter, It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>()
            ), Times.Once);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var dtos = okResult.Value as IEnumerable<AsteroidDto>;
            Assert.AreEqual(2, dtos.Count());
        }

        [TestMethod]
        public async Task Put_AsOwner_WithDraftStatus_ShouldBeAllowed()
        {
            // Given
            int userId = 10;
            int asteroidId = 1;
            SetupHttpContext(userId, "User");

            var entity = GetSampleEntity();
            entity.Id = asteroidId;
            entity.CelestialBodyId = 50;
            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = userId, DiscoveryStatusId = 1 }
            };

            _mockAsteroidRepository.Setup(r => r.GetByIdAsync(asteroidId)).ReturnsAsync(entity);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);
            _mockAsteroidRepository.Setup(r => r.UpdateAsync(entity, It.IsAny<Asteroid>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(asteroidId, GetValidUpdateDto());

            // Then
            _mockAsteroidRepository.Verify(r => r.GetByIdAsync(asteroidId), Times.Exactly(2));
            _mockAsteroidRepository.Verify(r => r.UpdateAsync(entity, It.IsAny<Asteroid>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_NotOwner_ShouldReturnForbid()
        {
            // Given
            int userId = 10;
            int otherUserId = 20;
            int asteroidId = 1;
            SetupHttpContext(userId, "User");

            var entity = GetSampleEntity();
            entity.CelestialBodyId = 50;

            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = otherUserId, DiscoveryStatusId = 1 }
            };

            _mockAsteroidRepository.Setup(r => r.GetByIdAsync(asteroidId)).ReturnsAsync(entity);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);

            // When
            var result = await _controller.Delete(asteroidId);

            // Then
            _mockAsteroidRepository.Verify(r => r.DeleteAsync(It.IsAny<Asteroid>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}