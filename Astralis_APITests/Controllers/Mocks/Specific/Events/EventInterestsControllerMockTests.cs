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
    public class EventInterestsControllerTestsMock : JoinControllerMockTests<EventInterestsController, EventInterest, EventInterestDto, EventInterestDto, int, int>
    {
        private Mock<IEventInterestRepository> _mockEventInterestRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override EventInterestsController CreateController(Mock<IJoinRepository<EventInterest, int, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockEventInterestRepository = new Mock<IEventInterestRepository>();

            _mockRepository = _mockEventInterestRepository.As<IJoinRepository<EventInterest, int, int>>();

            return new EventInterestsController(_mockEventInterestRepository.Object, mapper);
        }

        protected override List<EventInterest> GetSampleEntities() => new List<EventInterest>
        {
            new EventInterest { EventId = 1, UserId = 1 },
            new EventInterest { EventId = 2, UserId = 1 }
        };

        protected override EventInterest GetSampleEntity() => new EventInterest
        {
            EventId = 1,
            UserId = 1
        };

        protected override int GetExistingKey1() => 1;
        protected override int GetExistingKey2() => 1;

        protected override int GetNonExistingKey1() => 999;
        protected override int GetNonExistingKey2() => 999;

        protected override EventInterestDto GetValidCreateDto() => new EventInterestDto
        {
            EventId = 3,
            UserId = 1
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
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
                };
        }
    }
}