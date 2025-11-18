using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_commandstatus_cos")]
    public class CommandStatus
    {
        [Key]
        [Column("cos_id")]
        public int Id { get; set; }

        [Column("cos_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(30, ErrorMessage = "The label cannot be longer than 30 characters.")]
        public string Label { get; set; } = null!;

        [InverseProperty(nameof(Command.CommandStatusNavigation))]
        public virtual ICollection<Command> CommandsNavigation { get; set; } = new List<Command>();
    }
}
