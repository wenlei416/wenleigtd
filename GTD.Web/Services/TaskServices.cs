using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Castle.Core.Internal;
using GTD.Util;

namespace GTD.Services
{
    public class TaskServices : ITaskServices
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectServices _projectServices;
        private readonly IContextServices _contextServices;

        public TaskServices(ITaskRepository taskRepository, IProjectServices projectServices, IContextServices contextServices)
        {
            _taskRepository = taskRepository;
            _projectServices = projectServices;
            _contextServices = contextServices;
        }

        //这个是用在单元测试中，是不是间接说明了TaskService不应该依赖其他Service？
        //但需要的其他功能怎么办
        //public TaskServices(ITaskRepository taskRepository)
        //{
        //    _taskRepository = taskRepository;
        //    _projectServices = new ProjectServices();
        //}

        /// <summary>
        /// 根据现有信息，判断task的真实dateattribute
        /// </summary>
        /// <param name="dateAttribute"></param>
        /// <returns></returns>
        public IEnumerable<Task> GetTasksWithRealDa(DateAttribute dateAttribute)
        {
            IEnumerable<Task> tasks;
            var tomorrow = DateTime.Today.AddDays(1);
            switch (dateAttribute.ToString())
            {
                //根据业务规则，只有明日待办或者日程有可能变成今日待办，其他状态要过来，需要主动设置
                case "今日待办":
                    tasks = GetInProgressTasks()
                        .Where(
                            t =>
                                (t.DateAttribute == DateAttribute.日程 || t.DateAttribute == DateAttribute.明日待办) &&
                                t.StartDateTime != null && t.StartDateTime <= DateTime.Today);
                    UpdateDateAttribute(tasks, dateAttribute);

                    break;

                case "明日待办":
                    tasks = GetInProgressTasks() //_taskRepository.GetWorkingTasks()
                        .Where(
                            t =>
                                t.DateAttribute != dateAttribute && t.StartDateTime != null &&
                                t.StartDateTime == tomorrow);
                    UpdateDateAttribute(tasks, dateAttribute);

                    break;

                case "日程":
                    tasks = GetInProgressTasks()
                        .Where(
                            t =>
                                t.DateAttribute != dateAttribute && t.StartDateTime != null &&
                                t.StartDateTime > tomorrow);
                    UpdateDateAttribute(tasks, dateAttribute);

                    break;
            }
            return GetInProgressTasks().Where(t => t.DateAttribute == dateAttribute);
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
                //这里的写法只适合于新建任务的时候创建有循环任务的情况，没有考虑重复创建等问题
                var cycTasks = TaskUtil.CreateCycTasks(task);
                if (cycTasks == null) return;

                //插入第一个，获得id
                var id = _taskRepository.CreateWithId(cycTasks[0]);

                //修改刚插入的task，把id加入json
                if (!cycTasks[0].RepeatJson.Contains("\"id\":"))
                {
                    cycTasks[0].RepeatJson = cycTasks[0].RepeatJson.Replace("}", ",\"id\":\"" + id + "\"}");
                }
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
            //todo 完全没测试，需要进一步抽象后测试，屏蔽掉所有数据库的操作
            var originalTask = _taskRepository.GetOriginal(task);
            var oldRepeatJson = originalTask.RepeatJson;
            //如果RepeatJson一样，说明循环任务没有变化，不需要调整循环任务
            //但可能会需要调整每个循环任务的属性
            if (oldRepeatJson == task.RepeatJson)
            {
                //repeatJson和原来一样而且不为空；修改的属性也在所有repeatTask必须一样的范围内。
                //此时只需要更新字段，循环情况不用调整
                if (!oldRepeatJson.IsNullOrEmpty() && TaskUtil.ModifiedPropertiesInList(originalTask, task))//这里还要增加个条件判断修改属性在不在范围内
                {
                    //把循环任务都取出来
                    var repeatTasks = GetRepeatTasks(oldRepeatJson);
                    //更新循环任务的字段
                    //输入：tasklist，输出：修改后的tasklist，无需泛型支持（因为是要排除到一些特定的字段的，比如id，stasttime，closetime，comment
                    var toUpdateTasks = TaskUtil.UpdateRepeatTasksProperties(repeatTasks.AsQueryable(), task);
                    //然后循环update
                    foreach (var t in toUpdateTasks)
                    {
                        t.DateAttribute = TaskUtil.SetDateAttribute(t.StartDateTime, t.DateAttribute, t.ProjectID);
                        _taskRepository.Update(t);
                    }
                }
                else
                {
                    //repeatJson没有修改，而且为空的时候
                    task.DateAttribute = TaskUtil.SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
                    _taskRepository.Update(task);
                }
            }
            //RepeatJson发生了变化，需要调整原先生成的任务
            else
            {
                //可能1：新RepeatJson为空
                if (task.RepeatJson.IsNullOrEmpty())
                {
                    //删除所有循环任务，保留当前打开的任务
                    var repeatTasks = GetRepeatTasks(oldRepeatJson) as IList<Task> ?? GetRepeatTasks(oldRepeatJson).ToList();
                    repeatTasks.Remove(task);
                    foreach (var t in repeatTasks)
                    {
                        DeleteTask(t.TaskId);
                    }
                }
                //可能2：新RepeatJson不为空、老RepeatJson为空
                if (!task.RepeatJson.IsNullOrEmpty() && oldRepeatJson.IsNullOrEmpty())
                {
                    //增加循环任务
                    var repeatTasks = TaskUtil.CreateCycTasks(task);
                    //避免出现循环任务创建不出来，现在的任务又被删除掉了的情况
                    //如果循环任务为空，就什么都不做。
                    //如果循环任务不为空，就把原来的任务删掉，不确定是否合理
                    //得看一下页面的行为会不会有问题，不会有问题，update以后返回list页面
                    if (!repeatTasks.IsNullOrEmpty())
                    {
                        foreach (var t in repeatTasks)
                        {
                            AddTask(t);
                        }
                        DeleteTask(task.TaskId);
                    }
                }
                //可能3：新RepeatJson不为空、老RepeatJson不为空
                if (!task.RepeatJson.IsNullOrEmpty() && !oldRepeatJson.IsNullOrEmpty())
                {
                    //把老的循环任务全部删掉，把新的循环任务全部加上，最简单
                    //得看一下页面的行为会不会有问题，不会有问题，update以后返回list页面
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

        /// <summary>
        /// 获取已经完成的任务，不包括被删除的
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Task> GetCompletedTasks()
        {
            return _taskRepository.GetAll().Include(t => t.Pro).Where(t => t.IsComplete && t.IsDeleted == false);
        }

        /// <summary>
        /// 获取正在处理中的任务（没有完成也没有被删除的）
        /// </summary>
        /// <returns></returns>
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

        //[Log]
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
        /// </summary>
        /// 业务规则
        /// 每行一个task
        /// #引导项目，如果项目不存在，不新增。
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

        public void AddTaskFromFilter(Task task)
        {
            task.DateAttribute = TaskUtil.SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
            _taskRepository.Create(task);
        }

        /// <summary>
        /// 获取所有循环任务
        /// </summary>
        /// <param name="repeatJson"></param>
        /// <returns></returns>
        private IEnumerable<Task> GetRepeatTasks(string repeatJson)
        {
            return GetInProgressTasks().Where(t => t.RepeatJson == repeatJson);
        }


        /// <summary>
        /// 验证通过一行文本创建task，解析出来的文本是否正确，正确就返回一个task，错误返回null
        /// </summary>
        /// 正确与否的条件就是是否有headline，有headline就是正确的，不然就是错误的
        /// 项目如果没有就新建，context如果没有就忽略
        /// <param name="taskDictionary"></param>
        /// <returns></returns>
        public Task ValidateTaskDicIsCorrect(Dictionary<string, string> taskDictionary)
        {
            Task result = new Task()
            {
                IsComplete = false,
                IsDeleted = false,
                StartDateTime = DateTime.Today,
                CloseDateTime = DateTime.Today,
                Priority = Priority.中
            };

            if (taskDictionary.IsNullOrEmpty()) return null;

            if (taskDictionary.ContainsKey("taskName"))
            {
                result.Headline = taskDictionary["taskName"];
            }
            else
            {
                return null;
            }

            if (taskDictionary.ContainsKey("taskDescription"))
            {
                result.Description = taskDictionary["taskDescription"];
            }

            if (taskDictionary.ContainsKey("contextName"))
            {
                var contextId = _contextServices.IsExistByName(taskDictionary["contextName"]);
                if (contextId != null)
                {
                    result.ContextID = contextId;
                }
            }

            if (taskDictionary.ContainsKey("projectName"))
            {
                var projectId = _projectServices.IsExistByName(taskDictionary["projectName"]);
                if (projectId != null)
                {
                    result.ProjectID = projectId;
                }
                else
                {
                    var pro = new Project { ProjectName = taskDictionary["projectName"] };
                    result.ProjectID = _projectServices.CreateProjectReturnId(pro);
                }
            }

            return result;
        }

        /// <summary>
        /// modify的过程中，获取修改前的task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public Task GetOriginalTask(Task task)
        {
            return _taskRepository.GetOriginal(task);
        }

        /// <summary>
        /// 修改task的开始日期、结束日期、完成日期时，生成一条文本
        /// 用于插入commit
        /// </summary>
        /// 记录内容
        /// **2011-01-01 修改taskid：XX 开始日期 从2011-01-01 调整到 2011-01-02 修改开始日期时记录
        /// **2011-01-01 修改taskid：XX 开始日期 从 调整到 2011-01-02 修改开始日期时记录
        /// **2011-01-01 修改taskid：XX 结束日期 从2011-01-01 调整到 2011-01-02 修改完成日期时记录
        /// **2011-01-01 修改taskid：XX 结束日期 从 调整到 2011-01-02 修改完成日期时记录
        /// **2011-01-01 修改taskid：XX 计划完成： 从2011-01-01 实际完成: 2011-01-02 
        /// <param name="action"></param>
        /// <param name="task"></param>
        /// <param name="orginalTask"></param>
        /// <returns></returns>
        public string CreteCommentText(string action, Task task, Task orginalTask = null)
        {
            string result = "";

            switch (action.ToLower())
            {
                case "delete":
                    result = "** " + DateTime.Now.ToString("yyyy/MM/dd hh:mm", CultureInfo.InvariantCulture) +
                             " 删除taskId: " + task.TaskId;

                    break;

                case "modify":

                    break;
                case "complete":
                    var close = task.CloseDateTime?.Date.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) ?? "无";
                    result = "** " + DateTime.Now.ToString("yyyy/MM/dd hh:mm", CultureInfo.InvariantCulture) +
                             " 修改taskId: " + task.TaskId +
                             " 计划完成：" + close +
                             " 实际完成: " + DateTime.Today.Date.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                    break;
            }

            return result;
        }

    }
}