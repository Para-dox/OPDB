using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPDB.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace OPDB.Controllers
{
    public class UsuariosController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Usuarios/

        public ActionResult Index()
        {
            var users = from u in db.Users.Include(u => u.UserType).Include(u => u.UserDetails) where u.UserTypeID != 3 select u;
            //var users = from u in db.Users.Include(u => u.UserType).Include(u => u.UserDetails) where u.UserTypeID != 3 && u.DeletionDate == null select u;
            return View(users.ToList());
        }

        //
        // GET: /Usuarios/Details/5

        public ActionResult Detalles(int id = 0)
        {

            UserViewModel user = new UserViewModel
            {
                user = db.Users.Find(id),
                userDetail = (from ud in db.UserDetails where ud.UserID == id select ud).SingleOrDefault(),
                Notes = from note in db.UserNotes.Include(note => note.NoteType) where note.SubjectID == id && note.DeletionDate == null select note

            };
            
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /Usuarios/Create

        public ActionResult Crear()
        {
           
            UserViewModel user = new UserViewModel{
                
                userTypes = getUserTypes(),
                outreachTypes = getOutreachTypes()
                        
            };            

            return View(user);
        }

        //
        // POST: /Usuarios/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    userViewModel.user.CreateDate = DateTime.Now;
                    userViewModel.user.UpdateDate = DateTime.Now;
                    userViewModel.user.UserStatus = false;

                    if (userViewModel.user.UserTypeID == 3) {
                        
                        userViewModel.outreachEntity.CreateDate = DateTime.Now;
                        userViewModel.outreachEntity.UpdateDate = DateTime.Now;
                        userViewModel.user.OutreachEntityDetails = new List<OutreachEntityDetail>();
                        userViewModel.user.OutreachEntityDetails.Add(userViewModel.outreachEntity);
                        db.Users.Add(userViewModel.user);
                    }

                    else
                    {
                        
                        userViewModel.userDetail.CreateDate = DateTime.Now;
                        userViewModel.userDetail.UpdateDate = DateTime.Now;
                        userViewModel.user.UserDetails = new List<UserDetail>();
                        userViewModel.user.UserDetails.Add(userViewModel.userDetail);
                        db.Users.Add(userViewModel.user);

                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }

            userViewModel.userTypes = getUserTypes();
            userViewModel.outreachTypes = getOutreachTypes();
            return View(userViewModel);
        }

        //
        // GET: /Usuarios/Edit/5

        public ActionResult Editar(int id = 0)
        {
            //User ;
            UserViewModel user = new UserViewModel
            {
                user = db.Users.Find(id),
            };

            if (user == null)
            {
                return HttpNotFound();
            }

            else if (user.user.UserTypeID == 3)
            {
                user.outreachEntity = db.OutreachEntityDetails.FirstOrDefault(i => i.UserID == id);
                user.outreachTypes = getOutreachTypes();

            }
            else {

                user.userDetail = db.UserDetails.FirstOrDefault(i => i.UserID == id);
            }

            //ViewBag.UserTypeID = new SelectList(db.UserTypes, "UserTypeID", "UserType1", user.UserTypeID);
            return View(user);
        }

        //
        // POST: /Usuarios/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                userViewModel.user.UpdateDate = DateTime.Now;
                userViewModel.userDetail.UpdateDate = DateTime.Now;
                db.Entry(userViewModel.user).State = EntityState.Modified;
                db.Entry(userViewModel.userDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            userViewModel.userTypes = getUserTypes();
            return View(userViewModel);
        }

        //
        // GET: /Usuarios/Delete/5

        [HttpPost]
        public ActionResult Remover(int id = 0)
        {
            User user = db.Users.Find(id);
            UserDetail userDetail = db.UserDetails.FirstOrDefault(i => i.UserID == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                user.DeletionDate = DateTime.Now;
                userDetail.DeletionDate = DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.Entry(userDetail).State = EntityState.Modified;
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
            return RedirectToAction("Detalles", "Usuarios", new { id = note.SubjectID });
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private List<SelectListItem> getUserTypes()
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

        private List<SelectListItem> getOutreachTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();
            foreach (var outreachType in db.OutreachEntityTypes)
            {

                types.Add(new SelectListItem()
                    {
                        Text = outreachType.OureachEntityType,
                        Value = outreachType.OutreachEntityTypeID + ""

                    });
                
            }

            return types;

        }

        public ActionResult CrearNota(int id)
        {
            EscuelasController controller = new EscuelasController();

            UserViewModel userViewModel = new UserViewModel
            {

                NoteTypes = controller.getNoteTypes(),
                note = new UserNote
                {

                    SubjectID = id
                }
            };

            return View(userViewModel);
        }


        [HttpPost]
        public ActionResult GuardarNota(UserViewModel userViewModel)
        {

            userViewModel.note.UpdateUser = 2;
            userViewModel.note.UpdateDate = DateTime.Now;

            if (userViewModel.note.UserNoteID == 0)
            {

                userViewModel.note.CreateDate = DateTime.Now;

                userViewModel.note.UserID = 2;
                userViewModel.note.CreateUser = 2;

                db.UserNotes.Add(userViewModel.note);

            }
            else
            {
                db.Entry(userViewModel.note).State = EntityState.Modified;
            }

            db.SaveChanges();

            return RedirectToAction("Detalles", "Usuarios", new { id = userViewModel.note.SubjectID });
        }


        public ActionResult EditarNota(int id)
        {
            EscuelasController controller = new EscuelasController();

            UserViewModel userViewModel = new UserViewModel
            {

                NoteTypes = controller.getNoteTypes(),
                note = db.UserNotes.Find(id)
            };

            return View(userViewModel);
        }

        public ActionResult Lista()
        {
            var users = from u in db.Users.Include(u => u.UserType).Include(u => u.UserDetails) where u.UserTypeID != 3 select u;
            
            return View(users.ToList());

        }

       
    }
}