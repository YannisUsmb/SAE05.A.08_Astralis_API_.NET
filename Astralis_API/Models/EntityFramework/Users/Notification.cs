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

        [Column("nty_id")]
        [Required(ErrorMessage = "The notification type ID is required.")]
        public int NotificationTypeId { get; set; }

        [Column("not_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(100, ErrorMessage = "The label cannot be longer than 100 characters.")]
        public string Label { get; set; } = null!;

        [Column("not_description")]
        [StringLength(300, ErrorMessage = "The description cannot be longer than 300 characters.")]
        public string? Description { get; set; }

        [Column("not_link")]
        [StringLength(250)]
        public string? Link { get; set; }

        [InverseProperty(nameof(UserNotification.NotificationNavigation))]
        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();

        [ForeignKey(nameof(NotificationTypeId))]
        [InverseProperty(nameof(NotificationType.Notifications))]
        public virtual NotificationType NotificationTypeNavigation { get; set; } = null!;
    }
}