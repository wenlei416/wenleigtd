using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services;
using GTD.Services.Abstract;
using GTD.Util;
using GTD.ViewModels;

namespace GTD.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskServices _taskServices;

        public TaskController()
        {
            _taskServices = new TaskServices();

            //很多页面都需要这些dropdownlist，与其在各个页面分别构造，干脆在整个构造函数中一次搞定
            ViewBag.ProjectID = DropDownListHelp.PopulateProjectsDropDownList(_taskServices.GetContext());
            ViewBag.ContextId = DropDownListHelp.PopulateContextsDropDownList(_taskServices.GetContext());
            ViewBag.Priority = DropDownListHelp.PopulatePrioritysDropDownList();
            ViewBag.DateAttribute = DropDownListHelp.PopulateDateAttributeDropDownList();
            ViewBag.NextTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(_taskServices.GetContext());
            ViewBag.PreviousTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(_taskServices.GetContext());
        }

        // GET: /Task/
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
                InprogressSubTasks = task.SubTasks.Where(s => s.IsComplete == false).OrderByDescending(s => s.SubTaskId)
            };
            return View(viewmodel);
        }

        // GET: /Task/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Task task)
        {
            if (ModelState.IsValid)
            {
                _taskServices.AddTask(task);
                //task.DateAttribute = SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
                //_taskRepository.InsertTask(task);
                //_taskRepository.Save();
                return RedirectToAction("ListTask", new { da = task.DateAttribute.ToString() });
            }

            return View(task);
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
            ViewBag.ProjectID = DropDownListHelp.PopulateProjectsDropDownList(_taskServices.GetContext(), task.Pro != null ? task.Pro.ProjectId : new int());
            ViewBag.ContextId = DropDownListHelp.PopulateContextsDropDownList(_taskServices.GetContext(),
                task.Context != null ? task.Context.ContextId : new int());
            ViewBag.Priority = DropDownListHelp.PopulatePrioritysDropDownList(task.Priority != null ? task.Priority.Value.ToString() : "无");
            ViewBag.DateAttribute = DropDownListHelp.PopulateDateAttributeDropDownList(task.DateAttribute != null ? task.DateAttribute.Value.ToString() : "无");
            ViewBag.NextTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(_taskServices.GetContext(), task.NextTask_TaskId ?? new int());
            ViewBag.PreviousTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(_taskServices.GetContext(), task.PreviousTask_TaskId ?? new int());


            return View(task);
        }

        // POST: /Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Task task)
        {
            if (ModelState.IsValid)
            {

                _taskServices.UpdateTask(task);
                return RedirectToAction("ListTask", new { da = task.DateAttribute.ToString() });
            }
            ViewBag.ProjectID = DropDownListHelp.PopulateProjectsDropDownList(_taskServices.GetContext(), task.Pro != null ? task.Pro.ProjectId : new int());
            ViewBag.ContextId = DropDownListHelp.PopulateContextsDropDownList(_taskServices.GetContext(),
                task.Context != null ? task.Context.ContextId : new int());
            ViewBag.Priority = DropDownListHelp.PopulatePrioritysDropDownList(task.Priority != null ? task.Priority.Value.ToString() : "无");
            ViewBag.DateAttribute = DropDownListHelp.PopulateDateAttributeDropDownList(task.DateAttribute != null ? task.DateAttribute.Value.ToString() : "无");
            ViewBag.NextTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(_taskServices.GetContext(), task.NextTask_TaskId ?? new int());
            ViewBag.PreviousTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(_taskServices.GetContext(), task.PreviousTask_TaskId ?? new int());

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
            //todo 重新打开已完成任务时，无法关联到已删除项目
            //todo 需要抽出来放services
            var sort = ViewBag.SortOrder;
            Task task = _taskServices.GetTaskById(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            task.IsComplete = !task.IsComplete;
            task.CompleteDateTime = DateTime.Today;
            _taskServices.UpdateTask(task);

            //如果后续任务没有明确的日程，设为今日待办
            if (task.NextTask_TaskId != null)
            {
                var nextTask = _taskServices.GetAll().Single(t => t.TaskId == task.NextTask_TaskId);
                if (nextTask != null && nextTask.StartDateTime == null)
                {
                    nextTask.StartDateTime = DateTime.Today;
                    nextTask.DateAttribute = DateAttribute.今日待办;
                    _taskServices.UpdateTask(nextTask);
                }
            }
            //前置任务为这个任务的其他任务，如果没有明确的日程，设为今日待办
            var previousTasks = _taskServices.GetAll().Where(t => t.PreviousTask_TaskId == task.TaskId);
            foreach (var t in previousTasks)
            {
                if (t.StartDateTime == null)
                {
                    t.StartDateTime = DateTime.Today;
                    t.DateAttribute = DateAttribute.今日待办;
                }
            }
            _taskServices.BatchUpdateTask(previousTasks);
            return RedirectToAction("ListTask", new { da = task.DateAttribute, sortOrder = sort });
        }

        // GET: /Task/ListTask/da/sortOrder
        public ActionResult ListTask(string da = "收集箱", string sortOrder = "priority")
        {
            var dateAttribute = (DateAttribute)Enum.Parse(typeof(DateAttribute), da, true);
            ViewBag.Da = dateAttribute;
            ViewBag.SortOrder = sortOrder;
            ViewBag.countSJX = 1;

            //重新把需要的task选择出来
            var workingtasks = _taskServices.GetTasksWithRealDa(dateAttribute).OrderByDescending(i=>i.TaskId).ToList();

            ViewBag.Title = da + "  （" + workingtasks.Count() + "）";
            var viewmodel = new TasklistVM(workingtasks, sortOrder);
            return View("ListTask2", viewmodel);
        }


        /// <summary>
        /// 显示已经完成的任务列表
        /// </summary>
        /// <returns></returns>
        // Get: /Task/CompletedTask
        public ActionResult CompletedTask()
        {
            var tasks = _taskServices.GetAll().Where(i => i.IsComplete && i.IsDeleted == false).OrderByDescending(i => i.TaskId);
            ViewBag.Title = "已完成任务";
            var viewmodel = new TasklistVM(tasks, "CompleteDateTime");
            return View("CompletedTaskList", viewmodel);
        }

        //Get: /Task/DeletedTask
        public ActionResult DeletedTask()
        {
            ViewBag.Title = "已删除任务";
            var tasks = _taskServices.GetAll().Where(i => i.IsDeleted).OrderByDescending(i => i.TaskId);
            return View("DeletedTaskList", tasks.ToList());
        }

        //Get: /Task/ListTaskofWeek
        /// <summary>
        /// 展示每周待办任务和完成任务，方便写周报
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
    }
}