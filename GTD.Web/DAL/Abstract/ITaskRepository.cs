using System.Collections.Generic;
using GTD.Models;

namespace GTD.DAL.Abstract
{
    public interface ITaskRepository:IRepository<Task>
    {
        Task GetTaskById(int? taskId);
        void DeleteTask(int taskId);
        void BatchUpdateTask(IEnumerable<Task> tasks);

        //IEnumerable<Task> GetWorkingTasks();
        //Task GetPreviousTasksById(int taskId);
    }
}