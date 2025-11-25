using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AliasStatusManager : CrudManager<AliasStatus, int, string>
    {
        public AliasStatusManager(AstralisDbContext context) : base(context)
        {
        }
    }
}