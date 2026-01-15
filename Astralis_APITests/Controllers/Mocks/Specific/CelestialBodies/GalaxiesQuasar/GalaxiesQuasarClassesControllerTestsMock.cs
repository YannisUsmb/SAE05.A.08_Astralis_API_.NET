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
    public class GalaxyQuasarClassesControllerTestsMock : ReadableControllerMockTests<GalaxyQuasarClassesController, GalaxyQuasarClass, GalaxyQuasarClassDto, GalaxyQuasarClassDto, int>
    {
        private Mock<IGalaxyQuasarClassRepository> _mockGalaxyQuasarClassRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
                cfg.CreateMap<GalaxyQuasarClass, GalaxyQuasarClassDto>();
            });
            _mapper = config.CreateMapper();

            _controller = CreateController(null, _mapper);
            SetupHttpContext(1, "User");
        }

        protected override GalaxyQuasarClassesController CreateController(Mock<IReadableRepository<GalaxyQuasarClass, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockGalaxyQuasarClassRepository = new Mock<IGalaxyQuasarClassRepository>();

            _mockRepository = _mockGalaxyQuasarClassRepository.As<IReadableRepository<GalaxyQuasarClass, int>>();

            return new GalaxyQuasarClassesController(_mockGalaxyQuasarClassRepository.Object, mapper);
        }


        protected override List<GalaxyQuasarClass> GetSampleEntities() => new List<GalaxyQuasarClass>
        {
            new GalaxyQuasarClass
            {
                Id = 1,
                Label = "Spiral",
                Description = "A galaxy with a rotating disc of stars and a central bulge."
            },
            new GalaxyQuasarClass
            {
                Id = 2,
                Label = "Quasar",
                Description = "An active galactic nucleus of very high luminosity."
            }
        };

        protected override GalaxyQuasarClass GetSampleEntity() => new GalaxyQuasarClass
        {
            Id = 1,
            Label = "Spiral",
            Description = "A galaxy with a rotating disc of stars and a central bulge."
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