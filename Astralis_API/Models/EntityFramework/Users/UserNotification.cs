using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_j_usernotification_uno")]
    [PrimaryKey(nameof(UserId),nameof(NotificationId))]
    public class UserNotification
    {
        
        [Column("usr_id")]
        public int UserId { get; set; }

        [Key]
        [Column("not_id")]
        public int NotificationId { get; set; }

        [Column("uno_bymail")]
        [Required(ErrorMessage = "The mail notification status is required.")]
        public bool ByMail { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.UserNotifications))]
        public virtual User UserNavigation { get; set; } = null!;

        [ForeignKey(nameof(NotificationId))]
        [InverseProperty(nameof(Notification.UserNotifications))]
        public virtual Notification NotificationNavigation { get; set; } = null!;

    }
}
