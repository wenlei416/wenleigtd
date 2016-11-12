using GTD.Models;

namespace GTD.DAL.Abstract
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Comment GetCommentById(int? commentId);
    }
}
