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
    public class ContextController : Controller
    {
        private GTDContext db = new GTDContext();

        //
        // GET: /Context/

        public ActionResult Index()
        {
            return View(db.Contexts.ToList());
        }

        //
        // GET: /Context/Details/5

        public ActionResult Details(int id = 0)
        {
            Context context = db.Contexts.Find(id);
            if (context == null)
            {
                return HttpNotFound();
            }
            return View(context);
        }

        //
        // GET: /Context/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Context/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Context context)
        {
            if (ModelState.IsValid)
            {
                db.Contexts.Add(context);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(context);
        }

        //
        // GET: /Context/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Context context = db.Contexts.Find(id);
            if (context == null)
            {
                return HttpNotFound();
            }
            return View(context);
        }

        //
        // POST: /Context/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Context context)
        {
            if (ModelState.IsValid)
            {
                db.Entry(context).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(context);
        }

        //
        // GET: /Context/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Context context = db.Contexts.Find(id);
            if (context == null)
            {
                return HttpNotFound();
            }
            return View(context);
        }

        //
        // POST: /Context/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Context context = db.Contexts.Find(id);
            db.Contexts.Remove(context);
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