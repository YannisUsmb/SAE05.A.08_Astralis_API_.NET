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

        protected override IQueryable<CreditCard> WithIncludes(IQueryable<CreditCard> query)
        {
            return query.Include(cc => cc.UserNavigation);
        }
    }
}