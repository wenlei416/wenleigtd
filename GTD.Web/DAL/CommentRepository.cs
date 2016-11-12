using GTD.DAL.Abstract;
using GTD.Models;

namespace GTD.DAL
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public Comment GetCommentById(int? commentId)
        {
            return this.Get(t => t.CommentId == commentId);
        }
    }
}