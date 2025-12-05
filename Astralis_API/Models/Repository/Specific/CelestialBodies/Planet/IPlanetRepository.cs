using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IPlanetRepository : IDataRepository<Planet, int, string>
    {
        Task<IEnumerable<Planet>> SearchAsync(
            string? name = null,
            IEnumerable<int>? planetTypeIds = null,
            IEnumerable<int>? detectionMethodIds = null,
            decimal? minDistance = null,
            decimal? maxDistance = null,
            decimal? minMass = null,
            decimal? maxMass = null,
            decimal? minRadius = null,
            decimal? maxRadius = null,
            int? minDiscoveryYear = null,
            int? maxDiscoveryYear = null,
            decimal? minEccentricity = null,
            decimal? maxEccentricity = null,
            decimal? minStellarMagnitude = null,
            decimal? maxStellarMagnitude = null
        );
    }
}