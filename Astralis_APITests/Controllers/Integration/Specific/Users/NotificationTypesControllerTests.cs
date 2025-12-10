using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class NotificationTypesControllerTests : ReadableControllerTests<NotificationTypesController, NotificationType, NotificationTypeDto, NotificationTypeDto, int>
    {
        protected override NotificationTypesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new NotificationTypesController(new NotificationTypeManager(context), mapper);
        }

        protected override List<NotificationType> GetSampleEntities()
        {
            return new List<NotificationType>
            {
                new NotificationType {Id=902101, Label = "NotificationType 1", Description = "NotificationType description 1"},
                new NotificationType {Id=902102, Label = "NotificationType 2", Description = "NotificationType description 2"}
            };
        }

        protected override int GetIdFromEntity(NotificationType entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}