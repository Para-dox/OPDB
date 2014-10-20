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
    public class UnidadesController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Unidades/

        public ActionResult Index()
        {
            var units = from u in db.Units.Include(u => u.User).Include(u => u.User1) where u.DeletionDate == null select u;
            return View(units.ToList());
        }

        //
        // GET: /Unidades/Details/5

        public ActionResult Detalles(int id = 0)
        {
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        //
        // GET: /Unidades/Create

        public ActionResult Crear()
        {
            Unit unit = new Unit { };

            return View(unit);
        }

        //
        // POST: /Unidades/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Unit unitModel)
        {
            if (ModelState.IsValid)
            {
                // TODO create logic that extracts user ID
                unitModel.CreateDate = DateTime.Now;
                unitModel.CreateUser = 1;
                unitModel.UpdateDate = DateTime.Now;
                unitModel.UpdateUser = 1;

                db.Units.Add(unitModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(unitModel);
        }

        //
        // GET: /Unidades/Edit/5

        public ActionResult Editar(int id = 0)
        {
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            //these are not needed?
            //ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword", unit.CreateUser);
            //ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword", unit.UpdateUser);
            return View(unit);
        }

        //
        // POST: /Unidades/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Unit unit)
        {
            if (ModelState.IsValid)
            {
                unit.UpdateDate = DateTime.Now;
                unit.UpdateUser = 1; // TODO get actual user
                db.Entry(unit).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(unit);
        }

        //
        // GET: /Unidades/Delete/5

        public ActionResult Remover(int id = 0)
        {
            Unit unit = db.Units.Find(id);

            if (unit == null)
            {
                return HttpNotFound();
            }
            else
            {
                unit.DeletionDate = DateTime.Now;
                db.Entry(unit).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Unidades/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Unit unit = db.Units.Find(id);
            db.Units.Remove(unit);
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