using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_j_usernotification_uno")]
    [PrimaryKey(nameof(UserId), nameof(NotificationId))]
    public class UserNotification
    {
        [Column("usr_id")]
        public int UserId { get; set; }

        [Column("not_id")]
        public int NotificationId { get; set; }

        [Column("uno_isread")]
        [Required(ErrorMessage = "The readed status is required.")]
        public bool IsRead { get; set; }

        [Column("uno_receivedat")]
        [Required(ErrorMessage = "The received date is required.")]
        public DateTime ReceivedAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.UserNotifications))]
        public virtual User UserNavigation { get; set; } = null!;

        [ForeignKey(nameof(NotificationId))]
        [InverseProperty(nameof(Notification.UserNotifications))]
        public virtual Notification NotificationNavigation { get; set; } = null!;
    }
}