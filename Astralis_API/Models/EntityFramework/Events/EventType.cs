using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_eventtype_evt")]
    public class EventType
    {
        [Key]
        [Column("evt_id")]
        public int Id { get; set; }

        [Column("evt_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(50, ErrorMessage = "The label cannot be longer than 50 characters.")]
        public string Label { get; set; } = null!;

        [Column("evt_description")]
        [StringLength(300, ErrorMessage = "The description cannot be longer than 300 characters.")]
        public string? Description { get; set; }

        [InverseProperty(nameof(Event.EventType))]
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}