namespace Astralis_API.Models.Repository
{
    public interface ISearchableRepository<TEntity, in TKey>
    {
        Task<IEnumerable<TEntity?>> GetByKeyAsync(TKey key);
    }
}
