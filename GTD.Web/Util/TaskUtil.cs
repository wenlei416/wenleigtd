using System;
using GTD.DAL;
using GTD.Models;

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

        public static string NextTask_Headline(this Task task)
        {
            
                if (task.NextTask_TaskId != null)
                {
                    var db = new GTDContext();
                    return db.Tasks.Find(task.NextTask_TaskId) != null ? db.Tasks.Find(task.NextTask_TaskId).Headline : string.Empty;
                }
                return null;
        }

        public static string PreviousTask_Headline(this Task task)
        {
            
                if (task.PreviousTask_TaskId != null)
                {
                    var db = new GTDContext();
                    return db.Tasks.Find(task.PreviousTask_TaskId) != null ? db.Tasks.Find(task.PreviousTask_TaskId).Headline : string.Empty;
                }
                return null;
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


    }


}