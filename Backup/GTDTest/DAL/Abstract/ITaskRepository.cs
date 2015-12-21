using System;
using System.Collections.Generic;
using GTDTest.Models;

namespace GTDTest.DAL.Abstract
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

        //Task GetPreviousTasksById(int taskId);
    }
}