using System;
using System.Collections.Generic;
using System.Linq;
using GTDTest.Models;
using Microsoft.Ajax.Utilities;

namespace GTDTest.ViewModels
{
    public class Tasklist
    {
        public Tasklist(IEnumerable<Task> tasks, string sortOrder)
        {
            Tasks = tasks;
            SortOrder = sortOrder;
            Addtl();
        }

        public Dictionary<string, IEnumerable<Task>> tl { get; set; }
        public IEnumerable<Task> Tasks { get; set; }

        public IEnumerable<DateAttribute?> DateAttributeList
        {
            get
            {
                if (Tasks != null)
                {
                    return
                        Tasks.DistinctBy(t => t.DateAttribute)
                            .OrderByDescending(t => t.Priority.HasValue)
                            .ThenBy(t => t.Priority)
                            .Select(t => t.DateAttribute);
                }
                else
                {
                    return null;
                }
            }
        }

        public IEnumerable<Priority?> PrioritiesList
        {
            get
            {
                if (Tasks != null)
                {
                    return
                        Tasks.DistinctBy(t => t.Priority)
                            .OrderByDescending(t => t.Priority.HasValue)
                            .ThenBy(t => t.Priority)
                            .Select(t => t.Priority);
                }
                else
                {
                    return null;
                }
            }
        }

        public string SortOrder { get; set; }

        public void Addtl()
        {
            if (Tasks == null) return;
            tl = new Dictionary<string, IEnumerable<Task>>();
            switch (SortOrder)
            {
                case "priority":
                    {
                        var sortList = Tasks.DistinctBy(t => t.Priority)
                            .OrderByDescending(t => t.Priority.HasValue)
                            .ThenBy(t => t.Priority)
                            .Select(t => t.Priority);

                        foreach (var s in sortList)
                        {
                            Priority? s1 = s;
                            var t = Tasks.Where(i => i.Priority == s1);
                            if (s1==null)
                            {
                                tl.Add(Priority.无.ToString(), t);
                            }
                            else
                            {
                                tl.Add(Convert.ToString(s), t);
                            }
                        }
                        break;
                    }
                case "project":
                    {
                        var sortList = Tasks.DistinctBy(t => t.ProjectID)
                              .OrderByDescending(t => t.ProjectID.HasValue)
                              .ThenBy(t => t.ProjectID)
                              .Select(t => t.Pro);
                        foreach (var s in sortList)
                        {
                            var t = Tasks.Where(i => i.Pro == s);
                            if (s == null)
                            {
                                tl.Add("无项目", t);
                            }
                            else
                            {
                                tl.Add(s.ProjectName, t);
                            }
                        }
                        break;
                    }

                case "startat":
                    {
                        var sortList = Tasks.DistinctBy(t => t.StartDateTime)
                        .OrderByDescending(t => t.StartDateTime.HasValue)
                        .ThenBy(t => t.StartDateTime)
                        .Select(t => t.StartDateTime);
                        foreach (var s in sortList)
                        {
                            var t = Tasks.Where(i => i.StartDateTime == s);
                            if (s == null)
                            {
                                tl.Add("无开始日期", t);
                            }
                            else
                            {
                                tl.Add(s.Value.ToShortDateString(), t);
                            }
                        }
                        break;
                    }

                case "closeat": {
                    var sortList = Tasks.DistinctBy(t => t.CloseDateTime)
                        .OrderByDescending(t => t.CloseDateTime.HasValue)
                        .ThenBy(t => t.CloseDateTime)
                        .Select(t => t.CloseDateTime);
                    foreach (var s in sortList)
                    {
                        var t = Tasks.Where(i => i.CloseDateTime == s);
                        if (s == null)
                        {
                            tl.Add("无结束日期", t);
                        }
                        else
                        {
                            tl.Add(s.Value.ToShortDateString(), t);
                        }
                    }
                    break;
                }
                case "context": {
                    var sortList = Tasks.DistinctBy(t => t.ContextID)
                        .OrderByDescending(t => t.ContextID.HasValue)
                        .ThenBy(t => t.ContextID)
                        .Select(t => t.Context);
                    foreach (var s in sortList)
                    {
                        var t = Tasks.Where(i => i.Context == s);
                        if (s == null)
                        {
                            tl.Add("无场景", t);
                        }
                        else
                        {
                            tl.Add(s.ContextName, t);
                        }
                    }
                    break;
                }
                case "CompleteDateTime":
                {
                    var sortList = Tasks.DistinctBy(t => t.CompleteDateTime)
                        .OrderByDescending(t => t.CompleteDateTime.HasValue)
                        .ThenByDescending(t => t.CompleteDateTime)
                        .Select(t => t.CompleteDateTime);
                    foreach (var s in sortList)
                    {
                        var t = Tasks.Where(i => i.CompleteDateTime == s);
                        if (s == null)
                        {
                            tl.Add("无完成日期", t);
                        }
                        else
                        {
                            tl.Add(s.Value.ToShortDateString(), t);
                        }
                    }
                    break;
                }
                default:
                    break;
            }
        }
    }
}