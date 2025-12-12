using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class EventInterestsControllerMockTests
        : JoinControllerMockTests<EventInterestsController, EventInterest, EventInterestDto, EventInterestDto, int, int>
    {
        protected override EventInterestsController CreateController(Mock<IJoinRepository<EventInterest, int, int>> mockRepo, IMapper mapper)
        {
            var specificMock = mockRepo.As<IEventInterestRepository>();
            return new EventInterestsController(specificMock.Object, mapper);
        }

        protected override List<EventInterest> GetSampleEntities()
        {
            return new List<EventInterest>
            {
                new EventInterest { EventId = 1, UserId = 10 },
                new EventInterest { EventId = 2, UserId = 20 }
            };
        }

        protected override EventInterest GetSampleEntity()
        {
            return new EventInterest { EventId = 1, UserId = 10 };
        }

        protected override int GetExistingKey1()
        {
            return 1;
        }

        protected override int GetExistingKey2()
        {
            return 10;
        }

        protected override int GetNonExistingKey1()
        {
            return 999;
        }

        protected override int GetNonExistingKey2()
        {
            return 999;
        }

        protected override EventInterestDto GetValidCreateDto()
        {
            return new EventInterestDto { EventId = 3, UserId = 30 };
        }
    }
}