using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;
using Microsoft.Ajax.Utilities;

namespace GTD.Controllers
{
    public class PomodoroesController : Controller
    {
        private IPomodoroRepository pomodoroRepository;

        private GTDContext db = new GTDContext();

        public PomodoroesController(IPomodoroRepository pomodoroRepository)
        {
            this.pomodoroRepository = pomodoroRepository;
        }

        public string AddPomodoro(Pomodoro pomodoro)
        {
            if (ModelState.IsValid)
            {
                db.Pomodoroes.Add(pomodoro);
                db.SaveChanges();
                return "success";
            }
            return "fail";
        }

        // GET: Pomodoroes
        public ActionResult Index()
        {
            var pomodoroes = pomodoroRepository.GetAll().Include(p => p.Task);//db.Pomodoroes.Include(p => p.Task);
            return View(pomodoroes.ToList());
        }

        // GET: Pomodoroes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pomodoro pomodoro = db.Pomodoroes.Find(id);
            if (pomodoro == null)
            {
                return HttpNotFound();
            }
            return View(pomodoro);
        }

        // GET: Pomodoroes/Create
        public ActionResult Create()
        {
            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline");
            return View();
        }

        // POST: Pomodoroes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PomodoroId,IsCompletedPomodoro,StarDateTime,EnDateTime,IsWorkingTime,TaskId")] Pomodoro pomodoro)
        {
            if (ModelState.IsValid)
            {
                db.Pomodoroes.Add(pomodoro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline", pomodoro.TaskId);
            return View(pomodoro);
        }

        // GET: Pomodoroes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pomodoro pomodoro = db.Pomodoroes.Find(id);
            if (pomodoro == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline", pomodoro.TaskId);
            return View(pomodoro);
        }

        // POST: Pomodoroes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PomodoroId,IsCompletedPomodoro,StarDateTime,EnDateTime,IsWorkingTime,TaskId")] Pomodoro pomodoro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pomodoro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TaskId = new SelectList(db.Tasks, "TaskId", "Headline", pomodoro.TaskId);
            return View(pomodoro);
        }

        // GET: Pomodoroes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pomodoro pomodoro = db.Pomodoroes.Find(id);
            if (pomodoro == null)
            {
                return HttpNotFound();
            }
            return View(pomodoro);
        }

        // POST: Pomodoroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pomodoro pomodoro = db.Pomodoroes.Find(id);
            db.Pomodoroes.Remove(pomodoro);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
