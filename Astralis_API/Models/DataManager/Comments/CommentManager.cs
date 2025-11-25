using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class CommentManager : CrudManager<Comment, int>, ICommentRepository
    {
        public CommentManager(AstralisDbContext context) : base(context)
        {
        }
    }
}