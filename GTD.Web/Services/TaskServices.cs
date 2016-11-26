﻿using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.WebPages;
using Castle.Core.Internal;
using GTD.Util;

namespace GTD.Services
{
    public class TaskServices : ITaskServices
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectServices _projectServices;

        public TaskServices(ITaskRepository taskRepository, IProjectServices projectServices)
        {
            _taskRepository = taskRepository;
            _projectServices = projectServices;
        }

        //这个是用在单元测试中，是不是间接说明了TaskService不应该依赖其他Service？
        //但需要的其他功能怎么办
        //public TaskServices(ITaskRepository taskRepository)
        //{
        //    _taskRepository = taskRepository;
        //    _projectServices = new ProjectServices();
        //}

        public IEnumerable<Task> GetTasksWithRealDa(DateAttribute dateAttribute)
        {
            IEnumerable<Task> tasks;
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
            //没有重复任务的场景
            if (task.RepeatJson.IsNullOrEmpty())
            {
                task.DateAttribute = TaskUtil.SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
                _taskRepository.Create(task);
            }
            //有重复任务的场景
            else
            {
                var cycTasks = TaskUtil.CreateCycTasks(task);
                if (cycTasks == null) return;

                //插入第一个，获得id
                var id = _taskRepository.CreateWithId(cycTasks[0]);

                //修改刚插入的task，把id加入json
                cycTasks[0].RepeatJson = cycTasks[0].RepeatJson.Replace("}", ",\"id\":\"" + id + "\"}");
                cycTasks[0].TaskId = id;
                _taskRepository.Update(cycTasks[0]);

                //修改后面每一个的json，并插入数据库
                for (var i = 1; i < cycTasks.Count; i++)
                {
                    cycTasks[i].RepeatJson = cycTasks[0].RepeatJson;
                    _taskRepository.Create(cycTasks[i]);
                }
            }
        }

        /// <summary>
        /// 更新task，包括重复任务
        /// </summary>
        /// <param name="task"></param>
        public void UpdateTask(Task task)
        {
            var oldRepeatJson = GetTaskById(task.TaskId).RepeatJson;
            //如果RepeatJson一样，说明没变化，不需要调整循环任务，但可能会需要调整每个循环任务的属性
            //但需要更新循环任务
            if (oldRepeatJson == task.RepeatJson)
            {
                if (!oldRepeatJson.IsNullOrEmpty() && TaskUtil.ModifiedPropertiesInList(GetTaskById(task.TaskId), task))//这里还要增加个条件判断修改属性在不在范围内
                {
                    //更新循环任务，包括需要更新字段的
                    //输入：tasklist，输出：修改后的tasklist，无需泛型支持（因为是要排除到一些特定的字段的，比如id，stasttime，closetime，comment
                    //然后循环update
                    var repeatTasks = GetRepeatTasks(oldRepeatJson);
                    var toUpdateTasks = TaskUtil.UpdateRepeatTasksProperties(repeatTasks.AsQueryable(), task);
                    foreach (var t in toUpdateTasks)
                    {
                        t.DateAttribute = TaskUtil.SetDateAttribute(t.StartDateTime, t.DateAttribute, t.ProjectID);
                        _taskRepository.Update(t);
                    }
                }
                else
                {
                    task.DateAttribute = TaskUtil.SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
                    _taskRepository.Update(task);
                }
            }
            else
            {
                if (task.RepeatJson.IsNullOrEmpty())
                {
                    //删除所有循环任务，保留当前打开的任务
                    var repeatTasks = GetRepeatTasks(oldRepeatJson);
                    foreach (var t in repeatTasks)
                    {
                        if (t.TaskId != task.TaskId)
                            DeleteTask(t.TaskId);
                    }
                }
                else if (oldRepeatJson.IsNullOrEmpty())
                {
                    //增加循环任务，把当前选择的任务改成符合循环规则的
                    var repeatTasks = TaskUtil.CreateCycTasks(task);
                    //避免出现循环任务创建不出来，现在的任务又被删除掉了的情况
                    if (repeatTasks.IsNullOrEmpty())
                    {
                        foreach (var t in repeatTasks)
                        {
                            AddTask(t);
                        }
                        DeleteTask(task.TaskId);
                    }
                }
                else
                {
                    var repeatTasks = TaskUtil.CreateCycTasks(task);
                    var oldrepeatTasks = GetRepeatTasks(oldRepeatJson);
                    if (repeatTasks.IsNullOrEmpty())
                    {
                        foreach (var t in repeatTasks)
                        {
                            AddTask(t);
                        }
                        foreach (var t in oldrepeatTasks)
                        {
                            DeleteTask(t.TaskId);
                        }
                    }
                }
            }
        }

        //获取已经完成的任务，不包括被删除的
        public IEnumerable<Task> GetCompletedTasks()
        {
            return _taskRepository.GetAll().Include(t => t.Pro).Where(t => t.IsComplete && t.IsDeleted == false);
        }

        //获取正在处理中的任务（没有完成也没有被删除的）
        public IEnumerable<Task> GetInProgressTasks()
        {
            return _taskRepository.GetAll().Include(t => t.Pro).Where(t => t.IsComplete == false && t.IsDeleted == false);
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

        [Log]
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

        public void CompleteTask(Task task)
        {
            task.IsComplete = !task.IsComplete;
            task.CompleteDateTime = DateTime.Today;
            //如果关联的项目已经删除，断开关系
            if (task.ProjectID != null)
            {
                var p = _projectServices.GetProjectById((int)task.ProjectID);
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
            _taskRepository.BatchUpdateTask(enumerablepreviousTasks);
            //BatchUpdateTask(enumerablepreviousTasks);
        }

        /// <summary>
        /// 将批量输入的文本，拆分成多个task
        /// 业务规则
        /// 每行一个task
        /// #引导项目，如果项目不存在，不新增。
        /// </summary>
        /// <param name="taskText"></param>
        /// <returns></returns>
        public IEnumerable<Task> SplitTextToTasks(string taskText)
        {
            //拆分成若干行
            List<Task> tasklist = new List<Task>();
            IEnumerable<string> taskTexts = taskText.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var t in taskTexts)
            {
                var headline = TaskUtil.GetTaskNameFromText(t);
                var project = TaskUtil.GetProjectNameFromText(t);
                if (headline != null)
                {
                    tasklist.Add(new Task { Headline = headline, ProjectID = _projectServices.IsExistByName(project) });
                }
            }

            return tasklist;
        }

        public Task GetNextTaskByTaskId(int taskId)
        {
            var task = _taskRepository.GetTaskById(taskId);
            return task.NextTask_TaskId != null ? _taskRepository.GetTaskById(task.NextTask_TaskId) : null;
        }

        public Task GetPreviousTaskByTaskId(int taskId)
        {
            var task = _taskRepository.GetTaskById(taskId);
            return task.PreviousTask_TaskId != null ? _taskRepository.GetTaskById(task.PreviousTask_TaskId) : null;
        }

        //获取循环任务
        private IEnumerable<Task> GetRepeatTasks(string repeatJson)
        {
            return GetInProgressTasks().Where(t => t.RepeatJson == repeatJson);
        }
    }
}