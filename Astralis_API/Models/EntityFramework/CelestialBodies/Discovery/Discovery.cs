using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_discovery_dis")]
    public class Discovery
    {
        [Key]
        [Column("dis_id")]
        public int Id { get; set; }

        [Column("ceb_id")]
        [Required(ErrorMessage = "The celestial body ID is required.")]
        public int CelestialBodyId { get; set; }

        [Column("dst_id")]
        [Required(ErrorMessage = "The discovery status ID is required.")]
        public int DiscoveryStatusId { get; set; }

        [Column("als_id")]
        public int? AliasStatusId { get; set; }

        [Column("usr_id")]
        [Required(ErrorMessage = "The user ID is required.")]
        public int UserId { get; set; }

        [Column("usr_iddiscapproval")]
        public int? DiscoveryApprovalUserId { get; set; }

        [Column("usr_idaliasapproval")]
        public int? AliasApprovalUserId { get; set; }

        [Column("dis_title")]
        [Required(ErrorMessage = "The title is required.")]
        [StringLength(100, ErrorMessage = "The title cannot be longer than 100 characters.")]
        public string Title { get; set; } = null!;

        [ForeignKey(nameof(CelestialBodyId))]
        [InverseProperty(nameof(CelestialBody.DiscoveryNavigation))]
        public virtual CelestialBody CelestialBodyNavigation { get; set; } = null!;

        [ForeignKey(nameof(DiscoveryStatusId))]
        [InverseProperty(nameof(DiscoveryStatus.Discoveries))]
        public virtual DiscoveryStatus DiscoveryStatusNavigation { get; set; } = null!;

        [ForeignKey(nameof(AliasStatusId))]
        [InverseProperty(nameof(AliasStatus.Discoveries))]
        public virtual AliasStatus? AliasStatusNavigation { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Discoveries))]
        public virtual User UserNavigation { get; set; } = null!;

        [ForeignKey(nameof(DiscoveryApprovalUserId))]
        [InverseProperty(nameof(User.ApprovedDiscoveries))]
        public virtual User? ApprovalUserNavigation { get; set; }

        [ForeignKey(nameof(DiscoveryApprovalUserId))]
        [InverseProperty(nameof(User.ApprovedAliasDiscoveries))]
        public virtual User? ApprovalAliasUserNavigation { get; set; }
    }
}
