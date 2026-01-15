using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class NotificationsControllerTestsMock : CrudControllerMockTests<NotificationsController, Notification, NotificationDto, NotificationDto, NotificationCreateDto, NotificationUpdateDto, int>
    {
        private Mock<INotificationRepository> _mockNotificationRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
        }

        protected override NotificationsController CreateController(Mock<IReadableRepository<Notification, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockNotificationRepository = new Mock<INotificationRepository>();

            _mockRepository = _mockNotificationRepository.As<IReadableRepository<Notification, int>>();
            _mockCrudRepository = _mockNotificationRepository.As<ICrudRepository<Notification, int>>();

            return new NotificationsController(_mockNotificationRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(NotificationUpdateDto dto, int id)
        {
            dto.Id = id;
        }

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override Notification GetSampleEntity() => new Notification
        {
            Id = 1,
            Label = "Maintenance Serveur",
            Description = "Une maintenance est prévue ce soir.",
            NotificationTypeId = 1
        };

        protected override List<Notification> GetSampleEntities() => new List<Notification>
        {
            new Notification { Id = 1, Label = "Maintenance", Description = "...", NotificationTypeId = 1 },
            new Notification { Id = 2, Label = "Nouvel Article", Description = "...", NotificationTypeId = 2 }
        };

        protected override NotificationCreateDto GetValidCreateDto() => new NotificationCreateDto
        {
            Label = "Promotion de Noël",
            Description = "-50% sur la boutique",
            NotificationTypeId = 3,
            Link = "/shop/promo"
        };

        protected override NotificationUpdateDto GetValidUpdateDto() => new NotificationUpdateDto
        {
            Id = 1,
            Label = "Maintenance Annulée",
            Description = "La maintenance est reportée.",
            NotificationTypeId = 1
        };
    }
}