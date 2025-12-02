using Microsoft.EntityFrameworkCore;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public abstract class DataManager<TEntity, TIdentifier, TKey> : CrudManager<TEntity, TIdentifier>, IDataRepository<TEntity, TIdentifier, TKey> where TEntity : class
    {
        public DataManager(AstralisDbContext context) : base(context)
        {
        }

        public abstract Task<IEnumerable<TEntity>> GetByKeyAsync(TKey key);
    }
}