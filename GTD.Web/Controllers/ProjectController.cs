using System.Data;
using System.Linq;
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
        private readonly GTDContext _db = new GTDContext();
        private readonly ITaskServices _taskServices = new TaskServices();
        private readonly IProjectServices _projectServices = new ProjectServices();


        // GET: /Project/
        public ActionResult Index()
        {
            return View(_projectServices.GetAllProjects());

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
            return RedirectToAction("Index");
        }

        // GET: /Project/Complete/5
        [HttpPost]
        public string Complete(int id)
        {
            //检查是否还有未完成的任务
            //如果有就XXX
            //如果没有就改变状态，返回值
            Project project = _projectServices.GetProjectById(id);

            var allTaskCompleted = project.Tasks.Any(t => t.IsComplete == false);

            project.IsComplete = !project.IsComplete;
            _projectServices.UpdateProject(project);
            return project.IsComplete ? "已完成" : "进行中";
        }
    }
}