using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GTD.Models
{
    public class Comment
    {

        [HiddenInput(DisplayValue = false)]
        public int CommentId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public int? TaskId { get; set; }
        public virtual Task Task { get; set; }

    }
}