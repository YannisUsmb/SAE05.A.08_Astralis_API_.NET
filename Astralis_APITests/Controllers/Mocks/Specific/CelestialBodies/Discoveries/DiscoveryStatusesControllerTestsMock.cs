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
    public class DiscoveryStatusesControllerTestsMock : ReadableControllerMockTests<DiscoveryStatusesController, DiscoveryStatus, DiscoveryStatusDto, DiscoveryStatusDto, int>
    {
        private Mock<IDiscoveryStatusRepository> _mockDiscoveryStatusRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
                cfg.CreateMap<DiscoveryStatus, DiscoveryStatusDto>();
            });
            _mapper = config.CreateMapper();

            _controller = CreateController(null, _mapper);

            SetupHttpContext(1, "User");
        }

        protected override DiscoveryStatusesController CreateController(Mock<IReadableRepository<DiscoveryStatus, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockDiscoveryStatusRepository = new Mock<IDiscoveryStatusRepository>();

            _mockRepository = _mockDiscoveryStatusRepository.As<IReadableRepository<DiscoveryStatus, int>>();

            return new DiscoveryStatusesController(_mockDiscoveryStatusRepository.Object, mapper);
        }


        protected override List<DiscoveryStatus> GetSampleEntities() => new List<DiscoveryStatus>
        {
            new DiscoveryStatus { Id = 1, Label = "Draft" },
            new DiscoveryStatus { Id = 2, Label = "Validated" },
            new DiscoveryStatus { Id = 3, Label = "Refused" }
        };

        protected override DiscoveryStatus GetSampleEntity() => new DiscoveryStatus
        {
            Id = 1,
            Label = "Draft"
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