using GTD.Models;
using GTD.Services.Abstract;
using GTD.Util;
using GTD.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using GTD.Filters;

namespace GTD.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskServices _taskServices;
        private readonly IProjectServices _projectServices;
        private readonly IContextServices _contextServices;

        public TaskController(ITaskServices taskServices, IProjectServices projectServices, IContextServices contextServices)
        {
            _taskServices = taskServices;
            _projectServices = projectServices;
            _contextServices = contextServices;
        }

        // GET: /Task/
        //[RepeatTaskFilter]
        public ActionResult Index()
        {
            return RedirectToAction("ListTask", new { da = DateAttribute.今日待办.ToString() });
        }

        // GET: /Task/Details/5
        public ActionResult Details(int id = 0)
        {
            //todo 显示任务detail的时候，可以展示出已经删除的项目
            Task task = _taskServices.GetTaskById(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            var viewmodel = new TaskDetailVM
            {
                Task = task,
                CompletedSubTasks = task.SubTasks.Where(s => s.IsComplete).OrderByDescending(s => s.SubTaskId),
                InprogressSubTasks = task.SubTasks.Where(s => s.IsComplete == false).OrderByDescending(s => s.SubTaskId),
                NextTask = _taskServices.GetNextTaskByTaskId(id),
                PreviousTask = _taskServices.GetPreviousTaskByTaskId(id)
            };
            return View(viewmodel);
        }

        // GET: /Task/Create
        public ActionResult Create(string da)
        {
            ViewBag.dataAttr = da;
            InitView();
            return View();
        }

        // POST: /Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Task task)
        {
            if (ModelState.IsValid)
            {
                if (task.StartDateTime != null && task.CloseDateTime == null)
                {
                    task.CloseDateTime = task.StartDateTime;
                }
                _taskServices.AddTask(task);
                return RedirectToAction("ListTask", new { da = task.DateAttribute.ToString() });
            }
            InitView(task);
            return View(task);
        }

        /// <summary>
        /// Get: /Task/CreateInLine
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult CreateInLine()
        {
            var projectList = from p in _projectServices.GetAllInprogressProjects() select p.ProjectName;
            var contextList = from c in _contextServices.GetAllContexts() select c.ContextName;

            ViewBag.projects = projectList.ToList();

            ViewBag.contexts = contextList.ToList();
            return PartialView("_CreateTaskInLinePartialPage");
        }


        // POST: /Task/CreateInLine
        [HttpPost]
        public PartialViewResult CreateInLine(string createTaskInLine, string da = "收集箱", string sortOrder = "priority")
        {
            var t = new Task()
            {
                Headline = createTaskInLine,
                StartDateTime = DateTime.Now.Date,
                CloseDateTime = DateTime.Now.Date,
                IsComplete = false,
                IsDeleted = false,
                SubTasks = new List<SubTask>()
            };

            //复杂的处理逻辑，生成一个task
            _taskServices.AddTask(t);

            var dateAttribute = (DateAttribute)Enum.Parse(typeof(DateAttribute), da, true);
            var workingtasks = _taskServices.GetTasksWithRealDa(dateAttribute).OrderByDescending(i => i.TaskId).ToList();
            var viewmodel = new TasklistVM(workingtasks, sortOrder);

            return PartialView("_ListTasksPartialPage", viewmodel);
        }



        // GET: /Task/Edit/5
        public ActionResult Edit(int id = 0)
        {
            //todo 编辑已完成任务时，可以选到删除的project，在创建dropdownlist的时候控制selectitem，需要做一个判断

            Task task = _taskServices.GetTaskById(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            InitView(task);
            //todo 日志入侵了业务，需要修改
            LogHelper.WriteLog($"Get  : TaskId: {task.TaskId,-5} , StartDate: {task.StartDateTime,-20} , CloseDate: {task.CloseDateTime,-20}");
            return View(task);
        }

        //[Log]
        // POST: /Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Task task)
        {
            if (ModelState.IsValid)
            {
                //todo 日志入侵了业务，需要修改
                LogHelper.WriteLog($"Post : TaskId: {task.TaskId,-5} , StartDate: {task.StartDateTime,-20} , CloseDate: {task.CloseDateTime,-20}");
                _taskServices.UpdateTask(task);
                return RedirectToAction("ListTask", new { da = task.DateAttribute.ToString() });

            }

            InitView(task);
            return View(task);
        }

        // GET: /Task/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Task task = _taskServices.GetTaskById(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            var tempda = task.DateAttribute;
            _taskServices.DeleteTask(task.TaskId);
            return RedirectToAction("ListTask", new { da = tempda.ToString() });
        }

        // POST: /Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = _taskServices.GetTaskById(id);
            var tempda = task.DateAttribute;
            _taskServices.DeleteTask(task.TaskId);
            return RedirectToAction("ListTask", new { da = tempda.ToString() });
        }

        // GET: /Task/Complete/5
        public ActionResult Complete(int id)
        {
            var sort = ViewBag.SortOrder;
            Task task = _taskServices.GetTaskById(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            _taskServices.CompleteTask(task);
            return RedirectToAction("ListTask", new { da = task.DateAttribute, sortOrder = sort });
        }

        // GET: /Task/ListTask/da/sortOrder
        [RepeatTaskFilter]
        public ActionResult ListTask(string da = "收集箱", string sortOrder = "priority")
        {
            var dateAttribute = (DateAttribute)Enum.Parse(typeof(DateAttribute), da, true);
            ViewBag.Da = dateAttribute;
            ViewBag.SortOrder = sortOrder;
            ViewBag.countSJX = 1;

            //重新把需要的task选择出来
            /*
             * todo 这里面有操作数据库的活动，在每次列表的时候都会执行，效率其实很差，完全没有必要。
             * 其实每天执行1次即可,如果不放心，每次session过期执行一次即可
            */
            var workingtasks = _taskServices.GetTasksWithRealDa(dateAttribute).OrderByDescending(i => i.TaskId).ToList();

            ViewBag.Title = da + "  （" + workingtasks.Count() + "）";
            //var viewmodel = new TasklistVM(workingtasks, sortOrder);
            return View("ListTask2");
        }

        public PartialViewResult GetTasks(string da = "收集箱", string sortOrder = "priority")
        {
            var dateAttribute = (DateAttribute)Enum.Parse(typeof(DateAttribute), da, true);

            var workingtasks = _taskServices.GetTasksWithRealDa(dateAttribute).OrderByDescending(i => i.TaskId).ToList();
            var viewmodel = new TasklistVM(workingtasks, sortOrder);

            return PartialView("_ListTasksPartialPage", viewmodel);
        }


        /// <summary>
        /// 显示已经完成的任务列表
        /// Get: /Task/CompletedTask
        /// </summary>
        /// <returns></returns>
        public ActionResult CompletedTask()
        {
            var tasks = _taskServices.GetAll().Where(i => i.IsComplete && i.IsDeleted == false).OrderByDescending(i => i.TaskId);
            ViewBag.Title = "已完成任务";
            var viewmodel = new TasklistVM(tasks, "CompleteDateTime");
            return View("CompletedTaskList", viewmodel);
        }

        /// <summary>
        /// 显示已经删除的任务列表
        /// Get: /Task/DeletedTask
        /// </summary>
        /// <returns></returns>
        public ActionResult DeletedTask()
        {
            ViewBag.Title = "已删除任务";
            var tasks = _taskServices.GetAll().Where(i => i.IsDeleted).OrderByDescending(i => i.TaskId);
            return View("DeletedTaskList", tasks.ToList());
        }

        /// <summary>
        /// 显示每周待办任务和完成任务，方便写周报
        /// Get: /Task/ListTaskofWeek
        /// </summary>
        /// <param name="datestring"></param>
        /// <returns></returns>
        public ActionResult ListTaskofWeek(string datestring)
        {
            if (datestring == null)
                datestring = DateTime.Today.ToString(CultureInfo.InvariantCulture);
            //计算本周的第一天和最后一天
            DateTime day = Convert.ToDateTime(datestring);
            DateTime firstDayOfWeek = day.AddDays(0 - Convert.ToInt16(day.DayOfWeek));
            DateTime lastDayOfWeek = day.AddDays(6 - Convert.ToInt16(day.DayOfWeek));
            var viewmodel = new TaskWeekly()
            {
                WeeklyToDoTasks = from t in _taskServices.GetAll()
                                  where (t.IsComplete == false && t.IsDeleted == false)
                                  where ((t.StartDateTime != null && t.StartDateTime <= lastDayOfWeek) || (t.CloseDateTime != null && t.CloseDateTime <= lastDayOfWeek))
                                  orderby (t.Priority.HasValue) descending, t.Priority
                                  select t
                                  ,
                WeeklyCompletedTask = from t in _taskServices.GetAll()
                                      where (t.IsComplete && t.IsDeleted == false && t.CompleteDateTime >= firstDayOfWeek && t.CompleteDateTime <= lastDayOfWeek)
                                      orderby t.CompleteDateTime descending
                                      select t

                                      //taskRepository.GetTasks().Where(t=>t.IsComplete==true &)
            };
            return View(viewmodel);
        }

        /// <summary>
        /// 批量新增页面
        /// Get: /Task/BatchCreate
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchCreate()
        {
            return View();
        }

        /// <summary>
        /// 批量新增页面
        /// Post: /Task/BatchCreate
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BatchCreate(string taskTexts)
        {
            //先判空
            if (taskTexts == null || taskTexts.IsEmpty()) return View();

            //文本不为空，但是没有合格的task
            var tasks = _taskServices.SplitTextToTasks(taskTexts);
            if (tasks == null) return View();

            //有合格的task
            foreach (var t in tasks)
            {
                _taskServices.AddTask(t);
            }
            return RedirectToAction("ListTask", new { da = "收集箱" });
        }

        private void InitView()
        {
            ViewBag.NextTask_TaskId = new SelectList(_taskServices.GetInProgressTasks(), "TaskID", "Headline");
            ViewBag.PreviousTask_TaskId = new SelectList(_taskServices.GetInProgressTasks(), "TaskID", "Headline");
            ViewBag.ProjectID = new SelectList(_projectServices.GetAllInprogressProjects().OrderBy(p => p.ProjectName),
                "ProjectID", "ProjectName");
            ViewBag.ContextId = DropDownListHelp.PopulateContextsDropDownList(_contextServices);
            ViewBag.Priority = DropDownListHelp.PopulatePrioritysDropDownList();
            ViewBag.DateAttribute = DropDownListHelp.PopulateDateAttributeDropDownList();

        }

        private void InitView(Task task)
        {
            ViewBag.ProjectID = new SelectList(_projectServices.GetAllInprogressProjects().OrderBy(p => p.ProjectName),
                "ProjectID", "ProjectName", task.Pro?.ProjectId ?? new int());
            ViewBag.ContextId = DropDownListHelp.PopulateContextsDropDownList(_contextServices,
                task.Context?.ContextId ?? new int());
            ViewBag.Priority = DropDownListHelp.PopulatePrioritysDropDownList(task.Priority?.ToString() ?? "无");
            ViewBag.DateAttribute = DropDownListHelp.PopulateDateAttributeDropDownList(task.DateAttribute?.ToString() ?? "无");
            ViewBag.NextTask_TaskId = new SelectList(_taskServices.GetInProgressTasks().Where(t => t.TaskId != task.TaskId),
                "TaskID", "Headline", task.NextTask_TaskId ?? new int());
            ViewBag.PreviousTask_TaskId = new SelectList(_taskServices.GetInProgressTasks().Where(t => t.TaskId != task.TaskId),
                "TaskID", "Headline", task.PreviousTask_TaskId ?? new int());
            //ViewBag.PreviousTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(_taskServices, task.PreviousTask_TaskId ?? new int());
            //ViewBag.NextTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(_taskServices, task.NextTask_TaskId ?? new int());

        }
    }
}