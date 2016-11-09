using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTD.Models
{
    public class Pomodoro
    {
        [HiddenInput(DisplayValue = false)]
        public int PomodoroId { get; set; }

        //番茄时间是完成的=true还是中断的=false
        public bool IsCompletedPomodoro { get; set; }

        //番茄时间的启动和结束时间
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = @"启动时间")]
        public DateTime StarDateTime { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = @"结束时间")]
        public DateTime? EnDateTime { get; set; }

        //这个时间是工作时间=true还是休息时间=false
        public bool IsWorkingTime { get; set; }

        //关联的Task
        public int? TaskId { get; set; }
        public virtual Task Task { get; set; }

    }
}