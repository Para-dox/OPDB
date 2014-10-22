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
            var outreach = db.OutreachEntityDetails.Find(id);

            UserViewModel outreachViewModel = new UserViewModel
            {
                outreachEntity = outreach,
                Notes = from note in db.UserNotes.Include(note => note.NoteType) where note.SubjectID == outreach.UserID && note.DeletionDate == null select note
            };

            if (outreachViewModel.outreachEntity == null)
            {
                return HttpNotFound();
            }
            return View(outreachViewModel);
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

            UserViewModel outreachViewModel = new UserViewModel
            {
                outreachEntity = db.OutreachEntityDetails.Find(id)
            };

            if (outreachViewModel.outreachEntity == null)
            {
                return HttpNotFound();
            }
            return View(outreachViewModel);
        }

        //
        // POST: /Alcance/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(UserViewModel outreachentitydetail)
        {
            if (ModelState.IsValid)
            {
                outreachentitydetail.user.UpdateDate = DateTime.Now;
                outreachentitydetail.outreachEntity.UpdateDate = DateTime.Now;
                db.Entry(outreachentitydetail.user).State = EntityState.Modified;
                db.Entry(outreachentitydetail.outreachEntity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            outreachentitydetail.userTypes = getUserTypes();
            return View(outreachentitydetail);
        }

        //
        // GET: /Alcance/Delete/5

        public ActionResult Remover(int id = 0)
        {
            User outreach = db.Users.Find(id);
            OutreachEntityDetail outreachDetail = db.OutreachEntityDetails.FirstOrDefault(i => i.UserID == id);
            if (outreach == null)
            {
                return HttpNotFound();
            }
            else
            {
                outreach.DeletionDate = DateTime.Now;
                outreachDetail.DeletionDate = DateTime.Now;
                db.Entry(outreach).State = EntityState.Modified;
                db.Entry(outreachDetail).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult RemoverNota(int id)
        {
            var note = db.UserNotes.Find(id);
            note.DeletionDate = DateTime.Now;
            db.Entry(note).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Detalles", "Alcance", new { id = note.SubjectID });
        }

        [HttpPost]
        public ActionResult CrearNota(int id)
        {
            EscuelasController controller = new EscuelasController();

            UserViewModel outreachViewModel = new UserViewModel
             {

                 NoteTypes = controller.getNoteTypes(),
                 note = new UserNote
                 {

                    SubjectID = id
                 }
             };

             return PartialView("CrearNota", outreachViewModel);
        }

        [HttpPost]
        public ActionResult GuardarNota(UserViewModel outreachViewModel)
        {

            outreachViewModel.note.UpdateUser = 2;
            outreachViewModel.note.UpdateDate = DateTime.Now;

            if (outreachViewModel.note.UserNoteID == 0)
            {
                if (ModelState.IsValid)
                {
                    outreachViewModel.note.CreateDate = DateTime.Now;

                    outreachViewModel.note.UserID = 2;
                    outreachViewModel.note.CreateUser = 2;

                    db.UserNotes.Add(outreachViewModel.note);
                    db.SaveChanges();

                    return View("_Hack");
                }

                return Content(GetErrorsFromModelState(outreachViewModel));
            }
            else if (ModelState.IsValid)
            {

                db.Entry(outreachViewModel.note).State = EntityState.Modified;
                db.SaveChanges();
                return View("_Hack");

            }

            return Content(GetErrorsFromModelState(outreachViewModel));
        }

        public ActionResult EditarNota(int id)
        {
            EscuelasController controller = new EscuelasController();

            UserViewModel outreachViewModel = new UserViewModel
            {

                NoteTypes = controller.getNoteTypes(),
                note = db.UserNotes.Find(id)
            };

            return PartialView("EditarNota", outreachViewModel);
        }

        public ActionResult Lista()
        {
            var users = from o in db.OutreachEntityDetails.Include(o => o.OutreachEntityType) where o.DeletionDate == null select o;

            return View(users.ToList());

        }

        public ActionResult Removidos()
        {
            var users = from o in db.OutreachEntityDetails.Include(o => o.OutreachEntityType) where o.DeletionDate != null select o;

            return PartialView("Removidos", users.ToList());

        }

        public String GetErrorsFromModelState(UserViewModel outreachViewModel)
        {


            //retrieves the validation messages from the ModelState as strings    
            var str = "";
            var errorSates = from state in ModelState.Values
                             from error in state.Errors
                             select error.ErrorMessage;

            var errorList = errorSates.ToList();
            foreach (var m in errorList)
            {
                str = str + "<li>* " + m + "</li>";
            }

            return str;
        }

        public List<SelectListItem> getOutreachTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();
            foreach (var outreachType in db.OutreachEntityTypes)
            {

                types.Add(new SelectListItem()
                {
                    Text = outreachType.OutreachEntityType1,
                    Value = outreachType.OutreachEntityTypeID + ""

                });

            }

            return types;
        }

        public List<SelectListItem> getUserTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();
            foreach (var userType in db.UserTypes)
            {

                if (userType.UserTypeID > 2)
                {
                    types.Add(new SelectListItem()
                    {
                        Text = userType.UserType1,
                        Value = userType.UserTypeID + ""

                    });
                }
            }

            return types;

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}