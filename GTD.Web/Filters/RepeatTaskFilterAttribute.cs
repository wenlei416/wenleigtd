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
            var requestCookie = HttpContext.Current.Request.Cookies["lastCreateRepeatTaskDate"];
            //如果有有这个cookie
            if (requestCookie != null)
            {
                //获取cookie中的最后创建日期，与今天比较。如果大于今天，说明今天已经创建过了，结束
                var lastCreateRepeatTaskDate = Convert.ToDateTime(requestCookie.Value,
                    new DateTimeFormatInfo() { ShortDatePattern = "yyyyMMdd" });
                if (lastCreateRepeatTaskDate >= DateTime.Now.Date)
                    return;
            }
            //否则，不管是cookie是null，还是说cookie的日期比今天小，都需要创建
            //按RepeatJson分组
            var groupByRepeatJson = TaskServices.GetAll()
                .Where(t => t.RepeatJson.IsNullOrEmpty())
                .GroupBy(t => t.RepeatJson);
            //取得需要创建的任务
            //todo 但这里会有个问题，积累的任务会越来越多，效率会成为问题
            var toBeCreadedTasks =GetToBeCreadedTasks(groupByRepeatJson);
            //在这里集中创建会比较快
            foreach (var creadedTask in toBeCreadedTasks)
            {
                TaskServices.AddTask(creadedTask);
            }
            //创建完任务，处理cookie。cookie不为空就修改，为空就创建
            if (requestCookie != null)
                requestCookie.Value = DateTime.Now.Date.ToString("yyyyMMdd");
            else
            {
                //创建cookie
                HttpCookie cookie = new HttpCookie("lastCreateRepeatTaskDate") { Value = DateTime.Now.Date.ToString("yyyyMMdd") };
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        //todo UT
        //根据输入的任务组，返回需要创建的循环任务
        //传入的是所有的任务，所以完成的/删除任务也会被传入进来
        public static List<Task> GetToBeCreadedTasks(IEnumerable<IGrouping<string, Task>> groupByRepeatJson)
        {
            List<Task> toBeCreadedTasks=new List<Task>();
            //先在组级别循环1（组是按RepeatJson分的）
            foreach (var tasks in groupByRepeatJson)
            {
                //todo 这两个循环似乎可以减少成一个，因为内环永远只执行第一个
                //var firstOrDefault = tasks.FirstOrDefault() ?? tasks.FirstOrDefault();
                //在分组内部循环2
                foreach (var t in tasks)
                {

                    //如果t是完成或删除了的，就跳过，这主要是保证如果一个循环内所有任务都完成/删除了，其实是不需要再考虑重建的
                    //不能这么写，如果分组任务都被完成了，然后新任务没出来之前，这个重复任务就被干掉了
                    //if(t.IsComplete || t.IsDeleted)
                    //    continue;
                    //根据组内任务创建出循环任务，其实这个在整个循环2中都是适用的
                    //如果昨天应该有循环任务，但是没创建，怎么办
                    //这里似乎更应该用jsontoDate，来搞定时间，然后用clone+修改时间的方法来增加新任务
                    //var cycTasks = TaskUtil.CreateCycTasks(t);

                    var cycDates = RecurringDate.RecurringJsonToDate(t.RepeatJson);
                    //比较两个list是否一样，新创建的cycTasks考虑了今天的时间，所以和existTasks不同的可能性非常大
                    //因为existTasks中可能存在以前的任务，所以只能考查cycTasks是不是在existTasks中都存在
                    //这里的判断标准不完善，只判断cycTask的开始日期在existTasks中是否存在
                    var exitsTaskStarDateTimes = (from e in tasks select e.StartDateTime).ToList();

                    foreach (var cycDate in cycDates)
                    {
                        if (!exitsTaskStarDateTimes.Contains(cycDate))
                        {
                            var newTask = (Task) t.Clone();
                            var taskDuration= TaskUtil.TaskDuration(t);
                            toBeCreadedTasks.Add(newTask);
                            //这里Clone出来的Task可能是有id的，后面直接拿有id的Task去写入db，不会有问题，会直接把id省略掉，重新自动生成id
                            newTask.StartDateTime = cycDate;
                            //处理没有结束日期的任务
                            newTask.CloseDateTime = t.CloseDateTime != null
                                ? (DateTime?)cycDate.AddDays(taskDuration)
                                : null;
                            newTask.DateAttribute = TaskUtil.SetDateAttribute(t.StartDateTime, t.DateAttribute, t.ProjectID);
                        }
                    }

                    //foreach (var cycTask in cycTasks)
                    //{
                    //    //存在就说明有，不存在就说明这个任务没有
                    //    if (!exitsTaskStarDateTimes.Contains(cycTask.StartDateTime))
                    //    {
                    //        toBeCreadedTasks.Add(cycTask);
                    //    }
                    //}

                    //把tasks中一样RepeatJson的都跳过。
                    break;
                }
            }
            return toBeCreadedTasks;
        }
    }
}