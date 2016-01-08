using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;

namespace GTD.Services
{
    public class TaskServices : ITaskServices
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectServices _projectServices;

        public TaskServices()
        {
            _projectServices = new ProjectServices();
            _taskRepository = new TaskRepository();
        }

        public TaskServices(ITaskRepository taskRepository, IProjectServices projectServices)
        {
            _taskRepository = taskRepository;
            _projectServices = projectServices;
        }

        public IEnumerable<Task> GetTasksWithRealDa(DateAttribute dateAttribute)
        {
            IEnumerable<Task> tasks = null;
            var tomorrow = DateTime.Today.AddDays(1);
            switch (dateAttribute.ToString())
            {
                //根据业务规则，只有明日待办或者日程有可能变成今日待办，其他状态要过来，需要主动设置
                case "今日待办":
                    tasks = GetInProgressTasks()//_taskRepository.GetWorkingTasks()
                        .Where(
                            t =>
                                (t.DateAttribute == DateAttribute.日程 || t.DateAttribute == DateAttribute.明日待办)
                                &&
                                (t.StartDateTime != null && t.StartDateTime <= DateTime.Today));
                    UpdateDateAttribute(tasks, dateAttribute);

                    break;
                case "明日待办":
                    tasks = GetInProgressTasks()//_taskRepository.GetWorkingTasks()
                        .Where(
                            t =>
                                (t.DateAttribute != dateAttribute && t.StartDateTime != null && t.StartDateTime == tomorrow));
                    UpdateDateAttribute(tasks, dateAttribute);

                    break;
                case "日程":
                    tasks = GetInProgressTasks()//_taskRepository.GetWorkingTasks()
                        .Where(
                            t =>
                                (t.DateAttribute != dateAttribute && t.StartDateTime != null && t.StartDateTime > tomorrow));
                    UpdateDateAttribute(tasks, dateAttribute);

                    break;
            }
            return GetInProgressTasks().Where(t => t.DateAttribute == dateAttribute);//_taskRepository.GetWorkingTasks().Where(t=>t.DateAttribute==dateAttribute);
        }

        public void AddTask(Task task)
        {
            task.DateAttribute = SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
            _taskRepository.Create(task);
        }

        public void UpdateTask(Task task)
        {
            task.DateAttribute = SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
            _taskRepository.Update(task);
        }

        public IEnumerable<Task> GetCompletedTasks()
        {
            return _taskRepository.GetAll().Include(t=>t.Pro).Where(t => t.IsComplete == true && t.IsDeleted == false);
        }

        public IEnumerable<Task> GetInProgressTasks()
        {
            return _taskRepository.GetAll().Include(t=>t.Pro).Where(t => t.IsComplete == false && t.IsDeleted == false);
        }

        /// <summary>
        ///根据输入的其他属性，判断DateAttribute应该是什么
        ///业务规则如下：
        ///开始时间：无             项目：无     收集箱
        ///开始时间：今天           项目：任意   今日待办
        ///开始时间：无             项目：有    下一步行动
        ///开始时间：明天           项目：任意   明日待办
        ///开始时间：今天/明天以外   项目：任意   日程
        ///开始时间：无             项目：任意   需要主动设置 将来也许
        ///开始时间：无             项目：任意   需要主动设置 等待
        /// </summary>
        /// <param name="star"></param>
        /// <param name="att"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public static DateAttribute? SetDateAttribute(DateTime? star, DateAttribute? att, int? projectid)
        {
            if (star != null)
            {
                if (Convert.ToDateTime(star).DayOfYear <= DateTime.Now.DayOfYear)
                    return DateAttribute.今日待办;
                else if (Convert.ToDateTime(star).DayOfYear == DateTime.Now.DayOfYear + 1)
                    return DateAttribute.明日待办;
                else
                    return DateAttribute.日程;
            }
            else if (projectid != null)
            {
                return DateAttribute.下一步行动;
            }
            else if (att == DateAttribute.将来也许
                    || att == DateAttribute.等待
                    || att == DateAttribute.收集箱)
                return att;
            else
                return DateAttribute.收集箱;
        }

        /// <summary>
        /// 批量更新DateAttribute
        /// </summary>
        /// <param name="tasks">需要更新的Tasks</param>
        /// <param name="da">更新为这个DataAttribute</param>
        private void UpdateDateAttribute(IEnumerable<Task> tasks, DateAttribute da)
        {
            foreach (var task in tasks)
            {
                task.DateAttribute = da;
            }
            _taskRepository.BatchUpdateTask(tasks);
        }

        public Task GetTaskById(int? taskId)
        {
            return _taskRepository.GetTaskById(taskId);
        }

        public void DeleteTask(int taskId)
        {
            _taskRepository.DeleteTask(taskId);
        }

        public IEnumerable<Task> GetAll()
        {
            return _taskRepository.GetAll();
        }

        public void BatchUpdateTask(IEnumerable<Task> tasks)
        {
            _taskRepository.BatchUpdateTask(tasks);
        }

        //public GTDContext GetContext()
        //{
        //    return _taskRepository.GetContext();
        //}

        public void CompleteTask(Task task)
        {
            task.IsComplete = !task.IsComplete;
            task.CompleteDateTime = DateTime.Today;
            //如果关联的项目已经删除，断开关系
            if (task.ProjectID != null)
            {
                var p = _projectServices.GetProjectById((int) task.ProjectID);
                if (p.IsDeleted)
                {
                    task.ProjectID = null;
                }
            }
            UpdateTask(task);

            //如果后续任务没有明确的日程，设为今日待办
            if (task.NextTask_TaskId != null)
            {
                var nextTask = GetAll().Single(t => t.TaskId == task.NextTask_TaskId);
                if (nextTask != null && nextTask.StartDateTime == null)
                {
                    nextTask.StartDateTime = DateTime.Today;
                    nextTask.DateAttribute = DateAttribute.今日待办;
                    UpdateTask(nextTask);
                }
            }
            //前置任务为这个任务的其他任务，如果没有明确的日程，设为今日待办
            var previousTasks = GetAll().Where(t => t.PreviousTask_TaskId == task.TaskId);
            var enumerablepreviousTasks = previousTasks as Task[] ?? previousTasks.ToArray();
            foreach (var t in enumerablepreviousTasks)
            {
                if (t.StartDateTime == null)
                {
                    t.StartDateTime = DateTime.Today;
                    t.DateAttribute = DateAttribute.今日待办;
                }
            }
            BatchUpdateTask(enumerablepreviousTasks);
        }

    }
}