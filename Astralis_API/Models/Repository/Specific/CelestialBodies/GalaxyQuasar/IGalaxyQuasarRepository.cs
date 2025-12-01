using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IGalaxyQuasarRepository : IDataRepository<GalaxyQuasar, int, string>
    {
        Task<IEnumerable<GalaxyQuasar>> SearchAsync(
            string? reference = null,
            IEnumerable<int>? galaxyQuasarClassIds = null,
            decimal? minRightAscension = null,
            decimal? maxRightAscension = null,
            decimal? minDeclination = null,
            decimal? maxDeclination = null,
            decimal? minRedshift = null,
            decimal? maxRedshift = null,
            decimal? minRMagnitude = null,
            decimal? maxRMagnitude = null,
            int? minMjdObs = null,
            int? maxMjdObs = null
        );
    }
}