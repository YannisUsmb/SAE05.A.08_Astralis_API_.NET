using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CreditCardManager : CrudManager<CreditCard, int>, ICreditCardRepository
    {
        public CreditCardManager(AstralisDbContext context) : base(context)
        {
        }

        public new async Task<CreditCard?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(cc => cc.Id == id);
        }

        protected override IQueryable<CreditCard> WithIncludes(IQueryable<CreditCard> query)
        {
            return query.Include(cc => cc.UserNavigation);
        }
    }
}