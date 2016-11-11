using System.Collections.Generic;
using GTD.Models;

namespace GTD.ViewModels
{
    public class TaskDetailVM
    {
        public Task Task { get; set; }

        public IEnumerable<SubTask> CompletedSubTasks { get; set; }

        public IEnumerable<SubTask> InprogressSubTasks { get; set; }

        public Task NextTask { get; set; }

        public Task PreviousTask { get; set; }

    }
}