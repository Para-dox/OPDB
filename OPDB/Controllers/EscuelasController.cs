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

        public ActionResult Detalles(int id = 0)
        {
            SchoolViewModel schoolViewModel = new SchoolViewModel
            {
                school = db.Schools.Find(id),
                Notes = from note in db.SchoolNotes.Include(note => note.NoteType) where note.SchoolID == id && note.DeletionDate == null select note
            };

            if (schoolViewModel.school == null)
            {
                return HttpNotFound();
            }
            return View(schoolViewModel);
        }

        //
        // GET: /Escuelas/Create

        public ActionResult Crear()
        {
            ViewBag.CreateUser = new SelectList(db.Users, "UserID", "UserPassword");
            ViewBag.UpdateUser = new SelectList(db.Users, "UserID", "UserPassword");
            return View();
        }

        //
        // POST: /Escuelas/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(School school)
        {
            if (ModelState.IsValid)
            {
                school.UpdateDate = DateTime.Now;
                school.CreateDate = DateTime.Now;

                school.CreateUser = 2;
                school.UpdateUser = 2;

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

        public ActionResult Editar(int id = 0)
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
        public ActionResult Editar(School school)
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

        public ActionResult Remover(int id = 0)
        {
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        public ActionResult RemoverNota(int id)
        {
            var note = db.SchoolNotes.Find(id);
            note.DeletionDate = DateTime.Now;
            db.Entry(note).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction ("Detalles", "Escuelas", note.SchoolID);
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

        [HttpPost]
        public ActionResult GuardarNota(SchoolViewModel schoolViewModel)
        {
            schoolViewModel.note.CreateDate = DateTime.Now;
            schoolViewModel.note.UpdateDate = DateTime.Now;
            schoolViewModel.note.SchoolID = schoolViewModel.school.SchoolID;

            schoolViewModel.note.CreateUser = 2;
            schoolViewModel.note.UpdateUser = 2;
            schoolViewModel.note.UserID = 2;

            db.SchoolNotes.Add(schoolViewModel.note);
            db.SaveChanges();

            //schoolViewModel.school = db.Schools.Find(schoolViewModel.school.SchoolID);
            //schoolViewModel.Notes = from n in db.SchoolNotes where n.SchoolID == schoolViewModel.school.SchoolID select n;

            return RedirectToAction("Detalles", "Escuelas", schoolViewModel.school.SchoolID);
        }

        public ActionResult CrearNota(int id)
        {
            SchoolViewModel schoolViewModel = new SchoolViewModel
            {

                NoteTypes = GetNoteTypes(),
                school = new School
                {

                    SchoolID = id
                }
            };

            return View(schoolViewModel);
        }

        private List<SelectListItem> GetNoteTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();

            foreach (var type in db.NoteTypes)
            {
                types.Add(new SelectListItem()
                {

                    Text = type.NoteType1, 
                    Value = type.NoteTypeID + ""

                });
            }

            return types;
        }
    }
}