using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GTDTest.Models;

namespace GTDTest.ViewModels
{
    public class TaskDetail
    {
        public Task Task { get; set; }

        public IEnumerable<SubTask> CompletedSubTasks { get; set; }

        public IEnumerable<SubTask> InprogressSubTasks { get; set; }
    }
}