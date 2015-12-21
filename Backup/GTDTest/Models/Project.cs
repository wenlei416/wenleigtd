using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GTDTest.Controllers;

namespace GTDTest.Models
{
    public class Project
    {
        [HiddenInput(DisplayValue = false)]
        public int ProjectId { get; set; }

        [Display(Name = @"项目名称")]
        public string ProjectName { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }

        public virtual Goal Goal { get; set; }
        public bool IsComplete { get; set; }
        public bool IsDeleted { get; set; }
    }
}
