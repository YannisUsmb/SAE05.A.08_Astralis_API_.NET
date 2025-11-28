using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class CreditCardManager : CrudManager<CreditCard, int>, ICreditCardRepository
    {
        public CreditCardManager(AstralisDbContext context) : base(context)
        {
        }
    }
}