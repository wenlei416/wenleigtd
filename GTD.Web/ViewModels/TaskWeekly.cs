using System.Collections.Generic;
using GTD.Models;

namespace GTD.ViewModels
{
    public class TaskWeekly
    {

        //本周需要完成的任务
        public IEnumerable<Task> WeeklyToDoTasks { get; set; }

        //本周已经完成的任务
        public IEnumerable<Task> WeeklyCompletedTask { get; set; }

    }
}