using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.WebPages;
using Castle.Core.Internal;
using GTD.Util;
using Newtonsoft.Json;

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
                cycTasks[0].RepeatJson = cycTasks[0].RepeatJson.Replace("}", ",'id':'" + id + "'}");
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
                Task t = (Task) task.Clone();
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

        //计算任务的周期
        private int TaskDuration(Task task)
        {
            int taskDuration = 0;
            var timeSpan = task.CloseDateTime - task.StartDateTime;
            if (timeSpan == null) return taskDuration;
            TimeSpan ts = (TimeSpan) timeSpan;
            taskDuration = ts.Days;
            return taskDuration;
        }

        public void UpdateTask(Task task)
        {
            task.DateAttribute = SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
            _taskRepository.Update(task);

        }

        public IEnumerable<Task> GetCompletedTasks()
        {
            return _taskRepository.GetAll().Include(t => t.Pro).Where(t => t.IsComplete == true && t.IsDeleted == false);
        }

        public IEnumerable<Task> GetInProgressTasks()
        {
            return _taskRepository.GetAll().Include(t => t.Pro).Where(t => t.IsComplete == false && t.IsDeleted == false);
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
            //_taskRepository.Updates(tasks);
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

        //public void BatchUpdateTask(IEnumerable<Task> tasks)
        //{
        //    _taskRepository.BatchUpdateTask(tasks);
        //}

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
                var headline = GetTaskNameFromText(t);
                var project = GetProjectNameFromText(t);
                if (headline != null)
                {
                    tasklist.Add(new Task { Headline = headline, ProjectID = _projectServices.IsExistByName(project) });
                }
                //var t1 = t.Trim();
                //if (t1.IsEmpty()) continue; ;

                ////开头是#：#到第一个空格，作为项目名称，空格后所有内容作为项目标题（不去中间空格）
                //if (t1[0] == '#')
                //{
                //    //用第一个空格的位置来确定项目名称
                //    var i = t1.IndexOf(" ", StringComparison.Ordinal);
                //    if (i > 1)
                //    {
                //        project = t1.Substring(1, i - 1).Trim();
                //    }
                //    headline = t1.Substring(i + 1).Trim();
                //    if (!string.IsNullOrEmpty(project) && !string.IsNullOrEmpty(headline))
                //    {
                //        Task task = new Task { Headline = headline, ProjectID = _projectServices.IsExistByName(project) };
                //        tasks.Add(task);
                //        continue;
                //    }

                //}

                ////其他内容开头：空格#到紧接的空格，作为项目名称。项目名称之前的内容作为任务名称，项目名称之后的内容废弃。
                //if (t1.IndexOf(" #", StringComparison.Ordinal) > 0)
                //{
                //    int projectstart = t1.IndexOf(" #", StringComparison.Ordinal);
                //    project = t1.Substring(projectstart + 2, t1.Length - projectstart - 2).Trim();
                //    headline = t1.Substring(0, projectstart).Trim();
                //    if (!string.IsNullOrEmpty(project) && !string.IsNullOrEmpty(headline))
                //    {
                //        Task task = new Task { Headline = headline, ProjectID = _projectServices.IsExistByName(project) };
                //        tasks.Add(task);
                //        continue;
                //    }
                //}

                ////没有#的：所有内容作为task
                //if (t1.IndexOf("#", StringComparison.Ordinal) < 0)
                //{
                //    if (!string.IsNullOrEmpty(t1))
                //    {
                //        Task task = new Task { Headline = t1 };
                //        tasks.Add(task);
                //        continue;
                //    }
                //}
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

        //输入文本，返回任务名称
        public string GetTaskNameFromText(string tasktext)
        {
            string taskname = null;
            var t1 = tasktext.Trim();
            if (t1.IsEmpty()) return null; ;

            //开头是#：#到第一个空格作为项目名称，空格后所有内容作为项目标题（不去中间空格）
            if (t1.IndexOf("#", StringComparison.Ordinal) == 0)
            {
                int i = t1.IndexOf(" ", StringComparison.Ordinal);
                if (i > 0)
                {
                    taskname = t1.Substring(i + 1).Trim();
                }
            }

            //其他内容开头：空格#到紧接的空格，作为项目名称。项目名称之前的内容作为任务名称，项目名称之后的内容废弃。
            if (t1.IndexOf(" #", StringComparison.Ordinal) > 0)
            {
                var i = t1.IndexOf(" #", StringComparison.Ordinal);
                taskname = t1.Substring(0, i).Trim();
            }

            //没有#的：所有内容作为task
            if (t1.IndexOf("#", StringComparison.Ordinal) < 0)
            {
                taskname = t1;
            }
            return taskname;
        }

        //输入文本，返回项目名称
        public string GetProjectNameFromText(string tasktext)
        {
            string projectname = null;
            var t1 = tasktext.Trim();
            if (t1.IsEmpty()) return null; ;

            //开头是#：#到第一个空格作为项目名称
            if (t1.IndexOf("#", StringComparison.Ordinal) == 0)
            {
                //用第一个空格的位置来确定项目名称
                var i = t1.IndexOf(" ", StringComparison.Ordinal);
                if (i > 0)
                {
                    projectname = t1.Substring(1, i - 1).Trim();
                }
            }

            //其他内容开头：空格#到紧接的空格作为项目名称
            if (t1.IndexOf(" #", StringComparison.Ordinal) > 0)
            {
                int projectstart = t1.IndexOf(" #", StringComparison.Ordinal);
                projectname = t1.Substring(projectstart + 2, t1.Length - projectstart - 2).Trim();
            }

            //没有#的：没有project
            if (t1.IndexOf("#", StringComparison.Ordinal) < 0)
            {
            }
            return projectname;
        }
    }
}