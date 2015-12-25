using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GTD.DAL.Abstract;
using GTD.Models;

namespace GTD.DAL
{
    public class TaskRepository : GenericRepository<Task>, ITaskRepository
    {
        private readonly GTDContext _context;

        public TaskRepository()
        {
            this._context = new GTDContext();
        }
        public TaskRepository(GTDContext context)
        {
            this._context = context;
        }

        public Task GetTaskById(int? taskId)
        {
            return _context.Tasks.Find(taskId);
        }

        public void DeleteTask(int taskId)
        {
            Task task = _context.Tasks.Find(taskId);
            if (task != null) task.IsDeleted = !task.IsDeleted;
            Update(task);
        }

        public void UpdateTask(Task task)
        {
            _context.Entry(task).State = EntityState.Modified;
        }

        public GTDContext GetContext()
        {
            return _context;
        }

        public void BatchUpdateTask(IEnumerable<Task> tasks)
        {
            foreach (var t in tasks)
            {
                _context.Entry(t).State = EntityState.Modified;

            }
            SaveChanges();
        }
    }
}