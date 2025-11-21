using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_orbitalclass_oct")]
    public class OrbitalClass
    {
        [Key]
        [Column("oct_id")]
        public int Id { get; set; }

        [Column("oct_label", TypeName = "CHAR(3)")]
        [Required(ErrorMessage = "The label is required.")]
        public string Label { get; set; } = null!;

        [Column("oct_description")]
        [Required(ErrorMessage = "The description is required.")]
        [StringLength(200, ErrorMessage = "The description cannot be longer than 200 characters.")]
        public string Description { get; set; } = null!;

        [InverseProperty(nameof(Asteroid.OrbitalClassNavigation))]
        public virtual ICollection<Asteroid> Asteroids { get; set; } = new List<Asteroid>();
    }
}