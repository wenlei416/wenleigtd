using System.Collections.Generic;
using GTD.DAL.Abstract;
using GTD.Models;

namespace GTD.DAL
{
    public class TaskRepository : GenericRepository<Task>, ITaskRepository
    {

        public Task GetTaskById(int? taskId)
        {
            return this.Get(t => t.TaskId == taskId);
        }

        public void DeleteTask(int taskId)
        {
            Task task = this.Get(t => t.TaskId == taskId);
            if (task != null) task.IsDeleted = !task.IsDeleted;
            Update(task);
        }

        public void BatchUpdateTask(IEnumerable<Task> tasks)
        {
            Updates(tasks);
        }
        
    }
}