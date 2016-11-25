using GTD.DAL.Abstract;
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
            LogHelper.WriteLog(task.Headline);
            //没有重复任务的场景
            if (task.RepeatJson.IsNullOrEmpty())
            {
                task.DateAttribute = SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
                _taskRepository.Create(task);
            }
            //有重复任务的场景
            else
            {
                var cycTasks = CreateCycTasks(task);
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
        /// 根据传入的task，来计算需要生成的重复任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private List<Task> CreateCycTasks(Task task)
        {
            List<Task> cycTasks = new List<Task>();

            var recurringDate = RecurringDate.RecurringJsonToDate(task.RepeatJson);
            if (recurringDate == null)
                return cycTasks;

            //获取任务的周期
            var taskDuration = TaskDuration(task);

            //生成任务，最多生成今天、明天和第一个日程三个任务
            //逻辑：因为recurringDate不会为空（已经判断），所以第一个任务肯定要加（最差是日程任务）
            //除非是设置的第一天比今天还早
            //此时，如果第一个日期大于明天，则跳出循环；如果小于明天，就再加一天，再看是否大于明天。
            for (int i = 0; i <= recurringDate.Count; i++)
            {
                if (recurringDate[i].Date < DateTime.Now.Date)
                {
                    continue;
                }
                //深度克隆对象，可以考虑改写到model的clone接口中
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    BinaryFormatter bf = new BinaryFormatter();
                //    //序列化成流
                //    bf.Serialize(ms, task);
                //    ms.Seek(0, SeekOrigin.Begin);
                //    //反序列化成对象
                //    Task t = (Task)bf.Deserialize(ms);
                //    //克隆出来的对象要改任务开始和结束日期
                //    t.StartDateTime = recurringDate[i];
                //    //处理没有结束日期的任务
                //    t.CloseDateTime = task.CloseDateTime != null
                //        ? (DateTime?)recurringDate[i].AddDays(taskDuration)
                //        : null;
                //    t.DateAttribute = SetDateAttribute(t.StartDateTime, t.DateAttribute, t.ProjectID);

                //    cycTasks.Add(t);
                //    ms.Close();
                //}
                Task t = (Task)task.Clone();
                t.StartDateTime = recurringDate[i];
                //处理没有结束日期的任务
                t.CloseDateTime = task.CloseDateTime != null
                    ? (DateTime?)recurringDate[i].AddDays(taskDuration)
                    : null;
                t.DateAttribute = SetDateAttribute(t.StartDateTime, t.DateAttribute, t.ProjectID);

                cycTasks.Add(t);

                if (recurringDate[i].Date > DateTime.Now.AddDays(1).Date)
                {
                    break;
                }
            }

            return cycTasks;
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
                        t.DateAttribute = SetDateAttribute(t.StartDateTime, t.DateAttribute, t.ProjectID);
                        _taskRepository.Update(t);
                    }
                }
                else
                {
                    task.DateAttribute = SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
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
                    var repeatTasks = CreateCycTasks(task);
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
                    var repeatTasks = CreateCycTasks(task);
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
        private static DateAttribute? SetDateAttribute(DateTime? star, DateAttribute? att, int? projectid)
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

        //计算任务的周期
        private int TaskDuration(Task task)
        {
            int taskDuration = 0;
            var timeSpan = task.CloseDateTime - task.StartDateTime;
            if (timeSpan == null) return taskDuration;
            TimeSpan ts = (TimeSpan)timeSpan;
            taskDuration = ts.Days;
            return taskDuration;
        }
    }
}