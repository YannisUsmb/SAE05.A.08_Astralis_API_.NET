using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class UserManager : CrudManager<User, int>, IUserRepository
    {
        public UserManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(e => e.Id == id);
        }

        protected override IQueryable<User> WithIncludes(IQueryable<User> query)
        {
            return query.Include(u => u.DeliveryAddressNavigation)
                            .Include(u => u.InvoicingAddressNavigation)
                            .Include(u => u.PhonePrefixNavigation)
                            .Include(u => u.UserRoleNavigation)
                            .Include(u => u.CreditCards)
                            .Include(u => u.Events)
                            .Include(u => u.Commands)
                            .Include(u => u.CartItems)
                                .ThenInclude(ci => ci.ProductNavigation)
                            .Include(u => u.Products)
                            .Include(u => u.EventInterests)
                                .ThenInclude(ei => ei.EventNavigation)
                            .Include(u => u.Articles)
                            .Include(u => u.ArticleInterests)
                                .ThenInclude(ai => ai.ArticleNavigation)
                            .Include(u => u.Comments)
                            .Include(u => u.Reports)
                            .Include(u => u.TreatedReports)
                            .Include(u => u.Discoveries)
                            .Include(u => u.ApprovedDiscoveries)
                            .Include(u => u.ApprovedAliasDiscoveries)
                            .Include(u => u.UserNotifications)
                                .ThenInclude(un=> un.NotificationNavigation)
                            .Include(u => u.UserNotificationTypes)
                                .ThenInclude(unt => unt.NotificationTypeNavigation);
        }
    }
}
