using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTDTest.Migrations;
using GTDTest.Models;
using GTDTest.DAL;

namespace GTDTest.Controllers
{
    public class SubTaskController : Controller
    {
        private GTDContext db = new GTDContext();

        //
        // GET: /SubTask/

        public ActionResult Index()
        {
            return View(db.SubTasks.ToList());
        }

        //
        // GET: /SubTask/Create

        public ActionResult Create(int id)
        {
            SubTask newSubtask = new SubTask() { TaskId = id, Task = db.Tasks.Find(id) };
            return View(newSubtask);
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
                return RedirectToAction("Details", "Task", new {id = subtask.TaskId});
            }

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

        public ActionResult Complete(int id=0)
        {
            SubTask subtask = db.SubTasks.Find(id);
            if (subtask == null)
            {
                return HttpNotFound();
            }
            subtask.IsComplete = !subtask.IsComplete;
            db.SaveChanges();

            return RedirectToAction("Details","Task",new {id=subtask.TaskId}); 
        }
    }
}