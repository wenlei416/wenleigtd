﻿using System.Linq;
using System.Web.Mvc;
using GTD.DAL;
using GTD.Models;
using GTD.Services;
using GTD.Services.Abstract;
using GTD.ViewModels.ProjectVM;

namespace GTD.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ITaskServices _taskServices;
        private readonly IProjectServices _projectServices;

        //public ProjectController(ITaskServices taskServices)
        //{
        //    _projectServices = new ProjectServices();
        //    this._taskServices = taskServices;
        //}

        public ProjectController(ITaskServices taskServices, IProjectServices projectServices)
        {
            this._projectServices = projectServices;
            this._taskServices = taskServices;
        }

        // GET: /Project/
        public ActionResult Index()
        {
            return View(_projectServices.GetAllInprogressProjects());

        }

        // GET: /Project/Details/5
        public ActionResult Details(int id = 0)
        {
            Project project = _projectServices.GetProjectById(id);

            if (project == null)
            {
                return HttpNotFound();
            }
            ProjectDeatilVM projectDeatilVm = new ProjectDeatilVM
            {
                Project = project,
                ToDoTasks = _taskServices.GetInProgressTasks()
                    .Where(t => t.Pro != null)
                    .Where(t => t.Pro.ProjectId == project.ProjectId),
                CompletedTasks = _taskServices.GetCompletedTasks()
                    .Where(t => t.Pro != null)
                    .Where(t => t.Pro.ProjectId == project.ProjectId)
            };

            return View(projectDeatilVm);
        }

        // GET: /Project/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: /Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                _projectServices.CreateProject(project);
                return RedirectToAction("Index");
            }

            return View(project);
        }


        // GET: /Project/Edit/5
        public ActionResult Edit(int id = 0)
        {
            Project project = _projectServices.GetProjectById(id);//_db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }


        // POST: /Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                _projectServices.UpdateProject(project);
                return RedirectToAction("Index");
            }
            return View(project);
        }


        // GET: /Project/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Project project = _projectServices.GetProjectById(id);//_db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }


        // POST: /Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = _projectServices.GetProjectById(id);//_db.Projects.Find(id);
            _projectServices.DeleteProjectByLogic(project);
            //todo 这里有bug，如果项目关联的有未完成的task，这里实际无法删除成功

            var tasks = _taskServices.GetAll().Where(t => t.ProjectID == id && t.IsComplete == false);
            _taskServices.BreakRelationTaskandProject(tasks);

            return RedirectToAction("Index");
        }

        // Post: /Project/Complete/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ProjectID</param>
        /// <returns>true，表示全部都完成了；false表示有未完成的</returns>
        [HttpPost]
        public JsonResult Complete(int id)
        {
            //todo:应该用model state的方式来验证model
            //检查是否还有未完成的任务
            //如果有就提示有Task未完成
            //如果没有就改变状态返回值
            Project project = _projectServices.GetProjectById(id);

            var allTaskCompleted = project.Tasks.Where(t => t.IsDeleted == false).All(t => t.IsComplete == true);

            if (allTaskCompleted)
            {
                project.IsComplete = !project.IsComplete;
                _projectServices.UpdateProject(project);
            }
            return Json(allTaskCompleted);
        }
    }
}