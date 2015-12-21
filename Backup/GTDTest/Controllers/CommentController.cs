using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTDTest.Models;
using GTDTest.DAL;

namespace GTDTest.Controllers
{
    public class CommentController : Controller
    {
        private GTDContext db = new GTDContext();

        //
        // GET: /Comment/

        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Task).OrderByDescending(t=>t.TaskId).ThenByDescending(t=>t.CommentId);
            return View(comments.ToList());
        }


        //
        // GET: /Comment/Create

        public ActionResult Create()
        {
            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline");
            return View();
        }

        //
        // POST: /Comment/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details","Task",new{id=comment.TaskId});
            }

            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline", comment.TaskId);
            return View(comment);
        }

        //
        // GET: /Comment/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline", comment.TaskId);
            return View(comment);
        }

        //
        // POST: /Comment/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline", comment.TaskId);
            return View(comment);
        }

        //
        // GET: /Comment/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        //
        // POST: /Comment/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}