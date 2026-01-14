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

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _entities.AnyAsync(e => e.Email == email);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _entities.AnyAsync(e => e.Username == username);
        }

        public async Task<bool> ExistsByPhoneAsync(string phone, int? prefixId)
        {
            return await _entities.AnyAsync(e => e.Phone == phone && e.PhonePrefixId == prefixId);
        }

        public async Task<User?> GetByEmailOrUsernameAsync(string identifier)
        {
            return await WithIncludes(_entities)
                .FirstOrDefaultAsync(u => u.Email == identifier || u.Username == identifier);
        }

        public async Task<User?> GetByPhoneAndPrefixAsync(string phone, int? phonePrefixId)
        {
            return await WithIncludes(_entities)
                .FirstOrDefaultAsync(u => u.Phone == phone && u.PhonePrefixId == phonePrefixId);
        }

        public async Task UpdateStripeIdAsync(int userId, string stripeCustomerId)
        {
            var user = await _entities.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.StripeCustomerId = stripeCustomerId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetPremiumStatusAsync(int userId, bool isPremium)
        {
            var user = await _entities.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.IsPremium = isPremium;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AnonymizeUserAsync(int userId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null) return;

            if (user.CreditCards != null && user.CreditCards.Any())
            {
                _context.Set<CreditCard>().RemoveRange(user.CreditCards);
            }

            if (user.CartItems != null && user.CartItems.Any())
            {
                _context.Set<CartItem>().RemoveRange(user.CartItems);
            }

            user.DeliveryId = null;
            user.InvoicingId = null;
            user.DeliveryAddressNavigation = null;
            user.InvoicingAddressNavigation = null;

            string uniqueId = Guid.NewGuid().ToString().Substring(0, 8);

            user.FirstName = "Utilisateur";
            user.LastName = "Supprimé";
            user.Username = $"deleted_user_{uniqueId}";
            user.Email = $"deleted_{uniqueId}@astralis.local";
            user.Phone = null;
            user.AvatarUrl = null;

            user.Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString());

            user.IsPremium = false;
            user.StripeCustomerId = null;
            user.MultiFactorAuthentification = false;

            user.IsEmailVerified = false;
            user.VerificationToken = null;

            await _context.SaveChangesAsync();
        }
        protected override IQueryable<User> WithIncludes(IQueryable<User> query)
        {
            return query.Include(u => u.DeliveryAddressNavigation)
                            .Include(u => u.InvoicingAddressNavigation)
                            .Include(u => u.PhonePrefixNavigation)
                                .ThenInclude(p => p.Countries)
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
