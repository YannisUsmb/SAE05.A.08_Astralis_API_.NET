using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_galaxyquasarclass_gqc")]
    public class GalaxyQuasarClass
    {
        [Key]
        [Column("gqc_id")]
        public int Id { get; set; }

        [Column("gqc_label")]
        [Required(ErrorMessage = "The label is required")]
        [StringLength(20, ErrorMessage = "The label cannot be longer than 20 caracters.")]
        public string Label { get; set; } = null!;


        [InverseProperty(nameof(GalaxyQuasar.GalaxyQuasarClassNavigation))]
        public virtual ICollection<GalaxyQuasar> GalaxiesQuasars { get; set; } = new List<GalaxyQuasar>();
    }
}
