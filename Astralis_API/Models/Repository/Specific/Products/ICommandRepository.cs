using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface ICommandRepository : IDataRepository<Command, int, int>
    {
        Task<IEnumerable<Command>> GetByCommandStatusIdAsync(int id);
    }
}