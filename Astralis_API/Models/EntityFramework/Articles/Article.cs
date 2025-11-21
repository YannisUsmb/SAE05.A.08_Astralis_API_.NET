using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_article_art")]
    public class Article
    {
        [Key]
        [Column("art_id")]
        public int Id { get; set; }

        [Column("usr_id")]
        [Required(ErrorMessage = "The user ID is required.")]
        public int UserId { get; set; }

        [Column("art_title")]
        [Required(ErrorMessage = "The title is required.")]
        [StringLength(50, ErrorMessage = "The title cannot be longer than 50 characters.")]
        public string Title { get; set; } = null!;

        [Column("art_content", TypeName = "TEXT")]
        [Required(ErrorMessage = "The content is required.")]
        public string Content { get; set; } = null!;

        [Column("art_ispremium")]
        [Required(ErrorMessage = "The premium status of the article is required.")]
        public bool IsPremium { get; set; } = false;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Articles))]
        public virtual User UserNavigation { get; set; } = null!;

        [InverseProperty(nameof(TypeOfArticle.ArticleNavigation))]
        public virtual ICollection<TypeOfArticle> TypeOfArticles { get; set; } = new List<TypeOfArticle>();

        [InverseProperty(nameof(ArticleInterest.ArticleNavigation))]
        public virtual ICollection<ArticleInterest> ArticleInterests { get; set; } = new List<ArticleInterest>();

        [InverseProperty(nameof(Comment.ArticleNavigation))]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}