using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_comment_com")]
    public class Comment
    {
        [Key]
        [Column("com_id")]
        public int Id { get; set; }

        [Column("usr_id")]
        [Required(ErrorMessage = "The user ID is required.")]
        public int UserId { get; set; }

        [Column("art_id")]
        [Required(ErrorMessage = "The article ID is required.")]
        public int ArticleId { get; set; }

        [Column("com_idreply")]
        public int? RepliesToId { get; set; }

        [Column("com_text")]
        [Required(ErrorMessage = "The text is required.")]
        [StringLength(300, ErrorMessage = "The text cannot be longer than 300 characters.")]
        public string Text { get; set; } = null!;

        [Column("com_date")]
        [Required(ErrorMessage = "The date is required.")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Column("com_isvisible")]
        [Required(ErrorMessage = "The visible status of the comment is required.")]
        public bool IsVisible { get; set; } = true;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Comments))]
        public virtual User UserNavigation { get; set; } = null!;

        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(Article.Comments))]
        public virtual Article ArticleNavigation { get; set; } = null!;

        [ForeignKey(nameof(RepliesToId))]
        [InverseProperty(nameof(Comments))]
        public virtual Comment? RepliesToNavigation { get; set; }

        [InverseProperty(nameof(RepliesToNavigation))]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [InverseProperty(nameof(Report.CommentNavigation))]
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}