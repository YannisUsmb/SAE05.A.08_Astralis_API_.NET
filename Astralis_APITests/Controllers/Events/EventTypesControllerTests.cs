using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class EventTypesControllerTests : ReadableControllerTests<EventTypesController, EventType, EventTypeDto, EventTypeDto, int>
    {
        protected override EventTypesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new EventTypesController(new EventTypeManager(context), mapper);
        }

        protected override List<EventType> GetSampleEntities()
        {
            return new List<EventType>
            {
                new EventType {Id=902101, Label = "EventType 1", Description = "EventType description 1"},
                new EventType {Id=902102, Label = "EventType 2", Description = "EventType description 2"}
            };
        }

        protected override int GetIdFromEntity(EventType entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}