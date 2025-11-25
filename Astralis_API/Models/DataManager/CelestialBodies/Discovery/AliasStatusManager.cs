using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class AliasStatusManager : CrudManager<AliasStatus, int>, IAliasStatusRepository
    {
        public AliasStatusManager(AstralisDbContext context) : base(context)
        {
        }
    }
}