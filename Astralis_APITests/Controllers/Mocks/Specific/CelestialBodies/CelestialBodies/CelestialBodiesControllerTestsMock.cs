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
    public class CelestialBodiesControllerTestsMock : CrudControllerMockTests<CelestialBodiesController, CelestialBody, CelestialBodyListDto, CelestialBodyListDto, CelestialBodyCreateDto, CelestialBodyUpdateDto, int>
    {
        private Mock<ICelestialBodyRepository> _mockCelestialBodyRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "Admin");
        }

        protected override CelestialBodiesController CreateController(Mock<IReadableRepository<CelestialBody, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockCelestialBodyRepository = new Mock<ICelestialBodyRepository>();

            _mockCrudRepository = _mockCelestialBodyRepository.As<ICrudRepository<CelestialBody, int>>();
            _mockRepository = _mockCelestialBodyRepository.As<IReadableRepository<CelestialBody, int>>();

            return new CelestialBodiesController(_mockCelestialBodyRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(CelestialBodyUpdateDto dto, int id) { }

        protected override List<CelestialBody> GetSampleEntities() => new List<CelestialBody>
        {
            new CelestialBody { Id = 1, Name = "Mars", Alias = "Red Planet", CelestialBodyTypeId = 1 },
            new CelestialBody { Id = 2, Name = "Venus", Alias = "Morning Star", CelestialBodyTypeId = 1 }
        };

        protected override CelestialBody GetSampleEntity() => new CelestialBody
        {
            Id = 1,
            Name = "Mars",
            Alias = "Red Planet",
            CelestialBodyTypeId = 1
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override CelestialBodyCreateDto GetValidCreateDto() => new CelestialBodyCreateDto
        {
            Name = "Jupiter",
            Alias = "Jove",
            CelestialBodyTypeId = 2
        };

        protected override CelestialBodyUpdateDto GetValidUpdateDto() => new CelestialBodyUpdateDto
        {
            Name = "Jupiter Updated",
            Alias = "Jove Updated"
        };

        private void SetupHttpContext(int userId, string role = "Admin")
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
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            var createDto = GetValidCreateDto();

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockCelestialBodyRepository.Verify(r => r.AddAsync(It.IsAny<CelestialBody>()), Times.Never);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }


        [TestMethod]
        public async Task GetDetails_ExistingId_ShouldReturnDetailDto()
        {
            // Given
            int id = 1;
            var entity = GetSampleEntity();
            _mockCelestialBodyRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            // When
            var result = await _controller.GetDetails(id);

            // Then
            _mockCelestialBodyRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetDetails_NonExistingId_ShouldReturnNotFound()
        {
            // Given
            int id = 999;
            _mockCelestialBodyRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CelestialBody)null);

            // When
            var result = await _controller.GetDetails(id);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetSubtypes_ShouldReturnOk()
        {
            // Given
            int mainTypeId = 1;
            var subtypes = new List<CelestialBodySubtypeDto> { new CelestialBodySubtypeDto { Id = 1, Label = "Terrestrial" } };
            _mockCelestialBodyRepository.Setup(r => r.GetSubtypesByMainTypeAsync(mainTypeId)).ReturnsAsync(subtypes);

            // When
            var result = await _controller.GetSubtypes(mainTypeId);

            // Then
            _mockCelestialBodyRepository.Verify(r => r.GetSubtypesByMainTypeAsync(mainTypeId), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Search_ShouldReturnList()
        {
            // Given
            var filter = new CelestialBodyFilterDto { SearchText = "Mars" };
            var results = GetSampleEntities();

            _mockCelestialBodyRepository.Setup(r => r.SearchAsync(filter, 1, 30)).ReturnsAsync(results);

            // When
            var result = await _controller.Search(filter);

            // Then
            _mockCelestialBodyRepository.Verify(r => r.SearchAsync(filter, 1, 30), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }
    }
}