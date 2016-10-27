using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services;
using GTD.Services.Abstract;

namespace GTD.Filters
{
    public class TaskCount : ActionFilterAttribute
    {
        //todo 这里应该和service连接，而不是和repo连接
        //private ITaskRepository taskRepository;
        private ITaskServices _taskServices;


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
            //this.taskRepository = new TaskRepository();
            this._taskServices=new TaskServices();
            var tc = new Dictionary<string, int>();

            foreach (DateAttribute da in Enum.GetValues(typeof(DateAttribute)))
            {
                DateAttribute da1 = da;
                //tc.Add(da1.ToString(), taskRepository.GetAll().Where(t => t.DateAttribute == da1).Count(t => t.IsDeleted == false && t.IsComplete == false));
                tc.Add(da1.ToString(), _taskServices.GetInProgressTasks().Count(t => t.DateAttribute == da1));
            }
            return tc;
        }
    }
}