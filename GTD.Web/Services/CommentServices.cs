using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;

namespace GTD.Services
{
    public class CommentServices:ICommentServices
    {
        private readonly ICommentRepository _commentRepository;

        public CommentServices(ICommentRepository commentRepository)
        {
            this._commentRepository = commentRepository;
        }

        public Comment GetCommentById(int id)
        {
            return _commentRepository.GetCommentById(id);
        }

        public void CreateComment(Comment comment)
        {
            _commentRepository.Create(comment);
        }

        public IEnumerable<Comment> GetAllComments()
        {
            return _commentRepository.GetAll();
        }

        public void UpdateComment(Comment comment)
        {
            _commentRepository.Update(comment);
        }

        public void DeleteComment(Comment comment)
        {
            _commentRepository.Delete(comment);
        }
    }
}