namespace Astralis_API.Models.Repository
{
    public interface ISearchRepository<TEntity, in TKey>
    {
        Task<IEnumerable<TEntity?>> GetByKeyAsync(TKey key);
    }
}
