using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_j_articleinterest_ain")]
    [PrimaryKey(nameof(UserId), nameof(ArticleId))]
    public class ArticleInterest
    {
        [Column("art_id")]
        public int ArticleId { get; set; }

        [Column("usr_id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(Article.ArticleInterests))]
        public virtual Article ArticleNavigation { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.ArticleInterests))]
        public virtual User UserNavigation { get; set; } = null!;
    }
}