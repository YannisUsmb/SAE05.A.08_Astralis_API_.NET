using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_galaxyquasar_gaq")]
    public class GalaxyQuasar
    {
        [Key]
        [Column("gaq_id")]
        public int Id { get; set; }

        [Column("ceb_id")]
        [Required(ErrorMessage = "The celestial body ID is required.")]
        public int CelestialBodyId { get; set; }

        [Column("gqc_id")]
        [Required(ErrorMessage = "The galaxy/quasar class ID is required.")]
        public int GalaxyQuasarClassId { get; set; }

        [Column("gaq_reference")]
        [StringLength(100, ErrorMessage = "The reference cannot be longer than 100 characters.")]
        public string? Reference { get; set; }

        [Column("gaq_rightascension", TypeName = "NUMERIC(10,6)")]
        public decimal? RightAscension { get; set; }

        [Column("gaq_declination", TypeName = "NUMERIC(10,6)")]
        public decimal? Declination { get; set; }

        [Column("gaq_redshift", TypeName = "NUMERIC(8,6)")]
        public decimal? Redshift { get; set; }

        [Column("gaq_rmagnitude", TypeName = "NUMERIC(8,3)")]
        public decimal? RMagnitude { get; set; }

        [Column("gaq_mjdobs")]
        public int? ModifiedJulianDateObservation { get; set; }

        [ForeignKey(nameof(CelestialBodyId))]
        [InverseProperty(nameof(CelestialBody.GalaxyQuasarNavigation))]
        public virtual CelestialBody CelestialBodyNavigation { get; set; } = null!;

        [ForeignKey(nameof(GalaxyQuasarClassId))]
        [InverseProperty(nameof(GalaxyQuasarClass.GalaxiesQuasars))]
        public virtual GalaxyQuasarClass GalaxyQuasarClassNavigation { get; set; } = null!;
    }
}
