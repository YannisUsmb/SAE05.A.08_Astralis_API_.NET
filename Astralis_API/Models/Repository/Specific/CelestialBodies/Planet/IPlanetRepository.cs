using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IPlanetRepository : IDataRepository<Planet, int, string>
    {
        Task<IEnumerable<Planet>> SearchAsync(
            string? name = null,
            int? planetTypeId = null,
            int? detectionMethodId = null,
            int? minDiscoveryYear = null,
            int? maxDiscoveryYear = null,
            decimal? minOrbitalPeriod = null,
            decimal? maxOrbitalPeriod = null,
            decimal? minEccentricity = null,
            decimal? maxEccentricity = null,
            decimal? minStellarMagnitude = null,
            decimal? maxStellarMagnitude = null,
            string? distance = null,
            string? radius = null,
            string? temperature = null,
            string? hostStarTemperature = null,
            string? hostStarMass = null
        );
    }
}