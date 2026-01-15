using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Mapper;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class CelestialBodyTypesControllerTestsMock : ReadableControllerMockTests<CelestialBodyTypesController, CelestialBodyType, CelestialBodyTypeDto, CelestialBodyTypeDto, int>
    {
        private Mock<ICelestialBodyTypeRepository> _mockCelestialBodyTypeRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
                cfg.CreateMap<CelestialBodyType, CelestialBodyTypeDto>();
            });
            _mapper = config.CreateMapper();

            _controller = CreateController(null, _mapper);

            SetupHttpContext(1, "User");
        }

        protected override CelestialBodyTypesController CreateController(Mock<IReadableRepository<CelestialBodyType, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockCelestialBodyTypeRepository = new Mock<ICelestialBodyTypeRepository>();

            _mockRepository = _mockCelestialBodyTypeRepository.As<IReadableRepository<CelestialBodyType, int>>();

            return new CelestialBodyTypesController(_mockCelestialBodyTypeRepository.Object, mapper);
        }

        protected override List<CelestialBodyType> GetSampleEntities() => new List<CelestialBodyType>
        {
            new CelestialBodyType
            {
                Id = 1,
                Label = "Star",
                Description = "A luminous spheroid of plasma held together by its own gravity."
            },
            new CelestialBodyType
            {
                Id = 2,
                Label = "Planet",
                Description = "A celestial body orbiting a star or stellar remnant."
            }
        };

        protected override CelestialBodyType GetSampleEntity() => new CelestialBodyType
        {
            Id = 1,
            Label = "Star",
            Description = "A luminous spheroid of plasma held together by its own gravity."
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;
        private void SetupHttpContext(int userId, string role = "User")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            if (_controller != null)
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
                };
        }
    }
}