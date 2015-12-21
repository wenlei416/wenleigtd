using System.Collections.Generic;
using GTD.Models;

namespace GTD.ViewModels
{
    public class TaskDetailVM
    {
        public Task Task { get; set; }

        public IEnumerable<SubTask> CompletedSubTasks { get; set; }

        public IEnumerable<SubTask> InprogressSubTasks { get; set; }
    }
}