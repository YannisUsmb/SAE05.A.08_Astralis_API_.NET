using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository.Specific;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class NotificationTypeManager : ReadableManager<NotificationType, int>, INotificationTypeRepository
    {
        public NotificationTypeManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<NotificationType?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(nt => nt.Id == id);
        }

        protected override IQueryable<NotificationType> WithIncludes(IQueryable<NotificationType> query)
        {
            return query.Include(nt => nt.Notifications)
                        .Include(nt => nt.UserNotificationTypes);
        }
    }
}