using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using GTDTest.DAL.Abstract;
using GTDTest.Models;
using GTDTest.DAL;
using GTDTest.Util;
using GTDTest.ViewModels;

namespace GTDTest.Controllers
{
    public class TaskController : Controller
    {
        private ITaskRepository taskRepository;

        public TaskController()
        {
            this.taskRepository = new TaskRepository(new GTDContext());
            //很多页面都需要这些dropdownlist，与其在各个页面分别构造，干脆在整个构造函数中一次搞定
            ViewBag.ProjectID = DropDownListHelp.PopulateProjectsDropDownList(taskRepository.GetContext());
            ViewBag.ContextId = DropDownListHelp.PopulateContextsDropDownList(taskRepository.GetContext());
            ViewBag.Priority = DropDownListHelp.PopulatePrioritysDropDownList();
            ViewBag.DateAttribute = DropDownListHelp.PopulateDateAttributeDropDownList();
            ViewBag.NextTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(taskRepository.GetContext());
            ViewBag.PreviousTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(taskRepository.GetContext());
        }

        // GET: /Task/
        public ActionResult Index()
        {
            return RedirectToAction("ListTask", new { da = DateAttribute.今日待办.ToString() });
        }

        // GET: /Task/Details/5
        public ActionResult Details(int id = 0)
        {
            Task task = taskRepository.GetTaskById(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            var viewmodel = new TaskDetail
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
                task.DateAttribute = SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
                //db.Tasks.Add(task);
                taskRepository.InsertTask(task);
                //db.SaveChanges();
                taskRepository.Save();
                return RedirectToAction("ListTask", new { da = task.DateAttribute.ToString() });
            }

            return View(task);
        }

        // GET: /Task/Edit/5
        public ActionResult Edit(int id = 0)
        {
            Task task = taskRepository.GetTaskById(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectID = DropDownListHelp.PopulateProjectsDropDownList(taskRepository.GetContext(), task.Pro != null ? task.Pro.ProjectId : new int());
            ViewBag.ContextId = DropDownListHelp.PopulateContextsDropDownList(taskRepository.GetContext(),
                task.Context != null ? task.Context.ContextId : new int());
            ViewBag.Priority = DropDownListHelp.PopulatePrioritysDropDownList(task.Priority != null ? task.Priority.Value.ToString() : "无");
            ViewBag.DateAttribute = DropDownListHelp.PopulateDateAttributeDropDownList(task.DateAttribute != null ? task.DateAttribute.Value.ToString() : "无");
            ViewBag.NextTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(taskRepository.GetContext(), task.NextTask_TaskId ?? new int());
            ViewBag.PreviousTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(taskRepository.GetContext(), task.PreviousTask_TaskId ?? new int());


            return View(task);
        }

        // POST: /Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Task task)
        {
            if (ModelState.IsValid)
            {
                task.DateAttribute = SetDateAttribute(task.StartDateTime, task.DateAttribute, task.ProjectID);
                taskRepository.UpdateTask(task);
                taskRepository.Save(); ;
                return RedirectToAction("ListTask", new { da = task.DateAttribute.ToString() });
            }
            ViewBag.ProjectID = DropDownListHelp.PopulateProjectsDropDownList(taskRepository.GetContext(), task.Pro != null ? task.Pro.ProjectId : new int());
            ViewBag.ContextId = DropDownListHelp.PopulateContextsDropDownList(taskRepository.GetContext(),
                task.Context != null ? task.Context.ContextId : new int());
            ViewBag.Priority = DropDownListHelp.PopulatePrioritysDropDownList(task.Priority != null ? task.Priority.Value.ToString() : "无");
            ViewBag.DateAttribute = DropDownListHelp.PopulateDateAttributeDropDownList(task.DateAttribute != null ? task.DateAttribute.Value.ToString() : "无");
            ViewBag.NextTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(taskRepository.GetContext(), task.NextTask_TaskId ?? new int());
            ViewBag.PreviousTask_TaskId = DropDownListHelp.PopulateTaskDropDownList(taskRepository.GetContext(), task.PreviousTask_TaskId ?? new int());

            return View(task);
        }

        // GET: /Task/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Task task = taskRepository.GetTaskById(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            var tempda = task.DateAttribute;
            taskRepository.DeleteTask(task.TaskId);
            taskRepository.Save();
            return RedirectToAction("ListTask", new { da = tempda.ToString() });
        }

