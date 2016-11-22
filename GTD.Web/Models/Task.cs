using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Mvc;
using GTD.Util;

namespace GTD.Models
{
    [Serializable]
    public class Task:ICloneable
    {
        [HiddenInput(DisplayValue = false)]
        public int TaskId { get; set; }

        [Required(ErrorMessage = @"标题必填")]
        [Display(Name = @"标题")]
        public string Headline { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = @"描述")]
        public string Description { get; set; }


        public int? ProjectID { get; set; }
        [Display(Name = @"项目")]
        public virtual Project Pro { get; set; }

        public int? ContextID { get; set; }

        [Display(Name = @"场景")]
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
        [DateGreaterThan("StartDateTime","结束时间必须大于开始时间")]
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

        public virtual ICollection<Pomodoro> Pomodoros { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string RepeatJson { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }

        public object Clone()
        {
            using (MemoryStream objectStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                //Task t = (Task)formatter.Deserialize(objectStream);
                return formatter.Deserialize(objectStream);
            }
        }
    }
}