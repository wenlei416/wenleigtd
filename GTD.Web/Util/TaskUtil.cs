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

    }
}