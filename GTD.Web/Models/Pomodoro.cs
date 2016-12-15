using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GTD.Models
{

    [Serializable]
    public class Pomodoro
    {
        [HiddenInput(DisplayValue = false)]
        public int PomodoroId { get; set; }

        //番茄时间是完成的=true还是中断的=false
        [Display(Name = @"是否完成")]
        public bool IsCompletedPomodoro { get; set; }

        //番茄时间的启动和结束时间
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = @"启动时间")]
        public DateTime StarDateTime { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = @"结束时间")]
        public DateTime? EnDateTime { get; set; }

        //这个时间是工作时间=true还是休息时间=false
        [Display(Name = @"工作？休息")]
        public bool IsWorkingTime { get; set; }

        //关联的Task
        [Display(Name = @"任务名称")]
        public int? TaskId { get; set; }

        public virtual Task Task { get; set; }

    }
}