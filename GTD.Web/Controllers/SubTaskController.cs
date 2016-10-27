using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using GTD.DAL;
using GTD.Models;

namespace GTD.Controllers
{
    public class SubTaskController : Controller
    {
        private GTDContext db = new GTDContext();

        //
        // GET: /SubTask/

        public ActionResult Index()
        {
            var subtasks = db.SubTasks.Include(s => s.Task);
            return View(subtasks.ToList());
        }

        //
        // GET: /SubTask/Details/5

        public ActionResult Details(int id = 0)
        {
            SubTask subtask = db.SubTasks.Find(id);
            if (subtask == null)
            {
                return HttpNotFound();
            }
            return View(subtask);
        }

        //
        // GET: /SubTask/Create

        public ActionResult Create(int? id)
        {
            ViewBag.TaskId = new SelectList(db.Tasks.Where(i => i.IsComplete == false && i.IsDeleted == false), "TaskId", "Headline", id);
            
            return View();
        }

        //
        // POST: /SubTask/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubTask subtask)
        {
            if (ModelState.IsValid)
            {
                db.SubTasks.Add(subtask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline", subtask.TaskId);
            return View(subtask);
        }

        //
        // GET: /SubTask/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SubTask subtask = db.SubTasks.Find(id);
            if (subtask == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline", subtask.TaskId);
            return View(subtask);
        }

        //
        // POST: /SubTask/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SubTask subtask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subtask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline", subtask.TaskId);
            return View(subtask);
        }

        //
        // GET: /SubTask/Delete/5

        public ActionResult Delete(int id = 0)
        {
            SubTask subtask = db.SubTasks.Find(id);
            if (subtask == null)
            {
                return HttpNotFound();
            }
            return View(subtask);
        }

        //
        // POST: /SubTask/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SubTask subtask = db.SubTasks.Find(id);
            db.SubTasks.Remove(subtask);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Complete(int id)
        {
            var sub = db.SubTasks.FirstOrDefault(s=>s.SubTaskId==id);
            if (sub != null)
            {
                sub.IsComplete = !sub.IsComplete;
                db.Entry(sub).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Task", new { id = sub.TaskId });
        }
    }
}