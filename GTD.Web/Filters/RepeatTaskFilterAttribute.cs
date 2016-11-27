using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.Core.Internal;
using GTD.Models;
using GTD.Services.Abstract;
using GTD.Util;
using Ninject;

namespace GTD.Filters
{
    public class RepeatTaskFilterAttribute : ActionFilterAttribute
    {
        [Inject]
        public ITaskServices TaskServices { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var requestCookie = HttpContext.Current.Request.Cookies["lastCreateRepeatTaskDate"];
            if (requestCookie != null)
            {
                //获取cookie中的最后创建日期
                var lastCreateRepeatTaskDate = Convert.ToDateTime(requestCookie.Value,
                    new DateTimeFormatInfo() { ShortDatePattern = "yyyyMMdd" });
                if (lastCreateRepeatTaskDate >= DateTime.Now.Date)
                    return;
            }

            //按RepeatJson分组
            var groupByRepeatJson = TaskServices.GetInProgressTasks()
                .Where(t => t.RepeatJson.IsNullOrEmpty())
                .GroupBy(t => t.RepeatJson);

            var toBeCreadedTasks=GetValue(groupByRepeatJson);
            //在这里集中创建会比较快
            foreach (var creadedTask in toBeCreadedTasks)
            {
                TaskServices.AddTask(creadedTask);
            }
            //创建完任务，处理cookie
            if (requestCookie != null)
                requestCookie.Value = DateTime.Now.Date.ToString("yyyyMMdd");
            else
            {
                //创建cookie
                HttpCookie cookie = new HttpCookie("lastCreateRepeatTaskDate") { Value = DateTime.Now.Date.ToString("yyyyMMdd") };
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public static List<Task> GetValue(IEnumerable<IGrouping<string, Task>> groupByRepeatJson)
        {
            List<Task> toBeCreadedTasks=new List<Task>();
            //先在组级别循环（组是按RepeatJson分的）
            foreach (var tasks in groupByRepeatJson)
            {
                //在分组内部循环
                foreach (var t in tasks)
                {
                    //根据组内任务创建出循环任务，其实这个在整个循环是适用的
                    var cycTasks = TaskUtil.CreateCycTasks(t);

                    //比较两个list是否一样，新创建的cycTasks考虑了今天的时间，所以和existTasks不同的可能性非常大
                    //因为existTasks中可能存在以前的任务，所以只能考查cycTasks是不是在existTasks中都存在
                    //这里的判断标准不完善，只判断cycTask的开始日期在existTasks中是否存在
                    var exitsTaskStarDateTimes = (from e in tasks select e.StartDateTime).ToList();

                    foreach (var cycTask in cycTasks)
                    {
                        //存在就说明有，不存在就说明这个任务没有
                        if (!exitsTaskStarDateTimes.Contains(cycTask.StartDateTime))
                        {
                            toBeCreadedTasks.Add(cycTask);
                        }
                    }
                    //把tasks中一样RepeatJson的都跳过。
                    break;
                }
            }
            return toBeCreadedTasks;
        }
    }
}