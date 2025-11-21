using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_articletype_aty")]
    public class ArticleType
    {
        [Key]
        [Column("aty_id")]
        public int Id { get; set; }

        [Column("aty_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(50, ErrorMessage = "The title cannot be longer than 50 characters.")]
        public string Label { get; set; } = null!;

        [Column("aty_description")]
        [StringLength(300, ErrorMessage = "The description cannot be longer than 300 characters.")]
        public string? Description { get; set; }

        [InverseProperty(nameof(TypeOfArticle.ArticleTypeNavigation))]
        public virtual ICollection<TypeOfArticle> TypesOfArticle { get; set; } = new List<TypeOfArticle>();
    }
}