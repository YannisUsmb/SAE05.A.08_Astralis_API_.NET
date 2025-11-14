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
        public virtual Event Event { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
