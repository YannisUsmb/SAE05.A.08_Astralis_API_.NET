using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_planet_pla")]
    public class Planet
    {
        [Key]
        [Column("pla_id")]
        public int Id { get; set; }

        [Column("ceb_id")]
        [Required(ErrorMessage = "The celestial body ID is required.")]
        public int CelestialBodyId { get; set; }

        [Column("plt_id")]
        [Required(ErrorMessage = "The planet type ID is required.")]
        public int PlanetTypeId { get; set; }

        [Column("dem_id")]
        [Required(ErrorMessage = "The detection method ID is required.")]
        public int DetectionMethodId { get; set; }

        [Column("pla_distance", TypeName = "NUMERIC(20,10)")]
        public decimal? Distance { get; set; }

        [Column("pla_discoveryyear")]
        public int? DiscoveryYear { get; set; }

        [Column("pla_mass", TypeName = "NUMERIC(20,10)")]
        public decimal? Mass { get; set; }

        [Column("pla_radius", TypeName = "NUMERIC(20,10)")]
        public decimal? Radius { get; set; }

        [Column("pla_temperature")]
        [StringLength(30, ErrorMessage = "The temperature cannot be longer than 30 characters.")]
        public string? Temperature { get; set; }

        [Column("pla_orbitalperiod")]
        [StringLength(40, ErrorMessage = "The orbital period cannot be longer than 40 characters.")]
        public string? OrbitalPeriod { get; set; }

        [Column("pla_eccentricity", TypeName = "NUMERIC(4,3)")]
        public decimal? Eccentricity { get; set; }

        [Column("pla_stellarmagnitude", TypeName = "NUMERIC(5,3)")]
        public decimal? StellarMagnitude { get; set; }

        [Column("pla_hoststartemperature")]
        [StringLength(30, ErrorMessage = "The host star temperature cannot be longer than 30 characters.")]
        public string? HostStarTemperature { get; set; }

        [Column("pla_hoststarmass")]
        [StringLength(30, ErrorMessage = "The host star mass cannot be longer than 30 characters.")]
        public string? HostStarMass { get; set; }

        [Column("pla_remark")]
        [StringLength(250, ErrorMessage = "The remark cannot be longer than 250 characters.")]
        public string? Remark { get; set; }

        [ForeignKey(nameof(CelestialBodyId))]
        [InverseProperty(nameof(CelestialBody.PlanetNavigation))]
        public virtual CelestialBody CelestialBodyNavigation { get; set; } = null!;

        [ForeignKey(nameof(PlanetTypeId))]
        [InverseProperty(nameof(PlanetType.Planets))]
        public virtual PlanetType PlanetTypeNavigation { get; set; } = null!;

        [ForeignKey(nameof(DetectionMethodId))]
        [InverseProperty(nameof(DetectionMethod.Planets))]
        public virtual DetectionMethod DetectionMethodNavigation { get; set; } = null!;

        [InverseProperty(nameof(Satellite.PlanetNavigation))]
        public virtual ICollection<Satellite> Satellites { get; set; } = new List<Satellite>();
    }
}