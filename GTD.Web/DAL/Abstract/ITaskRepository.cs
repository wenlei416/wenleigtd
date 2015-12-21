using System;
using System.Collections.Generic;
using GTD.Models;

namespace GTD.DAL.Abstract
{
    public interface ITaskRepository:IDisposable
    {
        IEnumerable<Task> GetTasks();
        Task GetTaskById(int? taskId);
        void InsertTask(Task task);
        void DeleteTask(int taskId);
        void UpdateTask(Task task);
        void Save();
        GTDContext GetContext();
        IEnumerable<Task> GetWorkingTasks();

        //Task GetPreviousTasksById(int taskId);
    }
}