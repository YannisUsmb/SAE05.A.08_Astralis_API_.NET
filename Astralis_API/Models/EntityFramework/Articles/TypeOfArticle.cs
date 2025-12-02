using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_j_typeofarticle_toa")]
    [PrimaryKey(nameof(ArticleTypeId), nameof(ArticleId))]
    public class TypeOfArticle
    {
        [Column("aty_id")]
        public int ArticleTypeId { get; set; }

        [Column("art_id")]
        public int ArticleId { get; set; }

        [ForeignKey(nameof(ArticleId))]
        [InverseProperty(nameof(Article.TypesOfArticle))]
        public virtual Article ArticleNavigation { get; set; } = null!;

        [ForeignKey(nameof(ArticleTypeId))]
        [InverseProperty(nameof(ArticleType.TypesOfArticle))]
        public virtual ArticleType ArticleTypeNavigation { get; set; } = null!;
    }
}