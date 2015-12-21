using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;

namespace GTD.Filters
{
    public class TaskCount : ActionFilterAttribute
    {
        private ITaskRepository taskRepository;

        public TaskCount ()
        {
            
            
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var viewBag = filterContext.Controller.ViewBag;
            viewBag.TC = CountTask();
        }

        private Dictionary<string, int> CountTask()
        {
            //tc记录各个dateattribute的任务数，显示在导航中
            //每次必须建新的taskrepository,否则会出现数据库更新了，但是Repository不变的情况
            this.taskRepository = new TaskRepository(new GTDContext());
            var tc = new Dictionary<string, int>();

            foreach (DateAttribute da in Enum.GetValues(typeof(DateAttribute)))
            {
                DateAttribute da1 = da;
                tc.Add(da1.ToString(), taskRepository.GetTasks().Where(t =>t.DateAttribute == da1).Where(t => t.IsDeleted == false && t.IsComplete == false).Count());
            }
            return tc;
        }
    }
}