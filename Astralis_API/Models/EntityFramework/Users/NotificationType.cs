using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_notificationtype_nty")]
    public class NotificationType
    {
        [Key]
        [Column("nty_id")]
        public int Id { get; set; }

        [Column("nty_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(100, ErrorMessage = "The label cannot be longer than 100 characters.")]
        public string Label { get; set; } = null!;

        [Column("nty_description")]
        [Required(ErrorMessage = "The description is required.")]
        [StringLength(300, ErrorMessage = "The description cannot be longer than 300 characters.")]
        public string Description { get; set; } = null!;

        [InverseProperty(nameof(Notification.NotificationTypeNavigation))]
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        [InverseProperty(nameof(UserNotificationType.NotificationTypeNavigation))]
        public virtual ICollection<UserNotificationType> UserNotificationTypes { get; set; } = new List<UserNotificationType>();
    }
}
