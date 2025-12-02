using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CommentManager : CrudManager<Comment, int>, ICommentRepository
    {
        public CommentManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<Comment?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(c => c.Id == id);
        }

        protected override IQueryable<Comment> WithIncludes(IQueryable<Comment> query)
        {
            return query.Include(c => c.ArticleNavigation)
                        .Include(c => c.RepliesToNavigation)
                        .Include(c => c.UserNavigation)
                        .Include(c => c.Comments)
                        .Include(c => c.Reports);
        }
    }
}