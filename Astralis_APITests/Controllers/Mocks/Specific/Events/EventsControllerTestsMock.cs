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
    public class EventsControllerTestsMock : CrudControllerMockTests<EventsController, Event, EventDto, EventDto, EventCreateDto, EventUpdateDto, int>
    {
        private Mock<IEventRepository> _mockEventRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override EventsController CreateController(Mock<IReadableRepository<Event, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockEventRepository = new Mock<IEventRepository>();

            _mockCrudRepository = _mockEventRepository.As<ICrudRepository<Event, int>>();
            _mockRepository = _mockEventRepository.As<IReadableRepository<Event, int>>();

            return new EventsController(_mockEventRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(EventUpdateDto dto, int id) { }


        protected override List<Event> GetSampleEntities() => new List<Event>
        {
            new Event
            {
                Id = 1,
                Title = "Concert Rock",
                UserId = 1,
                EventTypeId = 1,
                StartDate = DateTime.UtcNow.AddDays(10),
                EventTypeNavigation = new EventType { Label = "Concert" },
                EventInterests = new List<EventInterest>()
            },
            new Event
            {
                Id = 2,
                Title = "Atelier Cuisine",
                UserId = 2,
                EventTypeId = 2,
                StartDate = DateTime.UtcNow.AddDays(5),
                EventTypeNavigation = new EventType { Label = "Atelier" },
                EventInterests = new List<EventInterest>()
            }
        };

        protected override Event GetSampleEntity() => new Event
        {
            Id = 1,
            Title = "Concert Rock",
            UserId = 1,
            EventTypeId = 1,
            StartDate = DateTime.UtcNow.AddDays(10),
            EventTypeNavigation = new EventType { Label = "Concert" },
            EventInterests = new List<EventInterest>()
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override EventCreateDto GetValidCreateDto() => new EventCreateDto
        {
            Title = "Nouvel Event",
            Description = "Description",
            EventTypeId = 1,
            StartDate = DateTime.Now.AddDays(20)
        };

        protected override EventUpdateDto GetValidUpdateDto() => new EventUpdateDto
        {
            Title = "Event Modifié",
            Description = "Nouvelle Description",
            EventTypeId = 1,
            StartDate = DateTime.Now.AddDays(25)
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
        public async Task Search_ShouldReturnPagedResult()
        {
            // Given
            var filter = new EventFilterDto { SearchText = "Rock", PageNumber = 1, PageSize = 10 };
            var entities = GetSampleEntities().Where(e => e.Title.Contains("Rock")).ToList();

            _mockEventRepository.Setup(r => r.SearchAsync(
                filter.SearchText,
                filter.EventTypeIds,
                filter.MinStartDate,
                filter.MaxStartDate,
                filter.MinEndDate,
                filter.MaxEndDate,
                filter.PageNumber,
                filter.PageSize,
                filter.SortBy
            )).ReturnsAsync((entities, 1));

            // When
            var result = await _controller.Search(filter);

            // Then
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var pagedResult = okResult.Value as PagedResultDto<EventDto>;
            Assert.IsNotNull(pagedResult);
            Assert.AreEqual(1, pagedResult.TotalCount);
            Assert.AreEqual("Concert Rock", pagedResult.Items.First().Title);
        }

        [TestMethod]
        public async Task GetById_WithUserInterest_ShouldSetIsInterestedTrue()
        {
            // Given
            int userId = 1;
            int eventId = 1;
            SetupHttpContext(userId);

            var entity = GetSampleEntity();
            entity.EventInterests.Add(new EventInterest { UserId = userId, EventId = eventId });

            _mockEventRepository.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(entity);

            // When
            var result = await _controller.GetById(eventId);

            // Then
            var okResult = result.Result as OkObjectResult;
            var dto = okResult.Value as EventDto;
            Assert.IsNotNull(dto);
            Assert.IsTrue(dto.IsInterested, "IsInterested doit être vrai si l'utilisateur est dans la liste des intérêts.");
        }

        [TestMethod]
        public async Task Put_NotOwnerAndNotAdmin_ShouldReturnForbid()
        {
            // Given
            int eventId = 1;
            int ownerId = 1;
            int attackerId = 2;

            SetupHttpContext(attackerId, "Rédacteur Commercial");

            var entity = GetSampleEntity();
            entity.UserId = ownerId;

            _mockEventRepository.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(entity);

            // When
            var result = await _controller.Put(eventId, GetValidUpdateDto());

            // Then
            _mockEventRepository.Verify(r => r.UpdateAsync(It.IsAny<Event>(), It.IsAny<Event>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_AsAdmin_ShouldAllowUpdateEvenIfNotOwner()
        {
            // Given
            int eventId = 1;
            int ownerId = 1;
            int adminId = 99;

            SetupHttpContext(adminId, "Admin");

            var entity = GetSampleEntity();
            entity.UserId = ownerId;

            _mockEventRepository.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(entity);
            _mockEventRepository.Setup(r => r.UpdateAsync(entity, It.IsAny<Event>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(eventId, GetValidUpdateDto());

            // Then
            _mockEventRepository.Verify(r => r.UpdateAsync(entity, It.IsAny<Event>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_NotOwnerAndNotAdmin_ShouldReturnForbid()
        {
            // Given
            int eventId = 1;
            int ownerId = 1;
            int attackerId = 2;

            SetupHttpContext(attackerId, "Rédacteur Commercial");

            var entity = GetSampleEntity();
            entity.UserId = ownerId;

            _mockEventRepository.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(entity);

            // When
            var result = await _controller.Delete(eventId);

            // Then
            _mockEventRepository.Verify(r => r.DeleteAsync(It.IsAny<Event>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
        [TestMethod]
        public async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            int userId = 1;
            SetupHttpContext(userId, "Rédacteur Commercial");

            var entity = GetSampleEntity();
            entity.UserId = userId;

            _mockEventRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockEventRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockEventRepository.Verify(r => r.GetByIdAsync(id), Times.Exactly(2));
            _mockEventRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}