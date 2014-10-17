﻿using System;
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

        /// <summary>
        /// This method is responsible for searching for the particular Notes associated with a particular school,
        /// by means of using its specific id (the parameter recevied by the method).
        /// </summary>
        /// 
        /// <param name="id">
        /// A unique integer that identifies each individual school.
        /// </param>
        /// 
        /// <returns> 
        /// Returns a schoolViewModel 
        /// </returns>

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

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// 
        /// </returns>

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
                school.UpdateUser = 2;
                school.UpdateDate = DateTime.Now;
                db.Entry(school).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
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
            return RedirectToAction("Detalles", "Escuelas", new { id = note.SchoolID });
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
            
            schoolViewModel.note.UpdateUser = 2;
            schoolViewModel.note.UpdateDate = DateTime.Now;

            if (schoolViewModel.note.SchoolNoteID == 0){  

                schoolViewModel.note.CreateDate = DateTime.Now;
                schoolViewModel.note.SchoolID = schoolViewModel.school.SchoolID;

                schoolViewModel.note.UserID = 2;
                schoolViewModel.note.CreateUser = 2;
               
                db.SchoolNotes.Add(schoolViewModel.note);

            }
            else
            {
                db.Entry(schoolViewModel.note).State = EntityState.Modified;
            }

            db.SaveChanges();

            return RedirectToAction("Detalles", "Escuelas", new { id = schoolViewModel.school.SchoolID});
        }

        
        public ActionResult CrearNota(int id)
        {
            SchoolViewModel schoolViewModel = new SchoolViewModel
            {

                NoteTypes = getNoteTypes(),
                school = new School
                {

                    SchoolID = id
                }
            };

            return View(schoolViewModel);
        }

        public ActionResult EditarNota(int id)
        {

            SchoolViewModel schoolViewModel = new SchoolViewModel
            {

                NoteTypes = getNoteTypes(),
                note = db.SchoolNotes.Find(id)
            };

            return View(schoolViewModel);
        }

        public List<SelectListItem> getNoteTypes()
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

        public ActionResult Lista()
        {
            var schools = db.Schools.Include(s => s.User).Include(s => s.User1);
            return View(schools.ToList());
        }
    }
}