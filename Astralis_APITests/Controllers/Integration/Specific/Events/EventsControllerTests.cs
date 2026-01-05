using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class EventsControllerTests
        : CrudControllerTests<EventsController, Event, EventDto, EventDto, EventCreateDto, EventUpdateDto, int>
    {
        private const int USER_EDITOR_ID = 5001;
        private const int USER_NORMAL_ID = 5002;

        private const int EVENT_TYPE_ECLIPSE = 1;
        private const int EVENT_TYPE_ROCKET_LAUNCH = 2;

        private const int EVENT_ID_1 = 44001;
        private const int EVENT_ID_2 = 44002;

        private int _eventId1;
        private int _eventId2;

        protected override EventsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var eventManager = new EventManager(context);

            var controller = new EventsController(
                eventManager,
                mapper
            );

            SetupUserContext(controller, USER_EDITOR_ID, "Rédacteur commercial");
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, $"User_{userId}")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        protected override List<Event> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == 10))
                _context.UserRoles.Add(new UserRole { Id = 10, Label = "Rédacteur commercial" });

            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == 1))
                _context.UserRoles.Add(new UserRole { Id = 1, Label = "User" });

            CreateUserIfNotExist(USER_EDITOR_ID, 10);
            CreateUserIfNotExist(USER_NORMAL_ID, 1);

            if (!_context.EventTypes.AsNoTracking().Any(t => t.Id == EVENT_TYPE_ECLIPSE))
                _context.EventTypes.Add(new EventType { Id = EVENT_TYPE_ECLIPSE, Label = "Eclipse" });

            if (!_context.EventTypes.AsNoTracking().Any(t => t.Id == EVENT_TYPE_ROCKET_LAUNCH))
                _context.EventTypes.Add(new EventType { Id = EVENT_TYPE_ROCKET_LAUNCH, Label = "Rocket launch" });

            var e1 = CreateEventInMemory(
                EVENT_ID_1,
                "Solar eclipse",
                "Watching an eclipse",
                USER_EDITOR_ID,
                EVENT_TYPE_ECLIPSE
            );

            var e2 = CreateEventInMemory(
                EVENT_ID_2,
                "Rocket launch",
                "Rocket launch",
                USER_NORMAL_ID,
                EVENT_TYPE_ROCKET_LAUNCH
            );

            _eventId1 = EVENT_ID_1;
            _eventId2 = EVENT_ID_2;

            return new List<Event> { e1, e2 };
        }

        private Event CreateEventInMemory(int id, string title, string description, int userId, int typeId)
        {
            return new Event
            {
                Id = id,
                Title = title,
                Description = description,
                UserId = userId,
                EventTypeId = typeId,
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(11),
                Location = "Paris",
                Link = "http://test.com"
            };
        }

        private void CreateUserIfNotExist(int id, int roleId)
        {
            if (!_context.Users.AsNoTracking().Any(u => u.Id == id))
            {
                _context.Users.Add(new User
                {
                    Id = id,
                    UserRoleId = roleId,
                    Username = $"User{id}",
                    Email = $"user{id}@test.com",
                    FirstName = "Test",
                    LastName = "User",
                    Password = "Pwd"
                });
            }
        }

        protected override int GetIdFromEntity(Event entity) => entity.Id;
        protected override int GetIdFromDto(EventDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override EventCreateDto GetValidCreateDto()
        {
            return new EventCreateDto
            {
                Title = "New Event",
                Description = "Description",
                EventTypeId = EVENT_TYPE_ECLIPSE,
                StartDate = DateTime.UtcNow.AddDays(20),
                EndDate = DateTime.UtcNow.AddDays(21),
                Location = "Lyon",
                Link = "http://new.com"
            };
        }

        protected override EventUpdateDto GetValidUpdateDto(Event entityToUpdate)
        {
            return new EventUpdateDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                EventTypeId = EVENT_TYPE_ROCKET_LAUNCH,
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(31)
            };
        }

        protected override void SetIdInUpdateDto(EventUpdateDto dto, int id) { }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            SetupUserContext(_controller, USER_EDITOR_ID, "Rédacteur commercial");
            await base.Post_ValidObject_ShouldCreateAndReturn200();
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreate_ReturnsCreated()
        {
            await Post_ValidObject_ShouldCreateAndReturn200();
        }


        [TestMethod]
        public async Task Delete_CommercialEditor_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_EDITOR_ID, "Rédacteur commercial");
            var result = await _controller.Delete(_eventId1);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_NormalUser_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");

            var result = await _controller.Delete(_eventId1);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Search_ByTitle_ShouldReturnCorrectEvents()
        {
            var filter = new EventFilterDto
            {
                SearchText = "Rocket"
            };

            var result = await _controller.Search(filter);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<EventDto>;

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any(e => e.Id == _eventId2));
            Assert.IsFalse(list.Any(e => e.Id == _eventId1));
        }

        [TestMethod]
        public async Task Put_ShouldUpdateDates()
        {
            SetupUserContext(_controller, USER_EDITOR_ID, "Rédacteur commercial");
            var updateDto = GetValidUpdateDto(new Event());

            var result = await _controller.Put(_eventId1, updateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updated = await _context.Events.FindAsync(_eventId1);

            Assert.IsTrue(updated.StartDate > DateTime.UtcNow.AddDays(25));
        }

        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            var result = await _controller.GetById(_eventId1);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var dto = (result.Result as OkObjectResult).Value as EventDto;
            Assert.AreEqual(_eventId1, dto.Id);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<EventDto>;
            Assert.IsTrue(list.Any(e => e.Id == _eventId1));
        }
    }
}