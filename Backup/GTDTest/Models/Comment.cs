using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTDTest.Models
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