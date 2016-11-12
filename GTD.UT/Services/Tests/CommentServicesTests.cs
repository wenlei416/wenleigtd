using System.Collections.Generic;
using System.Linq;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services;
using GTD.Services.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GTD.UT.Services.Tests
{
    [TestClass()]
    public class CommentServicesTests
    {
        private  ICommentServices _commentServices;
        public Mock<ICommentRepository> MockCommentRepository()
        {
            Mock<ICommentRepository> mock = new Mock<ICommentRepository>();

            mock.Setup(m => m.GetCommentById(1)).Returns(new Comment
            {
                CommentId = 1,
                Description = "备注",
                TaskId = 2
            });

            mock.Setup(m => m.GetAll()).Returns(new List<Comment>
            {
                new Comment{
                CommentId = 1,
                Description = "备注1",
                TaskId = 2
            },
            new Comment{
                CommentId = 2,
                Description = "备注2",
                TaskId = 3
            },
            new Comment{
                CommentId = 3,
                Description = "备注3",
                TaskId = 4
            }
                
            }.AsQueryable());
            return mock;

        }

        [TestInitialize()]
        public void CommentServicesTestInitialize()
        {
            var m = MockCommentRepository();
            this._commentServices = new CommentServices(m.Object);

        }

        [TestMethod()]
        public void GetCommentByIdTest()
        {
            //var m = MockCommentRepository();
            //ICommentServices commentServices = new CommentServices(m.Object);
            var comment = _commentServices.GetCommentById(1);

            Assert.AreEqual(comment.CommentId, 1);
            Assert.AreEqual(comment.TaskId, 2);
            Assert.AreEqual(comment.Description, "备注");
        }

        [TestMethod()]
        public void GetAllCommentsTest()
        {
            //var m = MockCommentRepository();
            //ICommentServices commentServices = new CommentServices(m.Object);
            var comments = _commentServices.GetAllComments();

            Assert.AreEqual(comments.Count(),3);

        }
    }
}
