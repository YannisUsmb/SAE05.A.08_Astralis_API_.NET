using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class CitiesControllerTestsMock : CrudControllerMockTests<CitiesController, City, CityDto, CityDto, CityCreateDto, CityCreateDto, int>
    {
        private Mock<ICityRepository> _mockCityRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "Admin");
        }

        protected override CitiesController CreateController(Mock<IReadableRepository<City, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockCityRepository = new Mock<ICityRepository>();

            _mockCrudRepository = _mockCityRepository.As<ICrudRepository<City, int>>();
            _mockRepository = _mockCityRepository.As<IReadableRepository<City, int>>();

            return new CitiesController(_mockCityRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(CityCreateDto dto, int id) { }

        protected override List<City> GetSampleEntities() => new List<City>
        {
            new City { Id = 1, Name = "Paris", PostCode = "75000", CountryId = 1 },
            new City { Id = 2, Name = "Lyon", PostCode = "69000", CountryId = 1 }
        };

        protected override City GetSampleEntity() => new City
        {
            Id = 1,
            Name = "Paris",
            PostCode = "75000",
            CountryId = 1
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override CityCreateDto GetValidCreateDto() => new CityCreateDto
        {
            Name = "Marseille",
            PostCode = "13000",
            CountryId = 1
        };

        protected override CityCreateDto GetValidUpdateDto() => new CityCreateDto
        {
            Name = "Marseille Updated",
            PostCode = "13000",
            CountryId = 1
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

            _mockCityRepository.Setup(r => r.AddAsync(It.IsAny<City>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockCityRepository.Verify(r => r.AddAsync(It.IsAny<City>()), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Search_ValidTerm_ShouldReturnList()
        {
            // Given
            string term = "Par";
            var entities = GetSampleEntities();
            _mockCityRepository.Setup(r => r.SearchAsync(term)).ReturnsAsync(entities);

            // When
            var result = await _controller.Search(term);

            // Then
            _mockCityRepository.Verify(r => r.SearchAsync(term), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Search_ShortTerm_ShouldReturnEmptyList()
        {
            // Given
            string term = "P";

            // When
            var result = await _controller.Search(term);

            // Then
            _mockCityRepository.Verify(r => r.SearchAsync(It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            var list = okResult.Value as IEnumerable<CityDto>;
            Assert.IsNotNull(list);
            var count = 0;
            foreach (var item in list) count++;
            Assert.AreEqual(0, count);
        }
    }
}