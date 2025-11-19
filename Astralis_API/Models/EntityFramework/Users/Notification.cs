using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_notification_not")]
    public class Notification
    {
        [Key]
        [Column("not_id")]
        public int Id { get; set; }

        [Column("not_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(100, ErrorMessage = "The label cannot be longer than 100 caracters.")]
        public string Label { get; set; } = null!;

        [Column("not_description")]
        [StringLength(300, ErrorMessage = "The description cannot be longer than 300 caracters.")]
        public string? Description { get; set; }

        [InverseProperty(nameof(UserNotification.NotificationNavigation))]
        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }
}