using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GTD.Models
{
    [Serializable]
    public class Context
    {
        [HiddenInput(DisplayValue = false)]
        public int ContextId { get; set; }

        [Display(Name = @"场景名称")]
        public string ContextName { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }


    }
}