using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPDB.Models;

namespace OPDB.Controllers
{
    public class ActividadesController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Actividades/

        public ActionResult Index()
        {
            var activities = db.Activities.Include(a => a.ActivityType).Include(a => a.School).Include(a => a.User).Include(a => a.User1).Include(a => a.User2);
            return View(activities.ToList());
        }

        //
        // GET: /Actividades/Details/5

        public ActionResult Detalles(int id = 0)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        //
        // GET: /Actividades/Create

        public ActionResult Crear()
        {
            ViewBag.ActivityTypeID = new SelectList(db.ActivityTypes, "ActivityTypeID", "ActivityType1");
            ViewBag.SchoolID = new SelectList(db.Schools, "SchoolID", "SchoolName");
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword");
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserPassword");
            return View();
        }

        //
        // POST: /Actividades/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActivityTypeID = new SelectList(db.ActivityTypes, "ActivityTypeID", "ActivityType1", activity.ActivityTypeID);
            ViewBag.SchoolID = new SelectList(db.Schools, "SchoolID", "SchoolName", activity.SchoolID);
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword", activity.CreateUser);
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword", activity.UpdateUser);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserPassword", activity.UserID);
            return View(activity);
        }

        //
        // GET: /Actividades/Edit/5

        public ActionResult Editar(int id = 0)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityTypeID = new SelectList(db.ActivityTypes, "ActivityTypeID", "ActivityType1", activity.ActivityTypeID);
            ViewBag.SchoolID = new SelectList(db.Schools, "SchoolID", "SchoolName", activity.SchoolID);
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword", activity.CreateUser);
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword", activity.UpdateUser);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserPassword", activity.UserID);
            return View(activity);
        }

        //
        // POST: /Actividades/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActivityTypeID = new SelectList(db.ActivityTypes, "ActivityTypeID", "ActivityType1", activity.ActivityTypeID);
            ViewBag.SchoolID = new SelectList(db.Schools, "SchoolID", "SchoolName", activity.SchoolID);
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword", activity.CreateUser);
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword", activity.UpdateUser);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserPassword", activity.UserID);
            return View(activity);
        }

        //
        // GET: /Actividades/Delete/5

        public ActionResult Remover(int id = 0)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        //
        // POST: /Actividades/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
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