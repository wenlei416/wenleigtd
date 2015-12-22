using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GTD.DAL.Abstract;
using GTD.Models;

namespace GTD.DAL
{
    public class TaskRepository:ITaskRepository,IDisposable
    {
        private readonly GTDContext _context;

        private bool _disposed = false;

        public TaskRepository(GTDContext context)
        {
            this._context = context;
        }

        public void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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
            return _context.Tasks.ToList();
        }

        public IEnumerable<Task> GetWorkingTasks()
        {
            return _context.Tasks.Where(i => i.IsComplete == false && i.IsDeleted == false);
        }

        public Task GetTaskById(int? taskId)
        {
            return _context.Tasks.Find(taskId);
        }

        public void InsertTask(Task task)
        {
            _context.Tasks.Add(task);
        }

        public void DeleteTask(int taskId)
        {
            Task task = _context.Tasks.Find(taskId);
            if (task != null) task.IsDeleted = !task.IsDeleted;
        }

        public void UpdateTask(Task task)
        {
            _context.Entry(task).State=EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public GTDContext GetContext()
        {
            return _context;
        }

    }
}