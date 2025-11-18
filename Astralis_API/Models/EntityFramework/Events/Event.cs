using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_event_eve")]
    public class Event : IValidatableObject
    {
        [Key]
        [Column("eve_id")]
        public int Id { get; set; }

        [Column("evt_id")]
        [Required(ErrorMessage = "The event type ID is required.")]
        public int EventTypeId { get; set; }

        [Column("usr_id")]
        [Required(ErrorMessage = "The user ID is required.")]
        public int UserId { get; set; }

        [Column("eve_title")]
        [Required(ErrorMessage = "The title is required.")]
        [StringLength(50, ErrorMessage = "The title cannot be longer than 50 characters.")]
        public string Title { get; set; } = null!;

        [Column("eve_description")]
        [Required(ErrorMessage = "The description is required.")]
        [StringLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
        public string Description { get; set; } = null!;

        [Column("eve_startdate")]
        [Required(ErrorMessage = "The start date is required.")]
        public DateTime StartDate { get; set; }

        [Column("eve_enddate")]
        public DateTime? EndDate { get; set; }

        [Column("eve_location")]
        [StringLength(100, ErrorMessage = "The location cannot be longer than 100 characters.")]
        public string? Location { get; set; }

        [Column("eve_link", TypeName = "TEXT")]
        [Url(ErrorMessage = "The link must be a valid URL.")]
        public string? Link { get; set; }

        [ForeignKey(nameof(EventTypeId))]
        [InverseProperty(nameof(EventType.Events))]
        public virtual EventType EventTypeNavigation { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Events))]
        public virtual User UserNavigation { get; set; } = null!;

        [InverseProperty(nameof(EventInterest.EventNavigation))]
        public virtual ICollection<EventInterest> EventInterests { get; set; } = new List<EventInterest>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate.HasValue && EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "The end date must be on or after the start date.",
                    new[] { nameof(EndDate), nameof(StartDate) }
                );
            }
        }
    }
}
