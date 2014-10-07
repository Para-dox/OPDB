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
    public class RecursosController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Recursos/

        public ActionResult Index()
        {
            var resources = db.Resources.Include(r => r.Unit).Include(r => r.User).Include(r => r.User1);
            return View(resources.ToList());
        }

        //
        // GET: /Recursos/Details/5

        public ActionResult Detalles(int id = 0)
        {
            Resource resource = db.Resources.Find(id);
            if (resource == null)
            {
                return HttpNotFound();
            }
            return View(resource);
        }

        //
        // GET: /Recursos/Create

        public ActionResult Crear()
        {
            ViewBag.UnitID = new SelectList(db.Units, "UnitID", "UnitName");
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword");
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword");
            return View();
        }

        //
        // POST: /Recursos/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Resource resource)
        {
            if (ModelState.IsValid)
            {
                db.Resources.Add(resource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UnitID = new SelectList(db.Units, "UnitID", "UnitName", resource.UnitID);
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword", resource.CreateUser);
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword", resource.UpdateUser);
            return View(resource);
        }

        //
        // GET: /Recursos/Edit/5

        public ActionResult Editar(int id = 0)
        {
            Resource resource = db.Resources.Find(id);
            if (resource == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnitID = new SelectList(db.Units, "UnitID", "UnitName", resource.UnitID);
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword", resource.CreateUser);
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword", resource.UpdateUser);
            return View(resource);
        }

        //
        // POST: /Recursos/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Resource resource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(resource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UnitID = new SelectList(db.Units, "UnitID", "UnitName", resource.UnitID);
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword", resource.CreateUser);
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword", resource.UpdateUser);
            return View(resource);
        }

        //
        // GET: /Recursos/Delete/5

        public ActionResult Remover(int id = 0)
        {
            Resource resource = db.Resources.Find(id);
            if (resource == null)
            {
                return HttpNotFound();
            }
            return View(resource);
        }

        //
        // POST: /Recursos/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Resource resource = db.Resources.Find(id);
            db.Resources.Remove(resource);
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