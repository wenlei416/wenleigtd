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


        // GET: /Project/
        public ActionResult Index()
        {
            return View(_db.Projects.ToList());
        }

        // GET: /Project/Details/5
        public ActionResult Details(int id = 0)
        {
            Project project = _db.Projects.Find(id);

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
                _db.Projects.Add(project);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }


        // GET: /Project/Edit/5
        public ActionResult Edit(int id = 0)
        {
            Project project = _db.Projects.Find(id);
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
                _db.Entry(project).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }


        // GET: /Project/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Project project = _db.Projects.Find(id);
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
            Project project = _db.Projects.Find(id);
            _db.Projects.Remove(project);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Project/Complete/5
        public ActionResult Complete(int id)
        {
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}