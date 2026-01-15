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
    public class StarsControllerTestsMock : CrudControllerMockTests<StarsController, Star, StarDto, StarDto, StarCreateDto, StarUpdateDto, int>
    {
        private Mock<IStarRepository> _mockStarRepository;
        private Mock<IDiscoveryRepository> _mockDiscoveryRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "Admin");
        }

        protected override StarsController CreateController(Mock<IReadableRepository<Star, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockStarRepository = new Mock<IStarRepository>();
            _mockDiscoveryRepository = new Mock<IDiscoveryRepository>();

            _mockCrudRepository = _mockStarRepository.As<ICrudRepository<Star, int>>();
            _mockRepository = _mockStarRepository.As<IReadableRepository<Star, int>>();

            return new StarsController(_mockStarRepository.Object, _mockDiscoveryRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(StarUpdateDto dto, int id)
        {
        }


        protected override List<Star> GetSampleEntities() => new List<Star>
        {
            new Star
            {
                Id = 1,
                CelestialBodyId = 10,
                CelestialBodyNavigation = new CelestialBody { Name = "Sirius" },
                SpectralClassNavigation = new SpectralClass { Label = "A" }
            },
            new Star
            {
                Id = 2,
                CelestialBodyId = 11,
                CelestialBodyNavigation = new CelestialBody { Name = "Vega" },
                SpectralClassNavigation = new SpectralClass { Label = "A" }
            }
        };

        protected override Star GetSampleEntity() => new Star
        {
            Id = 1,
            CelestialBodyId = 10,
            CelestialBodyNavigation = new CelestialBody { Name = "Sirius" },
            SpectralClassNavigation = new SpectralClass { Label = "A" }
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override StarCreateDto GetValidCreateDto() => new StarCreateDto
        {
            Name = "Betelgeuse",
            SpectralClassId = 1
        };

        protected override StarUpdateDto GetValidUpdateDto() => new StarUpdateDto
        {
            SpectralClassId = 2,
            Distance = 100
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
        public new async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var updateDto = GetValidUpdateDto();
            var entity = GetSampleEntity();

            _mockStarRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockStarRepository.Setup(r => r.UpdateAsync(It.IsAny<Star>(), It.IsAny<Star>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, updateDto);

            // Then
            _mockStarRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockStarRepository.Verify(r => r.UpdateAsync(It.IsAny<Star>(), It.IsAny<Star>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public new async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var entity = GetSampleEntity();

            _mockStarRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockStarRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockStarRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockStarRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
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
            _mockStarRepository.Verify(r => r.AddAsync(It.IsAny<Star>()), Times.Never);
        }


        [TestMethod]
        public async Task Search_ShouldReturnFilteredList()
        {
            // Given
            var filter = new StarFilterDto { Name = "Sirius", MinDistance = 5 };
            var entities = GetSampleEntities();

            _mockStarRepository.Setup(r => r.SearchAsync(
                filter.Name,
                filter.SpectralClassIds,
                filter.Constellation,
                filter.Designation,
                filter.BayerDesignation,
                filter.MinDistance, filter.MaxDistance,
                filter.MinLuminosity, filter.MaxLuminosity,
                filter.MinRadius, filter.MaxRadius,
                filter.MinTemperature, filter.MaxTemperature
            )).ReturnsAsync(entities);

            // When
            var result = await _controller.Search(filter);

            // Then
            _mockStarRepository.Verify(r => r.SearchAsync(
                filter.Name, It.IsAny<IEnumerable<int>?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                filter.MinDistance, It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>()
            ), Times.Once);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var dtos = okResult.Value as IEnumerable<StarDto>;
            Assert.AreEqual(2, dtos.Count());
        }

        [TestMethod]
        public async Task Put_AsOwner_WithDraftStatus_ShouldBeAllowed()
        {
            // Given
            int userId = 10;
            int starId = 1;
            SetupHttpContext(userId, "User");

            var star = GetSampleEntity();
            star.Id = starId;
            star.CelestialBodyId = 50;

            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = userId, DiscoveryStatusId = 1 }
            };

            _mockStarRepository.Setup(r => r.GetByIdAsync(starId)).ReturnsAsync(star);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);
            _mockStarRepository.Setup(r => r.UpdateAsync(star, It.IsAny<Star>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(starId, GetValidUpdateDto());

            // Then
            _mockStarRepository.Verify(r => r.GetByIdAsync(starId), Times.Exactly(2));
            _mockStarRepository.Verify(r => r.UpdateAsync(star, It.IsAny<Star>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Put_AsOwner_WithValidatedStatus_ShouldReturnForbid()
        {
            // Given
            int userId = 10;
            int starId = 1;
            SetupHttpContext(userId, "User");

            var star = GetSampleEntity();
            star.Id = starId;
            star.CelestialBodyId = 50;

            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = userId, DiscoveryStatusId = 2 }
            };

            _mockStarRepository.Setup(r => r.GetByIdAsync(starId)).ReturnsAsync(star);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);

            // When
            var result = await _controller.Put(starId, GetValidUpdateDto());

            // Then
            _mockStarRepository.Verify(r => r.GetByIdAsync(starId), Times.Once);
            _mockStarRepository.Verify(r => r.UpdateAsync(It.IsAny<Star>(), It.IsAny<Star>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_NotOwner_ShouldReturnForbid()
        {
            // Given
            int userId = 10;
            int otherUserId = 20;
            int starId = 1;
            SetupHttpContext(userId, "User");

            var star = GetSampleEntity();
            star.CelestialBodyId = 50;

            var discoveries = new List<Discovery>
            {
                new Discovery { CelestialBodyId = 50, UserId = otherUserId, DiscoveryStatusId = 1 }
            };

            _mockStarRepository.Setup(r => r.GetByIdAsync(starId)).ReturnsAsync(star);
            _mockDiscoveryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(discoveries);

            // When
            var result = await _controller.Delete(starId);

            // Then
            _mockStarRepository.Verify(r => r.DeleteAsync(It.IsAny<Star>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}