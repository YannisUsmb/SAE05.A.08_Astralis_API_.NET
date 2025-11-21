using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_audio_aud")]
    public class Audio
    {
        [Key]
        [Column("aud_id")]
        public int Id { get; set; }

        [Column("cbt_id")]
        [Required(ErrorMessage = "The celestial body type ID is required.")]
        public int CelestialBodyTypeId { get; set; }

        [Column("aud_title")]
        [StringLength(50, ErrorMessage = "The title cannot be longer than 50 characters.")]
        [Required(ErrorMessage = "The title is required.")]
        public string Title { get; set; } = null!;

        [Column("aud_decription")]
        [StringLength(300, ErrorMessage = "The description cannot be longer than 300 characters.")]
        [Required(ErrorMessage = "The description is required.")]
        public string Description { get; set; } = null!;

        [Column("aud_filepath")]
        [StringLength(500, ErrorMessage = "The file path cannot be longer than 500 characters.")]
        [Required(ErrorMessage = "The file path is required.")] 
        public string FilePath { get; set; } = null!;

        [ForeignKey(nameof(CelestialBodyTypeId))]
        [InverseProperty(nameof(CelestialBodyType.Audios))]
        public virtual CelestialBodyType CelestialBodyTypeNavigation { get; set; } = null!;
    }
}
