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
    //用于加载在制定的Action上，当这个Action执行的时候，检查是否有新的循环任务需要创建
    public class RepeatTaskFilterAttribute : ActionFilterAttribute
    {
        [Inject]
        public ITaskServices TaskServices { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //读取cookie
            //cookie用于记录最后一次执行创建新重复任务的日期
            var requestCookie = filterContext.HttpContext.Request.Cookies["lastCreateRepeatTaskDate"];
            //如果有有这个cookie
            if (requestCookie != null)
            {
                //获取cookie中的最后创建日期，与今天比较。如果大于今天，说明今天已经创建过了，结束
                var lastCreateRepeatTaskDate = DateTime.ParseExact(requestCookie.Value, "yyyyMMdd", CultureInfo.CurrentCulture);

                if (lastCreateRepeatTaskDate >= DateTime.Now.Date)
                    return;
            }
            //否则，不管是cookie是null，还是说cookie的日期比今天小，都需要创建
            //按RepeatJson分组
            var groupByRepeatJson = TaskServices.GetAll()
                .Where(t => !t.RepeatJson.IsNullOrEmpty())
                .GroupBy(t => t.RepeatJson);
            //取得需要创建的任务
            //todo 但这里会有个问题，积累的任务会越来越多，效率会成为问题
            //可以在这里循环一下比较一下，如果循环任务的结束日期小于今天，就不要了
            //其实还是有循环，只是比较的内容稍微少一点，比在GetToBeCreadedTasks里面做快
            var toBeCreadedTasks = GetToBeCreadedTasks(groupByRepeatJson);
            //在这里集中创建会比较快
            foreach (var creadedTask in toBeCreadedTasks)
            {
                TaskServices.AddTaskFromFilter(creadedTask);
            }
            //创建完任务，处理cookie。cookie不为空就修改，为空就创建
            if (requestCookie != null)
            {
                requestCookie.Value = DateTime.Now.Date.ToString("yyyyMMdd");
                requestCookie.Expires = DateTime.Now.AddDays(2);
            }
            else
            {
                //创建cookie
                HttpCookie cookie = new HttpCookie("lastCreateRepeatTaskDate")
                {
                    Value = DateTime.Now.Date.ToString("yyyyMMdd"),
                    Expires = DateTime.Now.AddDays(2)
                };
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        //todo UT
        //根据输入的任务组，返回需要创建的循环任务
        //传入的是所有的任务，所以完成的/删除任务也会被传入进来
        public static List<Task> GetToBeCreadedTasks(IEnumerable<IGrouping<string, Task>> groupByRepeatJson)
        {
            List<Task> toBeCreadedTasks = new List<Task>();
            //先在组级别循环1（组是按RepeatJson分的）
            foreach (var tasks in groupByRepeatJson)
            {
                //todo 这两个循环可以减少成一个，因为内环永远只执行第一个
                //var firstOrDefault = tasks.FirstOrDefault();
                //if (firstOrDefault != null) firstOrDefault.RepeatJson = "";
                //在分组内部循环2
                foreach (var t in tasks)
                {
                    //如果t是完成或删除了的，就跳过，这主要是保证如果一个循环内所有任务都完成/删除了，其实是不需要再考虑重建的
                    //要小心：如果分组任务都被完成了，然后新任务没出来之前，重复任务被干掉

                    //根据组内任务创建出循环任务，其实这个在整个循环2中都是适用的

                    //问题：如果昨天应该有循环任务，但是没创建
                    //解决：用jsontoDate，来搞定start时间，然后用clone+修改时间的方法来增加新任务

                    var cycDates = RecurringDate.RecurringJsonToDate(t.RepeatJson);
                    //把不能用于创建任务的日期去掉，比如可能成为第二个日程的时间
                    var cycDatesForToBeCreadedTasks = new List<DateTime>();
                    for (int i = 0; i < cycDates.Count; i++)
                    {
                        cycDatesForToBeCreadedTasks.Add(cycDates[i]);

                        if (cycDates[i].Date > DateTime.Now.AddDays(1).Date)
                        {
                            break;
                        }
                    }
                    //比较两个list是否一样，新创建的cycTasks考虑了今天的时间，所以和existTasks不同的可能性非常大
                    //因为existTasks中可能存在以前的任务，所以只能考查cycTasks是不是在existTasks中都存在
                    //这里的判断标准不完善，只判断cycTask的开始日期在existTasks中是否存在
                    var exitsTaskStarDateTimes = (from e in tasks select e.StartDateTime).ToList();
                    foreach (var cycDate in cycDatesForToBeCreadedTasks)
                    {
                        if (exitsTaskStarDateTimes.Contains(cycDate))
                            continue;

                        var newTask = TaskUtil.CloneTaskForRepeat(t, cycDate);
                        toBeCreadedTasks.Add(newTask);
                    }

                    //把tasks中一样RepeatJson的都跳过。
                    break;
                }
            }
            return toBeCreadedTasks;
        }
    }
}