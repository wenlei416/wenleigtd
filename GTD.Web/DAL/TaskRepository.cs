using System;
using System.Collections.Generic;
using System.Linq;
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

        public IQueryable<Task> GetTaskByProjectId(int projectId)
        {
            return GetAll().Where(t => t.ProjectID == projectId);
        }

        //创建task，返回taskid
        public int CreateWithId(Task instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                base.Context.Set<Task>().Add(instance);
                SaveChanges();
            }
            return instance.TaskId;
        }

        public Task GetOriginal(Task task)
        {
            return base.Context.Set<Task>().AsNoTracking().FirstOrDefault(t => t.TaskId == task.TaskId);
        }

    }
}