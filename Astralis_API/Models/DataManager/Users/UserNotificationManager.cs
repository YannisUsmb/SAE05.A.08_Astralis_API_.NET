using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class UserNotificationManager : CrudManager<UserNotification, int>, IUserNotificationRepository
    {
        public UserNotificationManager(AstralisDbContext context) : base(context)
        {
        }
    }
}