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
    public class AddressesControllerTestsMock : CrudControllerMockTests<AddressesController, Address, AddressDto, AddressDto, AddressCreateDto, AddressUpdateDto, int>
    {
        private Mock<IAddressRepository> _mockAddressRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override AddressesController CreateController(Mock<IReadableRepository<Address, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockAddressRepository = new Mock<IAddressRepository>();

            _mockCrudRepository = _mockAddressRepository.As<ICrudRepository<Address, int>>();
            _mockRepository = _mockAddressRepository.As<IReadableRepository<Address, int>>();

            return new AddressesController(_mockAddressRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(AddressUpdateDto dto, int id) { }

        protected override List<Address> GetSampleEntities() => new List<Address>
        {
            new Address { Id = 1, StreetNumber = "10", StreetAddress = "Rue de la Paix", CityId = 1 },
            new Address { Id = 2, StreetNumber = "20", StreetAddress = "Avenue Foch", CityId = 2 }
        };

        protected override Address GetSampleEntity() => new Address
        {
            Id = 1,
            StreetNumber = "10",
            StreetAddress = "Rue de la Paix",
            CityId = 1
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override AddressCreateDto GetValidCreateDto() => new AddressCreateDto
        {
            StreetNumber = "5",
            StreetAddress = "New Way",
            CityId = 3
        };

        protected override AddressUpdateDto GetValidUpdateDto() => new AddressUpdateDto
        {
            StreetNumber = "55",
            StreetAddress = "Updated Way",
            CityId = 3
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
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            var createDto = GetValidCreateDto();

            _mockAddressRepository.Setup(r => r.AddAsync(It.IsAny<Address>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockAddressRepository.Verify(r => r.AddAsync(It.IsAny<Address>()), Times.Once);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }
    }
}