using Microsoft.EntityFrameworkCore;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public abstract class DataManager<TEntity, TIdentifier, TKey> : CrudManager<TEntity, TIdentifier>, IDataRepository<TEntity, TIdentifier, TKey> where TEntity : class
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<TEntity> _entities;

        public DataManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        public abstract Task<IEnumerable<TEntity>> GetByKeyAsync(TKey key);
    }
}