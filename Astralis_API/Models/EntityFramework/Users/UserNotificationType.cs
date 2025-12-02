using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_j_usernotificationtype_unt")]
    [PrimaryKey(nameof(UserId), nameof(NotificationTypeId))]
    public class UserNotificationType
    {
        [Column("usr_id")]
        public int UserId { get; set; }

        [Column("nty_id")]
        public int NotificationTypeId { get; set; }

        [Column("unt_bymail")]
        [Required(ErrorMessage = "The mail notification status is required.")]
        public bool ByMail { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.UserNotificationTypes))]
        public virtual User UserNavigation { get; set; } = null!;

        [ForeignKey(nameof(NotificationTypeId))]
        [InverseProperty(nameof(NotificationType.UserNotificationTypes))]
        public virtual NotificationType NotificationTypeNavigation { get; set; } = null!;
    }
}
