using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_discoverystatus_dst")]
    public class DiscoveryStatus
    {
        [Key]
        [Column("dst_id")]
        public int Id { get; set; }

        [Column("dst_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(30, ErrorMessage = "The label cannot be longer than 30 caracters.")]
        public string Label { get; set; } = null!;

        [InverseProperty(nameof(Discovery.DiscoveryStatusNavigation))]
        public virtual ICollection<Discovery> Discoveries { get; set; } = new List<Discovery>();
    }
}
