using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GTDTest.DAL.Abstract;
using GTDTest.Models;

namespace GTDTest.DAL
{
    public class TaskRepository:ITaskRepository,IDisposable
    {
        private GTDContext context;

        private bool _disposed = false;

        public TaskRepository(GTDContext context)
        {
            this.context = context;
        }

        public void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public IEnumerable<Task> GetTasks()
        {
            return context.Tasks.ToList();
        }

        public Task GetTaskById(int? taskId)
        {
            return context.Tasks.Find(taskId);
        }

        public void InsertTask(Task task)
        {
            context.Tasks.Add(task);
        }

        public void DeleteTask(int taskId)
        {
            Task task = context.Tasks.Find(taskId);
            if (task != null) task.IsDeleted = !task.IsDeleted;
        }

        public void UpdateTask(Task task)
        {
            context.Entry(task).State=EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public GTDContext GetContext()
        {
            return context;
        }
    }
}