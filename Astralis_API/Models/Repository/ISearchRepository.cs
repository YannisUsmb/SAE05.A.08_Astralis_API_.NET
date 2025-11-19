namespace Astralis_API.Models.Repository
{
    public interface ISearchRepository<TEntity, in TKey>
    {
        Task<TEntity> GetByKeyAsync(TKey key);
    }
}
