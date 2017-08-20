using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GTD.Models
{

    [Serializable]
    public class Project
    {
        [HiddenInput(DisplayValue = false)]
        public int ProjectId { get; set; }

        [Display(Name = @"项目名称")]
        public string ProjectName { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }

        public int? GoalId { get; set; }
        public virtual Goal Goal { get; set; }
        public bool IsComplete { get; set; }
        public bool IsDeleted { get; set; }
    }
}
