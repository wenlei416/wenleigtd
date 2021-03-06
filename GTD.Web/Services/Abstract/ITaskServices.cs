﻿using System.Collections.Generic;
using GTD.DAL;
using GTD.Models;

namespace GTD.Services.Abstract
{
    public interface ITaskServices
    {
        /// <summary>
        /// 录入时设置的dateAttribute可能和实际的不一致
        /// 比如设置为“明日待办”，第二天就需要变为“今日待办”
        /// 需要更新各个任务的dateAttribute，然后再返回需要的tasks
        /// </summary>
        /// <param name="dateAttribute"></param>
        /// <returns></returns>
        IEnumerable<Task> GetTasksWithRealDa(DateAttribute dateAttribute);
        void AddTask(Task task);
        void UpdateTask(Task task);

        /// <summary>
        /// 获取所有已完成的任务（不包括已经删除的）
        /// </summary>
        /// <returns></returns>
        IEnumerable<Task> GetCompletedTasks();

        /// <summary>
        /// 获取所有未完成的任务（不包括已经删除的）
        /// </summary>
        /// <returns></returns>
        IEnumerable<Task> GetInProgressTasks();

        Task GetTaskById(int? taskId);

        void DeleteTask(int taskId);
        IEnumerable<Task> GetAll();

        //void BatchUpdateTask(IEnumerable<Task> tasks);

        //GTDContext GetContext();

        void CompleteTask(Task task);

        IEnumerable<Task> SplitTextToTasks(string taskText);

        Task GetNextTaskByTaskId(int taskId);
        
        Task GetPreviousTaskByTaskId(int taskId);

        void AddTaskFromFilter(Task task);

        Task ValidateTaskDicIsCorrect(Dictionary<string, string> taskDictionary);

        string CreteCommentText(string action, Task task, Task orginalTask = null);
        Task GetOriginalTask(Task task);

        void BreakRelationTaskandProject(IEnumerable<Task> tasks);
    }
}
