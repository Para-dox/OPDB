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
    public class AlcanceController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Alcance/

        public ActionResult Index()
        {
            var outreachentitydetails = db.OutreachEntityDetails.Include(o => o.OutreachEntityType).Include(o => o.User);
            return View(outreachentitydetails.ToList());
        }

        //
        // GET: /Alcance/Details/5

        public ActionResult Details(int id = 0)
        {
            OutreachEntityDetail outreachentitydetail = db.OutreachEntityDetails.Find(id);
            if (outreachentitydetail == null)
            {
                return HttpNotFound();
            }
            return View(outreachentitydetail);
        }

        //
        // GET: /Alcance/Create

        public ActionResult Create()
        {
            ViewBag.OutreachEntityTypeID = new SelectList(db.OutreachEntityTypes, "OutreachEntityTypeID", "OureachEntityType");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserPassword");
            return View();
        }

        //
        // POST: /Alcance/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OutreachEntityDetail outreachentitydetail)
        {
            if (ModelState.IsValid)
            {
                db.OutreachEntityDetails.Add(outreachentitydetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OutreachEntityTypeID = new SelectList(db.OutreachEntityTypes, "OutreachEntityTypeID", "OureachEntityType", outreachentitydetail.OutreachEntityTypeID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserPassword", outreachentitydetail.UserID);
            return View(outreachentitydetail);
        }

        //
        // GET: /Alcance/Edit/5

        public ActionResult Edit(int id = 0)
        {
            OutreachEntityDetail outreachentitydetail = db.OutreachEntityDetails.Find(id);
            if (outreachentitydetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.OutreachEntityTypeID = new SelectList(db.OutreachEntityTypes, "OutreachEntityTypeID", "OureachEntityType", outreachentitydetail.OutreachEntityTypeID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserPassword", outreachentitydetail.UserID);
            return View(outreachentitydetail);
        }

        //
        // POST: /Alcance/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OutreachEntityDetail outreachentitydetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(outreachentitydetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OutreachEntityTypeID = new SelectList(db.OutreachEntityTypes, "OutreachEntityTypeID", "OureachEntityType", outreachentitydetail.OutreachEntityTypeID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserPassword", outreachentitydetail.UserID);
            return View(outreachentitydetail);
        }

        //
        // GET: /Alcance/Delete/5

        public ActionResult Delete(int id = 0)
        {
            OutreachEntityDetail outreachentitydetail = db.OutreachEntityDetails.Find(id);
            if (outreachentitydetail == null)
            {
                return HttpNotFound();
            }
            return View(outreachentitydetail);
        }

        //
        // POST: /Alcance/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OutreachEntityDetail outreachentitydetail = db.OutreachEntityDetails.Find(id);
            db.OutreachEntityDetails.Remove(outreachentitydetail);
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