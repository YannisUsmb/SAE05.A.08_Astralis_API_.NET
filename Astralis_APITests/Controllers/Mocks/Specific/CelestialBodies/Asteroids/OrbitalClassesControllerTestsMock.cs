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
    public class OrbitalClassesControllerTestsMock : ReadableControllerMockTests<OrbitalClassesController, OrbitalClass, OrbitalClassDto, OrbitalClassDto, int>
    {
        private Mock<IOrbitalClassRepository> _mockOrbitalClassRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
                cfg.CreateMap<OrbitalClass, OrbitalClassDto>();
            });
            _mapper = config.CreateMapper();

            _controller = CreateController(null, _mapper);

            SetupHttpContext(1, "User");
        }

        protected override OrbitalClassesController CreateController(Mock<IReadableRepository<OrbitalClass, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockOrbitalClassRepository = new Mock<IOrbitalClassRepository>();

            _mockRepository = _mockOrbitalClassRepository.As<IReadableRepository<OrbitalClass, int>>();

            return new OrbitalClassesController(_mockOrbitalClassRepository.Object, mapper);
        }

        protected override List<OrbitalClass> GetSampleEntities() => new List<OrbitalClass>
        {
            new OrbitalClass
            {
                Id = 1,
                Label = "MBA",
                Description = "Main Belt Asteroid"
            },
            new OrbitalClass
            {
                Id = 2,
                Label = "NEA",
                Description = "Near Earth Asteroid"
            }
        };

        protected override OrbitalClass GetSampleEntity() => new OrbitalClass
        {
            Id = 1,
            Label = "MBA",
            Description = "Main Belt Asteroid"
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