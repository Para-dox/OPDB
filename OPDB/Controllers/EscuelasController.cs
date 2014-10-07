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
    public class EscuelasController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Escuelas/

        public ActionResult Index()
        {
            var schools = db.Schools.Include(s => s.User).Include(s => s.User1);
            return View(schools.ToList());
        }

        //
        // GET: /Escuelas/Details/5

        public ActionResult Details(int id = 0)
        {
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        //
        // GET: /Escuelas/Create

        public ActionResult Create()
        {
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword");
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword");
            return View();
        }

        //
        // POST: /Escuelas/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(School school)
        {
            if (ModelState.IsValid)
            {
                db.Schools.Add(school);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword", school.CreateUser);
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword", school.UpdateUser);
            return View(school);
        }

        //
        // GET: /Escuelas/Edit/5

        public ActionResult Edit(int id = 0)
        {
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword", school.CreateUser);
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword", school.UpdateUser);
            return View(school);
        }

        //
        // POST: /Escuelas/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(School school)
        {
            if (ModelState.IsValid)
            {
                db.Entry(school).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword", school.CreateUser);
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword", school.UpdateUser);
            return View(school);
        }

        //
        // GET: /Escuelas/Delete/5

        public ActionResult Delete(int id = 0)
        {
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        //
        // POST: /Escuelas/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            School school = db.Schools.Find(id);
            db.Schools.Remove(school);
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