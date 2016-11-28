using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;
using GTD.Models;
using Microsoft.Ajax.Utilities;

namespace GTD.Util
{
    public static class TaskUtil
    {
        public static string CssForOutCloseDateTime(this Task task)
        {
            if (task.CloseDateTime != null)
            {
                if (task.CloseDateTime > DateTime.Today)
                {
                    return "inschedule";
                }
                else
                {
                    return "outschedule";
                }
            }
            return "noschedule";
        }

        public static string CssForIsComplete(this Task task)
        {
            if (task.IsComplete)
            {
                return "done";
            }
            else
            {
                return "doing";
            }
        }

        public static string CssForPriority(this Task task)
        {
                switch (task.Priority)
                {
                    case Priority.高:
                        return "high";
                    case Priority.中:
                        return "mid";
                    case Priority.低:
                        return "low";
                    default:
                        return "none";
                }
        }

        private static int? RemainderTime(this Task task)
        {
           
                if (task.CloseDateTime != null)
                {
                    return (task.CloseDateTime - DateTime.Today).Value.Days;
                }
                return null;
        }

        public static string RemainderTimeString(this Task task)
        {
           
                if (task.RemainderTime() != null)
                {
                    if (task.RemainderTime() == 0)
                    {
                        return "最后1天";
                    }
                    if (task.RemainderTime() < 0)
                    {
                        return "过期" + Math.Abs((int)task.RemainderTime()) + "天";
                    }
                    if (task.RemainderTime() > 0)
                    {
                        return "还剩" + task.RemainderTime() + "天";
                    }
                }
                return string.Empty;
        }

        public static string RecurringString(this Task task)
        {
            return task.RepeatJson.IsNullOrWhiteSpace() ? "" : RecurringDate.JsonToString(task.RepeatJson);
        }

        /// <summary>
        /// 为修改repeat任务设置，判断修改的属性是否在需要统一的属性中
        /// </summary>
        /// <param name="oldTask"></param>
        /// <param name="newTask"></param>
        /// <returns></returns>
        public static bool ModifiedPropertiesInList(Task oldTask, Task newTask)
        {
            //需要批量更新的修改：标题，描述，项目，场景，优先级。其他的都需要批量更新，允许不一致
            var properitiesList = new string[] { "Headline", "Description", "ProjectID", "ContextID", "Priority" };
            foreach (var s in properitiesList)
            {
                if (typeof(Task).GetProperty(s).GetValue(newTask, null) !=
                    oldTask.GetType().GetProperty(s).GetValue(oldTask, null))
                {
                    return true;
                }
            }
            return false;
        }

        public static IQueryable<Task> UpdateRepeatTasksProperties(IQueryable<Task> repeatTasks, Task templateTask)
        {
            List<Task> resulTasks = new List<Task>();
            var properitiesList = new string[] { "Headline", "Description", "ProjectID", "ContextID", "Priority" };
            //需要批量更新的修改：标题，描述，项目，场景，优先级。其他的都需要批量更新，允许不一致
            foreach (var repeatTask in repeatTasks)
            {
                foreach (var s in properitiesList)
                {
                    typeof(Task).GetProperty(s).SetValue(repeatTask, templateTask.GetType().GetProperty(s).GetValue(templateTask, null));
                }
                resulTasks.Add(repeatTask);
            }

            return resulTasks.AsQueryable();
        }


        //输入文本，返回项目名称
        public static string GetProjectNameFromText(string tasktext)
        {
            string projectname = null;
            var t1 = tasktext.Trim();
            if (t1.IsEmpty()) return null;

            //开头是#：#到第一个空格作为项目名称
            if (t1.IndexOf("#", StringComparison.Ordinal) == 0)
            {
                //用第一个空格的位置来确定项目名称
                var i = t1.IndexOf(" ", StringComparison.Ordinal);
                if (i > 0)
                {
                    projectname = t1.Substring(1, i - 1).Trim();
                }
            }

            //其他内容开头：空格#到紧接的空格作为项目名称
            if (t1.IndexOf(" #", StringComparison.Ordinal) > 0)
            {
                int projectstart = t1.IndexOf(" #", StringComparison.Ordinal);
                projectname = t1.Substring(projectstart + 2, t1.Length - projectstart - 2).Trim();
            }

            //没有#的：没有project
            return projectname;
        }

