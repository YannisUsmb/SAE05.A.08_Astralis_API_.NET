using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Models.Repository.Specific;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class NotificationTypesControllerTestsMock : ReadableControllerMockTests<NotificationTypesController, NotificationType, NotificationTypeDto, NotificationTypeDto, int>
    {
        private Mock<INotificationTypeRepository> _mockNotificationTypeRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
        }

        protected override NotificationTypesController CreateController(Mock<IReadableRepository<NotificationType, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockNotificationTypeRepository = new Mock<INotificationTypeRepository>();

            _mockRepository = _mockNotificationTypeRepository.As<IReadableRepository<NotificationType, int>>();

            return new NotificationTypesController(_mockNotificationTypeRepository.Object, mapper);
        }


        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 99;

        protected override NotificationType GetSampleEntity() => new NotificationType
        {
            Id = 1,
            Label = "Information",
            Description = "Notifications informatives générales"
        };

        protected override List<NotificationType> GetSampleEntities() => new List<NotificationType>
        {
            new NotificationType { Id = 1, Label = "Information", Description = "Infos diverses" },
            new NotificationType { Id = 2, Label = "Alerte", Description = "Alertes de sécurité" },
            new NotificationType { Id = 3, Label = "Promotion", Description = "Offres spéciales" }
        };
    }
}