using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GTD.Models
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