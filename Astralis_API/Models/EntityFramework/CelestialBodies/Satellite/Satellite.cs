using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_satellite_sat")]
    public class Satellite
    {
        [Key]
        [Column("sat_id")]
        public int Id { get; set; }

        [Column("ceb_id")]
        [Required(ErrorMessage = "The celestial body ID is required.")]
        public int CelestialBodyId { get; set; }

        [Column("pla_id")]
        [Required(ErrorMessage = "The planet ID is required.")]
        public int PlanetId { get; set; }

        [Column("sat_gravity", TypeName = "NUMERIC(10,5)")]
        public decimal? Gravity { get; set; }

        [Column("sat_radius", TypeName = "NUMERIC(7,2)")]
        public decimal? Radius { get; set; }

        [Column("sat_density", TypeName = "NUMERIC(6,5)")]
        public decimal? Density { get; set; }

        [ForeignKey(nameof(CelestialBodyId))]
        [InverseProperty(nameof(CelestialBody.SatelliteNavigation))]
        public virtual CelestialBody CelestialBodyNavigation { get; set; } = null!;

        [ForeignKey(nameof(PlanetId))]
        [InverseProperty(nameof(Planet.Satellites))]
        public virtual Planet PlanetNavigation { get; set; } = null!;
    }
}