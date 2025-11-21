using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_aliasstatus_als")]
    public class AliasStatus
    {
        [Key]
        [Column("als_id")]
        public int Id { get; set; }

        [Column("als_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(30, ErrorMessage = "The label cannot be longer than 30 caracters.")]
        public string Label { get; set; } = null!;

        [InverseProperty(nameof(Discovery.AliasStatusNavigation))]
        public virtual ICollection<Discovery> Discoveries { get; set; } = new List<Discovery>();
    }
}