        //输入文本，返回任务名称
        public static string GetTaskNameFromText(string tasktext)
        {
            string taskname = null;
            var t1 = tasktext.Trim();
            if (t1.IsEmpty()) return null;

            //开头是#：#到第一个空格作为项目名称，空格后所有内容作为项目标题（不去中间空格）
            if (t1.IndexOf("#", StringComparison.Ordinal) == 0)
            {
                int i = t1.IndexOf(" ", StringComparison.Ordinal);
                if (i > 0)
                {
                    taskname = t1.Substring(i + 1).Trim();
                }
            }

            //其他内容开头：空格#到紧接的空格，作为项目名称。项目名称之前的内容作为任务名称，项目名称之后的内容废弃。
            if (t1.IndexOf(" #", StringComparison.Ordinal) > 0)
            {
                var i = t1.IndexOf(" #", StringComparison.Ordinal);
                taskname = t1.Substring(0, i).Trim();
            }

            //没有#的：所有内容作为task
            if (t1.IndexOf("#", StringComparison.Ordinal) < 0)
            {
                taskname = t1;
            }
            return taskname;
        }

        /// <summary>
        /// 计算任务周期
        /// </summary>
        /// <param name="task"></param>
        /// <returns>
        /// 如果没有结束日期， 返回的是0;
        /// 如果有结束日期，但结束日期和开始日期是同一天，返回的也是0
        /// </returns>
        public static int TaskDuration(Task task)
        {
            int taskDuration = 0;
            var timeSpan = task.CloseDateTime - task.StartDateTime;
            if (timeSpan == null) return taskDuration;
            TimeSpan ts = (TimeSpan)timeSpan;
            taskDuration = ts.Days;
            return taskDuration;
        }


        /// <summary>
        /// 根据传入的task，来计算需要生成的重复任务，考虑了今天的日期
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static List<Task> CreateCycTasks(Task task)
        {
            List<Task> cycTasks = new List<Task>();

            var recurringDate = RecurringDate.RecurringJsonToDate(task.RepeatJson);
            if (recurringDate == null)
                return cycTasks;


            //生成任务，最多生成今天、明天和第一个日程三个任务
            //逻辑：因为recurringDate不会为空（已经判断），所以第一个任务肯定要加（最差是日程任务）
            //除非是设置的第一天比今天还早
            //此时，如果第一个日期大于明天，则跳出循环；如果小于明天，就再加一天，再看是否大于明天。
            for (int i = 0; i <= recurringDate.Count; i++)
            {
                if (recurringDate[i].Date < DateTime.Now.Date)
                {
                    continue;
                }
                cycTasks.Add(CloneTaskForRepeat(task, recurringDate[i]));

                if (recurringDate[i].Date > DateTime.Now.AddDays(1).Date)
                {
                    break;
                }
            }

            return cycTasks;
        }

        //完整的用于创建循环任务的cloneedTask
        public static Task CloneTaskForRepeat(Task task, DateTime startDateTime)
        {
            var taskDuration = TaskDuration(task);
            Task t = (Task) task.Clone();
            //这里Clone出来的Task可能是有id的，后面直接拿有id的Task去写入db，不会有问题，会直接把id省略掉，重新自动生成id
            t.StartDateTime = startDateTime;
            //处理没有结束日期的任务
            t.CloseDateTime = task.CloseDateTime != null
                ? (DateTime?) startDateTime.AddDays(taskDuration)
                : null;
            t.DateAttribute = SetDateAttribute(t.StartDateTime, t.DateAttribute, t.ProjectID);
            t.IsComplete = false;
            t.IsDeleted = false;
            t.CompleteDateTime = null;
            t.NextTask_TaskId = null;
            t.PreviousTask_TaskId = null;
            //t.SubTasks = null;
            //t.Comments = null;
            //t.Pomodoros = null;
            return t;
        }

        /// <summary>
        ///根据输入的其他属性，判断DateAttribute应该是什么
        ///业务规则如下：
        ///开始时间：无             项目：无     收集箱
        ///开始时间：今天           项目：任意   今日待办
        ///开始时间：无             项目：有    下一步行动
        ///开始时间：明天           项目：任意   明日待办
        ///开始时间：今天/明天以外   项目：任意   日程
        ///开始时间：无             项目：任意   需要主动设置 将来也许
        ///开始时间：无             项目：任意   需要主动设置 等待
        /// </summary>
        /// <param name="star"></param>
        /// <param name="att"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public static DateAttribute? SetDateAttribute(DateTime? star, DateAttribute? att, int? projectid)
        {
            if (star != null)
            {
                if (Convert.ToDateTime(star).DayOfYear <= DateTime.Now.DayOfYear)
                    return DateAttribute.今日待办;
                else if (Convert.ToDateTime(star).DayOfYear == DateTime.Now.DayOfYear + 1)
                    return DateAttribute.明日待办;
                else
                    return DateAttribute.日程;
            }
            else if (projectid != null)
            {
                return DateAttribute.下一步行动;
            }
            else if (att == DateAttribute.将来也许
                    || att == DateAttribute.等待
                    || att == DateAttribute.收集箱)
                return att;
            else
                return DateAttribute.收集箱;
        }

    }
}