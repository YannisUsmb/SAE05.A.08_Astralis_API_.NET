using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IUserRepository : ICrudRepository<User, int>
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByPhoneAsync(string phone, int? prefixId);
        Task<User?> GetByEmailOrUsernameAsync(string identifier);
        Task<User?> GetByPhoneAndPrefixAsync(string phone, int? phonePrefixId);
        Task UpdateStripeIdAsync(int userId, string stripeCustomerId);
        Task SetPremiumStatusAsync(int userId, bool isPremium);
        Task AnonymizeUserAsync(int userId);
    }
}