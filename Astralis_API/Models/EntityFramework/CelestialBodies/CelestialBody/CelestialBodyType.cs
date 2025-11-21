using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_celestialbodytype_cbt")]
    public class CelestialBodyType
    {
        [Key]
        [Column("cbt_id")]
        public int Id { get; set; }

        [Column("cbt_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(50, ErrorMessage = "The label cannot be longer than 50 caracters.")]
        public string Label { get; set; } = null!;

        [Column("cbt_description")]
        [StringLength(300, ErrorMessage = "The description cannot be longer than 300 caracters.")]
        [Required(ErrorMessage = "The description is required.")]
        public string Description { get; set; } = null!;

        [InverseProperty(nameof(CelestialBody.CelestialBodyTypeNavigation))]
        public virtual ICollection<CelestialBody> CelestialBodies { get; set; } = new List<CelestialBody>();

        [InverseProperty(nameof(Audio.CelestialBodyTypeNavigation))]
        public virtual ICollection<Audio> Audios { get; set; } = new List<Audio>();
    }
}