        // POST: /Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = taskRepository.GetTaskById(id);
            var tempda = task.DateAttribute;
            taskRepository.DeleteTask(task.TaskId);
            taskRepository.Save();
            return RedirectToAction("ListTask", new { da = tempda.ToString() });
        }

        protected override void Dispose(bool disposing)
        {
            //db.Dispose();
            taskRepository.Dispose();
            base.Dispose(disposing);
        }

        // GET: /Task/Complete/5
        public ActionResult Complete(int id)
        {

            var sort = ViewBag.SortOrder;
            Task task = taskRepository.GetTaskById(id);
            IEnumerable<Task> previousTasks;
            if (task == null)
            {
                return HttpNotFound();
            }
            task.IsComplete = !task.IsComplete;
            task.CompleteDateTime = DateTime.Today;
            taskRepository.UpdateTask(task);
            if (task.PreviousTask_TaskId != null)
            {
                previousTasks = taskRepository.GetTasks().Where(t => t.TaskId == task.PreviousTask_TaskId);
                foreach (var t in previousTasks)
                {
                    if (t.StartDateTime == null)
                    {
                        t.StartDateTime = DateTime.Today;
                        t.DateAttribute = DateAttribute.今日待办;
                        taskRepository.UpdateTask(t);
                    }
                }
            }

            taskRepository.Save();
            return RedirectToAction("ListTask", new { da = task.DateAttribute, sortOrder = sort });
        }

        // GET: /Task/ListTask/da/sortOrder
        public ActionResult ListTask(string da = "收集箱", string sortOrder = "priority")
        {

            var dateAttribute = (DateAttribute)Enum.Parse(typeof(DateAttribute), da, true);
            ViewBag.Da = dateAttribute;
            ViewBag.SortOrder = sortOrder;
            IQueryable<Task> tasks = null;
            ViewBag.countSJX = 1;

            //收集箱：DateAttribute是收集箱的任务
            //今日待办：DateAttribute是今日待办，StartDateTime在今天及今天之前，不包括DateAttribute为将来也许和等待的，需要更新DateAttribute
            //下一步行动：DateAttribute是下一步行动
            //明日待办：StartDateTime是明天的，需要更新DateAttribute
            //日程：StartDateTime是明天之后的
            //将来也许，DateAttribute是将来也许
            //等待，DateAttribute是等待
            //先找到所有符合da条件的task，然后把da更新一遍
            switch (da)
            {
                case "今日待办":
                    {
                        tasks =
                         taskRepository.GetContext().Tasks.Include(t => t.Pro)
                             .Where(
                                 t => (t.IsComplete == false && (t.DateAttribute == dateAttribute || (t.StartDateTime != null && t.StartDateTime <= DateTime.Today))));
                        UpdateDateAttribute(tasks, DateAttribute.今日待办);

                    }
                    break;
                case "明日待办":
                    {
                        var tomorrow = DateTime.Today.AddDays(1);
                        tasks =
                         taskRepository.GetContext().Tasks.Include(i => i.Pro)
                             .Where(
                                 t => (t.DateAttribute == dateAttribute || (t.StartDateTime != null && t.StartDateTime == tomorrow))).Where(t => t.IsComplete == false)
                             ;
                        UpdateDateAttribute(tasks, DateAttribute.明日待办);
                    }
                    break;
                case "日程":
                    {
                        var tomorrow = DateTime.Today.AddDays(1);
                        tasks =
                            taskRepository.GetContext().Tasks.Include(i => i.Pro)
                                .Where(
                                    t => (t.DateAttribute == dateAttribute || (t.StartDateTime != null && t.StartDateTime > tomorrow))).Where(t => t.IsComplete == false)
                               ;
                        UpdateDateAttribute(tasks, DateAttribute.日程);
                        break;
                    }
                default:
                    tasks = taskRepository.GetContext().Tasks.Include(t => t.Pro).Where(t => t.DateAttribute == dateAttribute).Where(t => t.IsComplete == false).Where(t => t.IsComplete == false).OrderByDescending(t => t.TaskId);
                    break;
            }
            //重新把需要的task选择出来
            tasks = taskRepository.GetContext().Tasks.Include(i => i.Pro).Where(i => i.DateAttribute == dateAttribute).Where(i => i.IsComplete == false && i.IsDeleted == false).OrderByDescending(i => i.TaskId);

            //switch (sortOrder)
            //{
            //    //排序方式，有值的放前面，无值的放后面，有值的排序方案每个字段不一样。
            //    case "priority":
            //        tasks = tasks.OrderByDescending(t => t.Priority.HasValue).ThenBy(t => t.Priority);
            //        break;
            //    case "project":
            //        tasks = tasks.OrderByDescending(t => t.ProjectID.HasValue).ThenBy(t => t.ProjectID);
            //        break;
            //    case "startat":
            //        tasks = tasks.OrderByDescending(t => t.StartDateTime.HasValue).ThenBy(t => t.StartDateTime);
            //        break;
            //    case "closeat":
            //        tasks = tasks.OrderByDescending(t => t.CloseDateTime.HasValue).ThenBy(t => t.CloseDateTime);
            //        break;
            //    case "context":
            //        tasks = tasks.OrderByDescending(t => t.ContextID.HasValue).ThenBy(t => t.ContextID);
            //        break;
            //    default:
            //        break;
            //}
            ViewBag.Title = da + "  （" + tasks.Count() + "）";
            var viewmodel = new Tasklist(tasks, sortOrder);
            return View("ListTask2", viewmodel);
        }

