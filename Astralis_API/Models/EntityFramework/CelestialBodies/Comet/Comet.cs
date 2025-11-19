using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_comet_cmt")]
    public class Comet
    {
        [Key]
        [Column("cmt_id")]
        public int Id { get; set; }

        [Column("ceb_id")]
        [Required(ErrorMessage = "The celestial body ID is required")]
        public int CelestialBodyId { get; set; }

        [Column("cmt_orbitaleccentricity", TypeName = "NUMERIC(6,5)")]
        public decimal? OrbitalEccentricity { get; set; }

        [Column("cmt_orbitalinclinationdeg", TypeName = "NUMERIC(7,4)")]
        public decimal? OrbitalInclinationDegrees { get; set; }

        [Column("cmt_ascendingnodelongitudedeg", TypeName = "NUMERIC(7,4)")]
        public decimal? AscendingNodeLongitudeDegrees { get; set; }

        [Column("cmt_periheliondistanceau", TypeName = "NUMERIC(9,6)")]
        public decimal? PerihelionDistanceAU { get; set; }

        [Column("cmt_apheliondistanceau", TypeName = "NUMERIC(9,3)")]
        public decimal? AphelionDistanceAU { get; set; }

        [Column("cmt_orbitalperiodyears", TypeName = "NUMERIC(10,4)")]
        public decimal? OrbitalPeriodYears { get; set; }

        [Column("cmt_moidau", TypeName = "NUMERIC(9,6)")]
        public decimal? MinimumOrbitIntersectionDistanceAU { get; set; }

        [Column("cmt_ref")]
        [StringLength(250, ErrorMessage = "The reference cannot be longer than 250 caracters.")]
        public string? Reference { get; set; }

        [ForeignKey(nameof(CelestialBodyId))]
        [InverseProperty(nameof(CelestialBody.CometNavigation))]
        public virtual CelestialBody CelestialBodyNavigation { get; set; } = null!;
    }
}
