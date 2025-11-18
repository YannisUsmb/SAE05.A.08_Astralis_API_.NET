using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_j_eventinterest_evi")]
    public class EventInterest
    {
        [Column("eve_id")]
        public int EventId { get; set; }

        [Column("usr_id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(EventId))]
        [InverseProperty(nameof(Event.EventInterests))]
        public virtual Event EventNavigation { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.EventInterests))]
        public virtual User UserNavigation { get; set; } = null!;
    }
}
