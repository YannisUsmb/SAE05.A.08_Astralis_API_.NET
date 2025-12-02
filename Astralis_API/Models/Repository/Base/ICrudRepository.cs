namespace Astralis_API.Models.Repository
{
    public interface ICrudRepository<TEntity, in TIdentifier> : 
        IReadableRepository<TEntity, TIdentifier>, 
        IWriteableRepository<TEntity>
    {
    }
}
