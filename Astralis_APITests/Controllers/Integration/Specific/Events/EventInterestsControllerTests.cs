using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class EventInterestsControllerTests
        : JoinControllerTests<EventInterestsController, EventInterest, EventInterestDto, EventInterestDto, int, int>
    {
        private int _eventId1;
        private int _eventId2;
        private int _userId1;
        private int _userId2;

        protected override EventInterestsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new EventInterestsController(new EventInterestManager(context), mapper);
        }

        protected override List<EventInterest> GetSampleEntities()
        {
            UserRole role = new UserRole { Label = "UserTest" };
            _context.UserRoles.Add(role);

            EventType type = new EventType { Label = "Meeting" };
            _context.EventTypes.Add(type);
            _context.SaveChanges();

            User user = new User
            {
                Username = "User1",
                Email = "u1@test.com",
                Password = "Pwd",
                FirstName = "F",
                LastName = "L",
                UserRoleId = role.Id
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            Event evt1 = new Event
            {
                Title = "Event1",
                Description = "Desc",
                StartDate = DateTime.UtcNow,
                EventTypeId = type.Id,
                UserId = user.Id
            };
            Event evt2 = new Event
            {
                Title = "Event2",
                Description = "Desc",
                StartDate = DateTime.UtcNow,
                EventTypeId = type.Id,
                UserId = user.Id
            };
            _context.Events.AddRange(evt1, evt2);
            _context.SaveChanges();

            _eventId1 = evt1.Id;
            _eventId2 = evt2.Id;
            _userId1 = user.Id;

            User user2 = new User
            {
                Username = "User2",
                Email = "u2@test.com",
                Password = "Pwd",
                FirstName = "F",
                LastName = "L",
                UserRoleId = role.Id
            };
            _context.Users.Add(user2);
            _context.SaveChanges();
            _userId2 = user2.Id;

            return new List<EventInterest>
            {
                new EventInterest { EventId = _eventId1, UserId = _userId1 },
            };
        }

        protected override int GetKey1(EventInterest entity)
        {
            return entity.EventId;
        }

        protected override int GetKey2(EventInterest entity)
        {
            return entity.UserId;
        }

        protected override int GetNonExistingKey1()
        {
            return 99999;
        }

        protected override int GetNonExistingKey2()
        {
            return 99999;
        }

        protected override EventInterestDto GetValidCreateDto()
        {
            return new EventInterestDto
            {
                EventId = _eventId2,
                UserId = _userId2
            };
        }

        protected override (int, int) GetKeysFromCreateDto(EventInterestDto dto)
        {
            return (dto.EventId, dto.UserId);
        }
    }
}