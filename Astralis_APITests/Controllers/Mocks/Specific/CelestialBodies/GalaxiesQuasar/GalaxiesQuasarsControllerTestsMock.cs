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
    public class GalaxiesQuasarsControllerTestsMock : CrudControllerMockTests<GalaxyQuasarsController, GalaxyQuasar, GalaxyQuasarDto, GalaxyQuasarDto, GalaxyQuasarCreateDto, GalaxyQuasarUpdateDto, int>
    {
        private Mock<IGalaxyQuasarRepository> _mockGalaxyQuasarRepository;
        private Mock<IDiscoveryRepository> _mockDiscoveryRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            // Admin par défaut pour les tests génériques
            SetupHttpContext(1, "Admin");
        }

        protected override GalaxyQuasarsController CreateController(Mock<IReadableRepository<GalaxyQuasar, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockGalaxyQuasarRepository = new Mock<IGalaxyQuasarRepository>();
            _mockDiscoveryRepository = new Mock<IDiscoveryRepository>();

            _mockCrudRepository = _mockGalaxyQuasarRepository.As<ICrudRepository<GalaxyQuasar, int>>();
            _mockRepository = _mockGalaxyQuasarRepository.As<IReadableRepository<GalaxyQuasar, int>>();

            return new GalaxyQuasarsController(_mockGalaxyQuasarRepository.Object, _mockDiscoveryRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(GalaxyQuasarUpdateDto dto, int id)
        {
        }


        protected override List<GalaxyQuasar> GetSampleEntities() => new List<GalaxyQuasar>
        {
            new GalaxyQuasar
            {
                Id = 1,
                CelestialBodyId = 10,
                GalaxyQuasarClassId = 1,
                Reference = "M31",
                CelestialBodyNavigation = new CelestialBody { Name = "Andromeda" },
                GalaxyQuasarClassNavigation = new GalaxyQuasarClass { Label = "Spiral" }
            },
            new GalaxyQuasar
            {
                Id = 2,
                CelestialBodyId = 11,
                GalaxyQuasarClassId = 2,
                Reference = "3C 273",
                CelestialBodyNavigation = new CelestialBody { Name = "Quasar 3C 273" },
                GalaxyQuasarClassNavigation = new GalaxyQuasarClass { Label = "Quasar" }
            }
        };

        protected override GalaxyQuasar GetSampleEntity() => new GalaxyQuasar
        {
            Id = 1,
            CelestialBodyId = 10,
            GalaxyQuasarClassId = 1,
            Reference = "M31",
            CelestialBodyNavigation = new CelestialBody { Name = "Andromeda" },
            GalaxyQuasarClassNavigation = new GalaxyQuasarClass { Label = "Spiral" }
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override GalaxyQuasarCreateDto GetValidCreateDto() => new GalaxyQuasarCreateDto
        {
            Name = "Triangulum",
            GalaxyQuasarClassId = 1,
            RightAscension = 23.4m,
            Declination = 30.5m
        };

        protected override GalaxyQuasarUpdateDto GetValidUpdateDto() => new GalaxyQuasarUpdateDto
        {
            GalaxyQuasarClassId = 2,
            Redshift = 0.5m
        };

        // --- Helper Context ---
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
            _mockGalaxyQuasarRepository.Verify(r => r.AddAsync(It.IsAny<GalaxyQuasar>()), Times.Never);
        }

        [TestMethod]
        public new async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var updateDto = GetValidUpdateDto();
            var entity = GetSampleEntity();

            _mockGalaxyQuasarRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockGalaxyQuasarRepository.Setup(r => r.UpdateAsync(It.IsAny<GalaxyQuasar>(), It.IsAny<GalaxyQuasar>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, updateDto);

            // Then
            _mockGalaxyQuasarRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockGalaxyQuasarRepository.Verify(r => r.UpdateAsync(It.IsAny<GalaxyQuasar>(), It.IsAny<GalaxyQuasar>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public new async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var entity = GetSampleEntity();

            _mockGalaxyQuasarRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockGalaxyQuasarRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockGalaxyQuasarRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockGalaxyQuasarRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Search_ShouldReturnFilteredList()
        {
            // Given
            var filter = new GalaxyQuasarFilterDto { Reference = "M31", MinRedshift = 0.1m };
            var entities = GetSampleEntities();

            _mockGalaxyQuasarRepository.Setup(r => r.SearchAsync(
                filter.Reference,
                filter.GalaxyQuasarClassIds,
                filter.MinRightAscension, filter.MaxRightAscension,
                filter.MinDeclination, filter.MaxDeclination,
                filter.MinRedshift, filter.MaxRedshift,
                filter.MinRMagnitude, filter.MaxRMagnitude,
                filter.MinMjdObs, filter.MaxMjdObs
            )).ReturnsAsync(entities);

            // When
            var result = await _controller.Search(filter);

            // Then
            _mockGalaxyQuasarRepository.Verify(r => r.SearchAsync(
                filter.Reference,
                It.IsAny<IEnumerable<int>?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                filter.MinRedshift, It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<int?>(), It.IsAny<int?>()
            ), Times.Once);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var dtos = okResult.Value as IEnumerable<GalaxyQuasarDto>;
            Assert.AreEqual(2, dtos.Count());
        }

        [TestMethod]
        public async Task Put_AsOwner_WithDraftStatus_ShouldBeAllowed()
        {
            // Given
            int userId = 10;
            int gqId = 1;
            SetupHttpContext(userId, "User");

            var entity = GetSampleEntity();
            entity.Id = gqId;
            entity.CelestialBodyId = 50;

            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = userId, DiscoveryStatusId = 1 }
            };

            _mockGalaxyQuasarRepository.Setup(r => r.GetByIdAsync(gqId)).ReturnsAsync(entity);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);
            _mockGalaxyQuasarRepository.Setup(r => r.UpdateAsync(entity, It.IsAny<GalaxyQuasar>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(gqId, GetValidUpdateDto());

            // Then
            _mockGalaxyQuasarRepository.Verify(r => r.GetByIdAsync(gqId), Times.Exactly(2));
            _mockGalaxyQuasarRepository.Verify(r => r.UpdateAsync(entity, It.IsAny<GalaxyQuasar>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_NotOwner_ShouldReturnForbid()
        {
            // Given
            int userId = 10;
            int otherUserId = 20;
            int gqId = 1;
            SetupHttpContext(userId, "User");

            var entity = GetSampleEntity();
            entity.CelestialBodyId = 50;

            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = otherUserId, DiscoveryStatusId = 1 }
            };

            _mockGalaxyQuasarRepository.Setup(r => r.GetByIdAsync(gqId)).ReturnsAsync(entity);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);

            // When
            var result = await _controller.Delete(gqId);

            // Then
            _mockGalaxyQuasarRepository.Verify(r => r.DeleteAsync(It.IsAny<GalaxyQuasar>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}