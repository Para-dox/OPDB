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
            var users = from u in db.Users.Include(u => u.UserDetails) where u.UserTypeID == 3 && u.DeletionDate == null select u;
            return PartialView("Index", users.ToList());
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
            UserViewModel userViewModel = new UserViewModel
            {
                userTypes = getTypes(),
                outreachTypes = getOutreachTypes()

            };

            return View(userViewModel);
        }

        //[HttpPost]
        //public ActionResult Crear(UserViewModel userViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        userViewModel.user.CreateDate = DateTime.Now;
        //        userViewModel.user.UpdateDate = DateTime.Now;
        //        userViewModel.user.UserStatus = false;

        //        if (userViewModel.user.UserTypeID == 3)
        //        {
        //            userViewModel.outreachEntity.CreateDate = DateTime.Now;
        //            userViewModel.outreachEntity.UpdateDate = DateTime.Now;
        //            userViewModel.user.OutreachEntityDetails = new List<OutreachEntityDetail>();
        //            userViewModel.user.OutreachEntityDetails.Add(userViewModel.outreachEntity);
        //            db.Users.Add(userViewModel.user);
        //        }

        //        else
        //        {
        //            userViewModel.userDetail.CreateDate = DateTime.Now;
        //            userViewModel.userDetail.UpdateDate = DateTime.Now;
        //            userViewModel.user.UserDetails = new List<UserDetail>();
        //            userViewModel.user.UserDetails.Add(userViewModel.userDetail);
        //            db.Users.Add(userViewModel.user);
        //        }

        //        db.SaveChanges();

        //        return RedirectToAction("Administracion", "Home", null);
        //    }

        //    userViewModel.userTypes = getUserTypes();
        //    userViewModel.outreachTypes = getOutreachTypes();
        //    return View(userViewModel);
        //}

        //
        // GET: /Alcance/Edit/5

        public ActionResult Editar(int id = 0)
        {
            var outreach = db.OutreachEntityDetails.Find(id);

            UserViewModel outreachViewModel = new UserViewModel
            {
                user = db.Users.Find(outreach.UserID),
                outreachEntity = outreach,
                outreachTypes = getOutreachTypes()
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
        public ActionResult Editar(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                userViewModel.user.UpdateDate = DateTime.Now;
                userViewModel.outreachEntity.UpdateDate = DateTime.Now;
                db.Entry(userViewModel.user).State = EntityState.Modified;
                db.Entry(userViewModel.outreachEntity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            userViewModel.outreachTypes = getOutreachTypes();
            return View(userViewModel);
        }

        //
        // GET: /Alcance/Delete/5

        public ActionResult Remover(int id = 0)
        {
            OutreachEntityDetail outreachDetail = db.OutreachEntityDetails.Find(id);
            User outreach = db.Users.FirstOrDefault(i => i.UserID == outreachDetail.UserID);
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
        public ActionResult GuardarNota(UserViewModel userViewModel)
        {
            try
            {
                userViewModel.note.UpdateUser = 2;
                userViewModel.note.UpdateDate = DateTime.Now;

                if (userViewModel.note.UserNoteID == 0)
                {
                    if (ModelState.IsValid)
                    {
                        userViewModel.note.CreateDate = DateTime.Now;

                        userViewModel.note.UserID = 2;
                        userViewModel.note.CreateUser = 2;

                        db.UserNotes.Add(userViewModel.note);
                        db.SaveChanges();

                        return View("_Hack");
                    }

                    return Content(GetErrorsFromModelState(userViewModel));
                }
                else if (ModelState.IsValid)
                {

                    db.Entry(userViewModel.note).State = EntityState.Modified;
                    db.SaveChanges();
                    return View("_Hack");

                }
            }
            catch(Exception e) { return Content(e.ToString()); }
            return Content(GetErrorsFromModelState(userViewModel));
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

        [HttpPost]
        public ActionResult VerNota(int id)
        {
            UserNote userNote = db.UserNotes.Find(id);
            userNote.NoteType = db.NoteTypes.Find(userNote.NoteTypeID);


            UserViewModel userViewModel = new UserViewModel
            {
                note = userNote,
                outreachEntity = db.OutreachEntityDetails.First(user => user.UserID == userNote.SubjectID)
            };


            return PartialView("VerNota", userViewModel);
        }

        public ActionResult Lista()
        {
            var users = (from outreachEntity in db.OutreachEntityDetails join user in db.Users on outreachEntity.UserID equals user.UserID where (user.UserTypeID == 3) && outreachEntity.DeletionDate == null orderby outreachEntity.OutreachEntityName ascending select outreachEntity).ToList();

            UserViewModel userViewModel = new UserViewModel
            {
                Information = new List<UserInfoViewModel>()
            };

            foreach (var outreach in users)
            {
                userViewModel.Information.Add(new UserInfoViewModel
                {
                    User = db.Users.Find(outreach.UserID),
                    OutreachEntity = outreach
                });
            }

            return View(userViewModel);
        }

        public ActionResult Removidos()
        {
            var users = from o in db.OutreachEntityDetails.Include(o => o.OutreachEntityType) where o.DeletionDate != null select o;

            return PartialView("Removidos", users.ToList());

        }

        public String GetErrorsFromModelState(UserViewModel userViewModel)
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

        public List<SelectListItem> getTypes()
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