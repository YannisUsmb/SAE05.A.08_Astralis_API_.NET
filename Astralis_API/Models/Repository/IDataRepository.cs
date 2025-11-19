namespace Astralis_API.Models.Repository
{
    public interface IDataRepository<TEntity, TIdentifier, TKey> : IReadableRepository<TEntity, TIdentifier>, IWriteableRepository<TEntity>, ISearchRepository<TEntity, TKey>
    {
    }
}
