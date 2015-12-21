using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GTD.Models;

namespace GTD.ViewModels.ProjectVM
{
    public class ProjectDeatilVM
    {
        public Project Project { get; set; }

        //项目下未完成的任务
        public IEnumerable<Task> ToDoTasks { get; set; }

        //项目下已经完成的任务
        public IEnumerable<Task> CompletedTasks { get; set; }
    }
}