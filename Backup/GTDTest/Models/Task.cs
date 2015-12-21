﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GTDTest.DAL;

namespace GTDTest.Models
{
    public class Task
    {
        [HiddenInput(DisplayValue = false)]
        public int TaskId { get; set; }

        [Required(ErrorMessage = @"标题必填")]
        [Display(Name = @"标题")]
        public string Headline { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = @"描述")]
        public string Description { get; set; }

        [Display(Name = @"项目")]
        public int? ProjectID { get; set; }
        public virtual Project Pro { get; set; }

        [Display(Name = @"场景")]
        public int? ContextID { get; set; }
        public virtual Context Context { get; set; }

        [Display(Name = @"优先级")]
        public Priority? Priority { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = @"开始时间")]
        public DateTime? StartDateTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = @"结束时间")]
        public DateTime? CloseDateTime { get; set; }

        [Display(Name = @"属性")]
        public DateAttribute? DateAttribute { get; set; }

        public virtual ICollection<SubTask> SubTasks { get; set; }

        public bool IsComplete { get; set; }

        public bool IsDeleted { get; set; }

        [HiddenInput(DisplayValue = false)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = @"实际完成时间")]
        public DateTime? CompleteDateTime { get; set; }

        [Display(Name = @"后续任务")]
        public int? NextTask_TaskId { get; set; }

        [Display(Name = @"前置任务")]
        public int? PreviousTask_TaskId { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }


    }
}