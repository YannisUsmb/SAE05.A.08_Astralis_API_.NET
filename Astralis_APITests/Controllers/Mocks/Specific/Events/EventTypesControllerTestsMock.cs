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
    public class EventTypesControllerTestsMock : CrudControllerMockTests<EventTypesController, EventType, EventTypeDto, EventTypeDto, EventTypeCreateDto, EventTypeUpdateDto, int>
    {
        private Mock<IEventTypeRepository> _mockEventTypeRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
                cfg.CreateMap<EventTypeUpdateDto, EventType>();
            });
            _mapper = config.CreateMapper();

            _controller = CreateController(null, _mapper);

            SetupHttpContext(1, "Admin");
        }

        protected override EventTypesController CreateController(Mock<IReadableRepository<EventType, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockEventTypeRepository = new Mock<IEventTypeRepository>();

            _mockCrudRepository = _mockEventTypeRepository.As<ICrudRepository<EventType, int>>();
            _mockRepository = _mockEventTypeRepository.As<IReadableRepository<EventType, int>>();

            return new EventTypesController(_mockEventTypeRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(EventTypeUpdateDto dto, int id)
        {
        }

        protected override List<EventType> GetSampleEntities() => new List<EventType>
        {
            new EventType { Id = 1, Label = "Concert", Description = "Musique live" },
            new EventType { Id = 2, Label = "Théâtre", Description = "Art dramatique" }
        };

        protected override EventType GetSampleEntity() => new EventType
        {
            Id = 1,
            Label = "Concert",
            Description = "Musique live"
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override EventTypeCreateDto GetValidCreateDto() => new EventTypeCreateDto
        {
            Label = "Festival",
            Description = "Evénement plein air"
        };

        protected override EventTypeUpdateDto GetValidUpdateDto() => new EventTypeUpdateDto
        {
            Label = "Festival Update",
            Description = "Description mise à jour"
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
    }
}