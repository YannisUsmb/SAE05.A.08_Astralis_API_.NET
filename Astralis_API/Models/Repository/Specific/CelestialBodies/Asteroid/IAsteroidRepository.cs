using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IAsteroidRepository : IDataRepository<Asteroid, int, string>
    {
        Task<IEnumerable<Asteroid>> GetByOrbitalIClassIdAsync(int id);
        Task<IEnumerable<Asteroid>> SearchAsync(
            string? reference = null,
            int? orbitalClassId = null,
            bool? isPotentiallyHazardous = null,
            int? orbitId = null,
            decimal? absoluteMagnitude = null,
            decimal? minDiameter = null,
            decimal? MaxDiameter = null,
            decimal? minInclination = null,
            decimal? MaxInclination = null,
            decimal? semiMajorAxis = null,
            DateTime? firstObservationDate = null,
            DateTime? lastObservationDate = null
        );
    }
}