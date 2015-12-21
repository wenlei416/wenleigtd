using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTDTest.Models
{
    public class SubTask
    {
        [HiddenInput(DisplayValue = false)]
        public int SubTaskId { get; set; }

        public string SubTaskName { get; set; }

        [Display(Name = @"任务")]
        public int TaskId { get; set; }
        public virtual Task Task { get; set; }
        public bool IsComplete { get; set; }
        public bool IsDeleted { get; set; }

    }
}