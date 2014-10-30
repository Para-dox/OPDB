using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
            var users = from u in db.Users.Include(u => u.UserType).Include(u => u.UserDetails) where u.UserTypeID != 3 && u.DeletionDate == null select u;
            return PartialView("Index", users.ToList());
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

                userTypes = getTypes(),
                outreachTypes = getOutreachTypes()
                        
            };            

            return View(user);
        }

        //
        // POST: /Usuarios/Create

        [HttpPost]
        public ActionResult Crear(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                    userViewModel.user.CreateDate = DateTime.Now;
                    userViewModel.user.UpdateDate = DateTime.Now;
                    userViewModel.user.UserStatus = false;
                    bool validModel = true;

                    if (userViewModel.user.UserTypeID == 3) {

                        if (userViewModel.outreachEntity.OutreachEntityName == null)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_OutreachEntityName_Required", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_Required);
                            validModel = false;
                        }

                        if (userViewModel.outreachEntity.Mission == null)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_Mission_Required", Resources.WebResources.OutreachEntityDetail_Mission_Required);
                            validModel = false;
                        }

                        if (userViewModel.outreachEntity.Vision == null)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_Vision_Required", Resources.WebResources.OutreachEntityDetail_Vision_Required);
                            validModel = false;
                        }

                        if (userViewModel.outreachEntity.Objectives == null)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_Objectives_Required", Resources.WebResources.OutreachEntityDetail_Objectives_Required);
                            validModel = false;
                        }

                        if (validModel) 
                        { 
                            userViewModel.outreachEntity.CreateDate = DateTime.Now;
                            userViewModel.outreachEntity.UpdateDate = DateTime.Now;
                            userViewModel.user.OutreachEntityDetails = new List<OutreachEntityDetail>();
                            userViewModel.user.OutreachEntityDetails.Add(userViewModel.outreachEntity);
                            db.Users.Add(userViewModel.user);
                        }

                        else
                        {
                            userViewModel.userTypes = getTypes();
                            userViewModel.outreachTypes = getOutreachTypes();
                            return View(userViewModel);
                        }
                    }

                    else
                    {

                        if (userViewModel.userDetail.FirstName == null)
                        {
                            ModelState.AddModelError("UserDetail_FirstName_Required", Resources.WebResources.UserDetail_FirstName_Required);
                            validModel = false;
                        }

                        if (userViewModel.userDetail.LastName == null)
                        {
                            ModelState.AddModelError("UserDetail_LastName_Required", Resources.WebResources.UserDetail_LastName_Required);
                            validModel = false;
                        }

                        if (userViewModel.userDetail.Gender == null)
                        {
                            ModelState.AddModelError("UserDetail_Gender_Required", Resources.WebResources.UserDetail_Gender_Required);
                            validModel = false;
                        }

                        if (validModel) 
                        { 
                            userViewModel.userDetail.CreateDate = DateTime.Now;
                            userViewModel.userDetail.UpdateDate = DateTime.Now;
                            userViewModel.user.UserDetails = new List<UserDetail>();
                            userViewModel.user.UserDetails.Add(userViewModel.userDetail);
                            db.Users.Add(userViewModel.user);
                        }

                        else
                        {
                            userViewModel.userTypes = getTypes();
                            userViewModel.outreachTypes = getOutreachTypes();
                            return View(userViewModel);
                        }
                    }

                    db.SaveChanges();

                    return RedirectToAction("Administracion", "Home", null);
                    
            }

            userViewModel.userTypes = getTypes();
            userViewModel.outreachTypes = getOutreachTypes();
            return View(userViewModel);
        }

        //
        // GET: /Usuarios/Edit/5

        public ActionResult Editar(int id = 0)
        {
            //User ;
            UserViewModel userViewModel = new UserViewModel
            {
                user = db.Users.Find(id),
                userDetail = db.UserDetails.FirstOrDefault(i => i.UserID == id),
                userTypes = getUserTypes()
            };

            if (userViewModel.user == null)
            {
                return HttpNotFound();
            }


            return View(userViewModel);
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
                return RedirectToAction("Administracion", "Home", null);
            }
            userViewModel.userTypes = getTypes();
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
            return RedirectToAction("Administracion", "Home", null);
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

                if (userType.UserTypeID > 3)
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

        [HttpPost]
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

            return PartialView("CrearNota", userViewModel);
        }


        [HttpPost]
        public ActionResult GuardarNota(UserViewModel userViewModel)
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
            else if(ModelState.IsValid)
            {

                db.Entry(userViewModel.note).State = EntityState.Modified;
                db.SaveChanges();
                return View("_Hack");                
                
            }

            return Content(GetErrorsFromModelState(userViewModel));
        }

        [HttpPost]
        public ActionResult EditarNota(int id)
        {
            EscuelasController controller = new EscuelasController();

            UserViewModel userViewModel = new UserViewModel
            {

                NoteTypes = controller.getNoteTypes(),
                note = db.UserNotes.Find(id)
            };

            return PartialView("EditarNota", userViewModel);
        }


        [HttpPost]
        public ActionResult VerNota(int id)
        {
            UserNote userNote = db.UserNotes.Find(id);
            userNote.NoteType = db.NoteTypes.Find(userNote.NoteTypeID);
            

            UserViewModel userViewModel = new UserViewModel
            {
                note = userNote,
                userDetail = db.UserDetails.First(user => user.UserID == userNote.SubjectID)
            };

            
            return PartialView("VerNota", userViewModel);
        }

        public ActionResult Lista()
        {
            var users = (from u in db.Users where u.UserTypeID != 3 && u.DeletionDate == null select u).ToList();

            UserViewModel userViewModel = new UserViewModel
            {
                Information = new List<UserInfoViewModel>()
            };

            foreach (var user in users)
            {
                userViewModel.Information.Add(new UserInfoViewModel
                {
                    User = user,
                    UserDetail = db.UserDetails.First(ud => ud.UserID == user.UserID)
                });
            }

            return View(userViewModel);

        }

        public ActionResult Removidos()
        {
            var users = from u in db.Users.Include(u => u.UserType).Include(u => u.UserDetails) where (u.UserTypeID != 3) && u.DeletionDate != null select u;

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

        public ActionResult IniciarSesion()
        {
            return View();
        }

        public ActionResult IniciarSesion(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                if (LoginValid(userViewModel.user.Email, userViewModel.user.UserPassword))
                {
                    FormsAuthentication.SetAuthCookie(userViewModel.user.Email, false);
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    // TODO: needs a validation message here
                    ModelState.AddModelError("", "Invalid Login");
                }
            }

            return View();
        }

        public ActionResult Registrar()
        {
            return View();
        }

        public ActionResult Registrar(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                // TODO: create here new user

                // add the pass, email, etc., and store it in the DB

                // db.SaveChanges etc...

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // TODO: add validation messages here
                ModelState.AddModelError("", "Registration Info is not valid");
            }

            return View();
        }

        private bool LoginValid(string email, string password)
        {
            bool isValid = false;
            User user = db.Users.FirstOrDefault(u => u.Email == email);

            // TODO: passwords have no encryption at all - fix later with SimpleCrypto NuGet Package

            if (user != null)
            {
                if (user.UserPassword.ToString().Equals(password))
                {
                    isValid = true;
                }
            }

            return isValid;
        }
        
       
        public ActionResult CerrarSesion()
        {
            return View();
        }
    }
}