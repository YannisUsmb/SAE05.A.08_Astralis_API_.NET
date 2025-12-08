using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IAudioRepository : IReadableRepository<Audio, int>, ISearchableRepository<Audio, string>
    {
        Task<IEnumerable<Audio?>> SearchAsync(
                    string? searchTerm = null,
                    IEnumerable<int>? celestialBodyTypeIds = null
            );
    }
}