        //根据输入的其他属性，判断DateAttribute应该是什么
        //业务规则如下：
        //开始时间：无             项目：无     收集箱
        //开始时间：今天           项目：任意   今日待办
        //开始时间：无             项目：有    下一步行动
        //开始时间：明天           项目：任意   明日待办
        //开始时间：今天/明天以外   项目：任意   日程
        //开始时间：无             项目：任意   需要主动设置 将来也许
        //开始时间：无             项目：任意   需要主动设置 等待
        /// <summary>
        ///创建任务的时判断DateAttribute
        /// </summary>
        /// <param name="star"></param>
        /// <param name="att"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        private DateAttribute? SetDateAttribute(DateTime? star, DateAttribute? att, int? projectid)
        {
            var goalAtt = att;
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
            else if (goalAtt == DateAttribute.将来也许
                    || goalAtt == DateAttribute.等待
                    || goalAtt == DateAttribute.收集箱)
                return goalAtt;
            else
                return DateAttribute.收集箱;

            /*
                        return goalAtt;
            */
        }

        /// <summary>
        /// 根据今天的情况更新DateAttribute
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="da"></param>
        private void UpdateDateAttribute(IQueryable<Task> tasks, DateAttribute da)
        {
            foreach (var task in tasks)
            {
                task.DateAttribute = da;
                taskRepository.UpdateTask(task);

            }
            taskRepository.Save();
        }

        /// <summary>
        /// 显示已经完成的任务列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CompletedTask()
        {
            var tasks = taskRepository.GetTasks().Where(i => i.IsComplete && i.IsDeleted == false).OrderByDescending(i => i.TaskId);
            ViewBag.Title = "已完成任务";
            var viewmodel = new Tasklist(tasks, "CompleteDateTime");
            return View("CompletedTaskList", viewmodel);
        }

        public ActionResult DeletedTask()
        {
            ViewBag.Title = "已删除任务";
            var tasks = taskRepository.GetTasks().Where(i => i.IsDeleted).OrderByDescending(i => i.TaskId);
            return View("DeletedTaskList", tasks.ToList());
        }


        //展示每周待办任务和完成任务，方便写周报
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
                WeeklyToDoTasks = from t in taskRepository.GetTasks()
                                  where (t.IsComplete == false && t.IsDeleted == false)
                                  where ((t.StartDateTime != null && t.StartDateTime <= lastDayOfWeek) || (t.CloseDateTime != null && t.CloseDateTime <= lastDayOfWeek))
                                  orderby (t.Priority.HasValue) descending,t.Priority
                                  select t
                                  ,
                WeeklyCompletedTask = from t in taskRepository.GetTasks()
                                      where (t.IsComplete && t.IsDeleted == false && t.CompleteDateTime >= firstDayOfWeek && t.CompleteDateTime <= lastDayOfWeek)
                                      orderby t.CompleteDateTime descending
                                      select t

                //taskRepository.GetTasks().Where(t=>t.IsComplete==true &)

            };
            return View(viewmodel);
        }
    }
}