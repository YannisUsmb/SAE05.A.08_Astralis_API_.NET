using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_planettype_plt")]
    public class PlanetType
    {
        [Key]
        [Column("plt_id")]
        public int Id { get; set; }

        [Column("plt_label")]
        [Required(ErrorMessage = "The label is required")]
        [StringLength(30, ErrorMessage = "The label cannot be longer than 30 characters.")]
        public string Label { get; set; } = null!;


        [InverseProperty(nameof(Planet.PlanetTypeNavigation))]
        public virtual ICollection<Planet> Planets { get; set; } = new List<Planet>();
    }
}
