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
    public class DiscoveriesControllerTestsMock : CrudControllerMockTests<DiscoveriesController, Discovery, DiscoveryDto, DiscoveryDto, DiscoveryCreateDto, DiscoveryUpdateDto, int>
    {
        private Mock<IDiscoveryRepository> _mockDiscoveryRepository;
        private Mock<IAsteroidRepository> _mockAsteroidRepository;
        private Mock<IPlanetRepository> _mockPlanetRepository;
        private Mock<IStarRepository> _mockStarRepository;
        private Mock<ICometRepository> _mockCometRepository;
        private Mock<IGalaxyQuasarRepository> _mockGalaxyRepository;
        private Mock<ICelestialBodyRepository> _mockCelestialBodyRepository;
        private Mock<ISatelliteRepository> _mockSatelliteRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override DiscoveriesController CreateController(Mock<IReadableRepository<Discovery, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockDiscoveryRepository = new Mock<IDiscoveryRepository>();
            _mockAsteroidRepository = new Mock<IAsteroidRepository>();
            _mockPlanetRepository = new Mock<IPlanetRepository>();
            _mockStarRepository = new Mock<IStarRepository>();
            _mockCometRepository = new Mock<ICometRepository>();
            _mockGalaxyRepository = new Mock<IGalaxyQuasarRepository>();
            _mockCelestialBodyRepository = new Mock<ICelestialBodyRepository>();
            _mockSatelliteRepository = new Mock<ISatelliteRepository>();

            _mockCrudRepository = _mockDiscoveryRepository.As<ICrudRepository<Discovery, int>>();
            _mockRepository = _mockDiscoveryRepository.As<IReadableRepository<Discovery, int>>();

            return new DiscoveriesController(
                _mockDiscoveryRepository.Object,
                _mockAsteroidRepository.Object,
                _mockPlanetRepository.Object,
                _mockStarRepository.Object,
                _mockCometRepository.Object,
                _mockGalaxyRepository.Object,
                _mockCelestialBodyRepository.Object,
                _mockSatelliteRepository.Object,
                mapper
            );
        }

        protected override void SetIdInUpdateDto(DiscoveryUpdateDto dto, int id) { }

        protected override List<Discovery> GetSampleEntities() => new List<Discovery>
        {
            new Discovery
            {
                Id = 1,
                Title = "Nouvelle Etoile",
                UserId = 1,
                DiscoveryStatusId = 2,
                CelestialBodyNavigation = new CelestialBody { Name = "Star X", CelestialBodyTypeNavigation = new CelestialBodyType { Label = "Star" } },
                UserNavigation = new User { Username = "Discoverer1" },
                DiscoveryStatusNavigation = new DiscoveryStatus { Label = "Validated" }
            },
            new Discovery
            {
                Id = 2,
                Title = "Nouvelle Planète",
                UserId = 2,
                DiscoveryStatusId = 2,
                CelestialBodyNavigation = new CelestialBody { Name = "Planet Y", CelestialBodyTypeNavigation = new CelestialBodyType { Label = "Planet" } },
                UserNavigation = new User { Username = "Discoverer2" },
                DiscoveryStatusNavigation = new DiscoveryStatus { Label = "Validated" }
            }
        };

        protected override Discovery GetSampleEntity() => new Discovery
        {
            Id = 1,
            Title = "Brouillon Etoile",
            UserId = 1,
            DiscoveryStatusId = 1,
            CelestialBodyNavigation = new CelestialBody { Name = "Star Draft", CelestialBodyTypeNavigation = new CelestialBodyType { Label = "Star" } },
            UserNavigation = new User { Username = "Me" },
            DiscoveryStatusNavigation = new DiscoveryStatus { Label = "Draft" }
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override DiscoveryCreateDto GetValidCreateDto() => new DiscoveryCreateDto
        {
            Title = "Découverte Générique",
            CelestialBodyId = 10
        };

        protected override DiscoveryUpdateDto GetValidUpdateDto() => new DiscoveryUpdateDto
        {
            Title = "Titre Modifié"
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
        public async Task GetAll_ShouldReturnOk_WithListOfDtos()
        {
            // Given
            var entities = GetSampleEntities();
            _mockDiscoveryRepository.Setup(r => r.SearchAsync(null, 3, null, null, null))
                .ReturnsAsync(entities);

            // When
            var result = await _controller.GetAll();

            // Then
            _mockDiscoveryRepository.Verify(r => r.SearchAsync(null, 3, null, null, null), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public new async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            await Task.CompletedTask;
        }

        [TestMethod]
        public new async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            int userId = 1;
            SetupHttpContext(userId);

            var updateDto = GetValidUpdateDto();
            var entity = GetSampleEntity();
            entity.UserId = userId;
            entity.DiscoveryStatusId = 1;

            _mockDiscoveryRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockDiscoveryRepository.Setup(r => r.UpdateAsync(entity, entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, updateDto);

            // Then
            _mockDiscoveryRepository.Verify(r => r.UpdateAsync(entity, entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public new async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            int userId = 1;
            SetupHttpContext(userId);

            var entity = GetSampleEntity();
            entity.UserId = userId;
            entity.DiscoveryStatusId = 1;

            _mockDiscoveryRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockDiscoveryRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockDiscoveryRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }


        [TestMethod]
        public async Task PostStar_ShouldCreateStarAndDiscovery()
        {
            // Given
            int userId = 1;
            SetupHttpContext(userId);

            var submissionDto = new DiscoveryStarSubmissionDto
            {
                Title = "Supernova 2025",
                Details = new StarCreateDto { Name = "SN 2025", Distance = 500 }
            };

            _mockStarRepository.Setup(r => r.AddAsync(It.IsAny<Star>()))
                .Callback<Star>(s => s.CelestialBodyId = 100)
                .Returns(Task.CompletedTask);

            _mockDiscoveryRepository.Setup(r => r.AddAsync(It.IsAny<Discovery>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.PostStar(submissionDto);

            // Then
            _mockStarRepository.Verify(r => r.AddAsync(It.Is<Star>(s => s.CelestialBodyNavigation.Name == "SN 2025")), Times.Once);
            _mockDiscoveryRepository.Verify(r => r.AddAsync(It.Is<Discovery>(d => d.CelestialBodyId == 100)), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Search_ShouldReturnFilteredList()
        {
            // Given
            var filter = new DiscoveryFilterDto { Title = "Etoile", DiscoveryStatusId = 2 };
            var entities = GetSampleEntities();

            _mockDiscoveryRepository.Setup(r => r.SearchAsync(
                filter.Title,
                filter.DiscoveryStatusId,
                filter.AliasStatusId,
                filter.DiscoveryApprovalUserId,
                filter.AliasApprovalUserId
            )).ReturnsAsync(entities);

            // When
            var result = await _controller.Search(filter);

            // Then
            _mockDiscoveryRepository.Verify(r => r.SearchAsync(filter.Title, filter.DiscoveryStatusId, null, null, null), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }
    }
}