using System.Collections.Generic;
using GTD.Models;

namespace GTD.Services.Abstract
{
    public interface ICommentServices
    {
        Comment GetCommentById(int id);
        void CreateComment(Comment comment);
        IEnumerable<Comment> GetAllComments();
        void UpdateComment(Comment comment);
        void DeleteComment(Comment comment);
    }
}
