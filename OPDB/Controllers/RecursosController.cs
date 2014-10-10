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
            var resources = db.Resources.Include(r => r.Unit);
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
                resource.CreateDate = DateTime.Now;
                resource.UpdateDate = DateTime.Now;
                db.Resources.Add(resource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UnitID = new SelectList(db.Units, "UnitID", "UnitName", resource.UnitID);
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
                resource.UpdateDate = DateTime.Now;
                db.Entry(resource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UnitID = new SelectList(db.Units, "UnitID", "UnitName", resource.UnitID);
            return View(resource);
        }

        //
        // GET: /Recursos/Delete/5

        [HttpPost]
        public ActionResult Remover(int id = 0)
        {
            Resource resource = db.Resources.Find(id);
            if (resource == null)
            {
                return HttpNotFound();
            }
            else
            {
                resource.DeletionDate = DateTime.Now;
                db.Entry(resource).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //
        // POST: /Recursos/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Resource resource = db.Resources.Find(id);
        //    db.Resources.Remove(resource);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}