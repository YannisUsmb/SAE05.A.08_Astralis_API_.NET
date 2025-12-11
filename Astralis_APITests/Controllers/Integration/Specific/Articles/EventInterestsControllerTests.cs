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
            var role = new UserRole { Label = "UserTest" };
            _context.UserRoles.Add(role);

            var type = new EventType { Label = "Meeting" };
            _context.EventTypes.Add(type);
            _context.SaveChanges();

            var user = new User
            {
                Username = "User1",
                Email = "u1@test.com",
                Password = "Pwd",
                FirstName = "F",
                LastName = "L",
                UserRoleId = role.Id
            };
            _context.Users.Add(user);

            var evt1 = new Event
            {
                Title = "Event1",
                Description = "Desc",
                StartDate = DateTime.Now,
                EventTypeId = type.Id,
                UserId = user.Id
            };
            var evt2 = new Event
            {
                Title = "Event2",
                Description = "Desc",
                StartDate = DateTime.Now,
                EventTypeId = type.Id,
                UserId = user.Id
            };
            _context.Events.AddRange(evt1, evt2);
            _context.SaveChanges();

            _eventId1 = evt1.Id;
            _eventId2 = evt2.Id;
            _userId1 = user.Id;

            var user2 = new User
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

        // --- Implémentation des Helpers ---

        protected override int GetKey1(EventInterest entity) => entity.EventId;
        protected override int GetKey2(EventInterest entity) => entity.UserId;

        protected override int GetNonExistingKey1() => 99999;
        protected override int GetNonExistingKey2() => 99999;

        protected override EventInterestDto GetValidCreateDto()
        {
            // On crée une jointure entre l'Event 2 et le User 2 (qui n'existe pas encore dans SampleEntities)
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