using System.Linq;
using System.Web.Mvc;
using GTD.Models;
using GTD.Services.Abstract;

namespace GTD.Controllers
{
    public class GoalController : Controller
    {
        //private GTDContext db = new GTDContext();
        private readonly IGoalServices _goalServices;

        public GoalController(IGoalServices goalServices)
        {
            this._goalServices = goalServices;
        }

        //
        // GET: /Goal/

        public ActionResult Index()
        {
            //return View(db.Goals.ToList());
            return View(_goalServices.GetAllGoals().ToList());
        }

        //
        // GET: /Goal/Details/5

        public ActionResult Details(int id = 0)
        {
            //Goal goal = db.Goals.Find(id);
            Goal goal = _goalServices.GetGoalById(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

        //
        // GET: /Goal/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Goal/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Goal goal)
        {
            if (ModelState.IsValid)
            {
                //db.Goals.Add(goal);
                //db.SaveChanges();
                _goalServices.CreateGoal(goal);
                return RedirectToAction("Index");
            }

            return View(goal);
        }

        //
        // GET: /Goal/Edit/5

        public ActionResult Edit(int id = 0)
        {
            //Goal goal = db.Goals.Find(id);
            Goal goal = _goalServices.GetGoalById(id);

            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

        //
        // POST: /Goal/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Goal goal)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(goal).State = EntityState.Modified;
                //db.SaveChanges();
                _goalServices.UpdateGoal(goal);
                return RedirectToAction("Index");
            }
            return View(goal);
        }

        //
        // GET: /Goal/Delete/5

        public ActionResult Delete(int id = 0)
        {
            //Goal goal = db.Goals.Find(id);
            Goal goal = _goalServices.GetGoalById(id);

            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

        //
        // POST: /Goal/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Goal goal = db.Goals.Find(id);
            Goal goal = _goalServices.GetGoalById(id);

            //db.Goals.Remove(goal);
            //db.SaveChanges();
            _goalServices.DeleteGoal(goal);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}