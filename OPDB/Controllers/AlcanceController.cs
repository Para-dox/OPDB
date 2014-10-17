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

        public ActionResult Detalles(int id = 0)
        {
            UserViewModel userViewModel = new UserViewModel
            {
                outreachEntity = db.OutreachEntityDetails.Find(id),
                Notes = from note in db.UserNotes.Include(note => note.NoteType) where note.UserID == id && note.DeletionDate == null select note
            };

            if (userViewModel.outreachEntity == null)
            {
                return HttpNotFound();
            }
            return View(userViewModel);
        }

        //
        // GET: /Alcance/Create

        public ActionResult Crear()
        {
            ViewBag.OutreachEntityTypeID = new SelectList(db.OutreachEntityTypes, "OutreachEntityTypeID", "OureachEntityType");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserPassword");
            return View();
        }

        //
        // POST: /Alcance/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(OutreachEntityDetail outreachentitydetail)
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

        public ActionResult Editar(int id = 0)
        {
            OutreachEntityDetail outreachentitydetail = db.OutreachEntityDetails.FirstOrDefault(i => i.UserID == id); 
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
        public ActionResult Editar(OutreachEntityDetail outreachentitydetail)
        {
            if (ModelState.IsValid)
            {
                outreachentitydetail.UpdateDate = DateTime.Now;
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

        public ActionResult Remover(int id = 0)
        {
            OutreachEntityDetail outreachentitydetail = db.OutreachEntityDetails.Find(id);
            if (outreachentitydetail == null)
            {
                return HttpNotFound();
            }
            return View(outreachentitydetail);
        }
               

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}