﻿using System;
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
using System.Text.RegularExpressions;

namespace OPDB.Controllers
{
    public class UsuariosController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Usuarios/

        public ActionResult Index()
        {
            if (!Request.IsAuthenticated || (Int32.Parse(User.Identity.Name.ToString().Substring(User.Identity.Name.ToString().LastIndexOf('=') + 1).Trim()) != 1))
            {
                return RedirectToAction("AccesoDenegado", "Home", null);
            }

            var users = from u in db.Users.Include(u => u.UserType).Include(u => u.UserDetails) where u.UserTypeID != 3 && u.DeletionDate == null select u;
            return PartialView("Index", users.ToList());
        }

        public ActionResult MenuUsuarios()
        {
            return PartialView("Usuarios");
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

                User matchingUser = db.Users.FirstOrDefault(u => u.Email == userViewModel.user.Email);

                if (matchingUser != null)
                {
                    ModelState.AddModelError("User_Email_Exists", Resources.WebResources.User_Email_Exists);
                    validModel = false;
                }

                if (userViewModel.user.UserTypeID == 3)
                {

                    if (userViewModel.outreachEntity.OutreachEntityName == null || userViewModel.outreachEntity.OutreachEntityName == "")
                    {
                        ModelState.AddModelError("OutreachEntityDetail_OutreachEntityName_Required", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,100}$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.outreachEntity.OutreachEntityName);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("OutreachEntity_OutreachEntityName_Invalid", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_Invalid);
                            validModel = false;
                        }

                    }

                    if (userViewModel.outreachEntity.Mission == null || userViewModel.outreachEntity.Mission == "")
                    {
                        ModelState.AddModelError("OutreachEntityDetail_Mission_Required", Resources.WebResources.OutreachEntityDetail_Mission_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^[a-zA-Z\u00c0-\u017e¿\?.,;:¡!()""''-'\s]+$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.outreachEntity.Mission);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_Mission_Invalid", Resources.WebResources.OutreachEntityDetail_Mission_Invalid);
                            validModel = false;
                        }

                    }

                    if (userViewModel.outreachEntity.Vision == null || userViewModel.outreachEntity.Vision == "")
                    {
                        ModelState.AddModelError("OutreachEntityDetail_Vision_Required", Resources.WebResources.OutreachEntityDetail_Vision_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^[a-zA-Z\u00c0-\u017e¿\?.,;:¡!()""''-'\s]+$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.outreachEntity.Vision);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_Vision_Invalid", Resources.WebResources.OutreachEntityDetail_Vision_Invalid);
                            validModel = false;
                        }

                    }

                    if (userViewModel.outreachEntity.Objectives == null || userViewModel.outreachEntity.Objectives == "")
                    {
                        ModelState.AddModelError("OutreachEntityDetail_Objectives_Required", Resources.WebResources.OutreachEntityDetail_Objectives_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^[a-zA-Z\u00c0-\u017e¿\?.,;:¡!()""''-'\s]+$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.outreachEntity.Objectives);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_Objectives_Invalid", Resources.WebResources.OutreachEntityDetail_Objectives_Invalid);
                            validModel = false;
                        }

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

                    if (userViewModel.userDetail.FirstName == null || userViewModel.userDetail.FirstName == "")
                    {
                        ModelState.AddModelError("UserDetail_FirstName_Required", Resources.WebResources.UserDetail_FirstName_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.userDetail.FirstName);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_FirstName_Invalid", Resources.WebResources.UserDetail_FirstName_Invalid);
                            validModel = false;
                        }

                    }

                    if (userViewModel.userDetail.LastName == null || userViewModel.userDetail.LastName == "")
                    {
                        ModelState.AddModelError("UserDetail_LastName_Required", Resources.WebResources.UserDetail_LastName_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.userDetail.LastName);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_LastName_Invalid", Resources.WebResources.UserDetail_LastName_Invalid);
                            validModel = false;
                        }

                    }

                    if (userViewModel.userDetail.Gender == null || userViewModel.userDetail.Gender == "")
                    {
                        ModelState.AddModelError("UserDetail_Gender_Required", Resources.WebResources.UserDetail_Gender_Required);
                        validModel = false;
                    }

                    if (userViewModel.userDetail.MiddleInitial != null)
                    {
                        string pattern = @"^[A-Z]$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.userDetail.MiddleInitial);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_MiddleInitial_Invalid", Resources.WebResources.UserDetail_MiddleInitial_Invalid);
                            validModel = false;
                        }

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

                return RedirectToAction("Index", "Home", null); // changed to redirect to Index instead of Administracion, for now

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
                userTypes = getUserTypes(),
                Schools = getSchools(),
                OutreachEntities = getOutreachEntities(),
                Units = getUnits()
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
                //if(userViewModel.SchoolID != "")
                //    userViewModel.userDetail.AffiliateTypeID  = Int32.Parse(userViewModel.SchoolID);

                //if(userViewModel.UnitID != "")
                //    userViewModel.userDetail.AffiliateTypeID = Int32.Parse(userViewModel.UnitID);

                //if(userViewModel.OutreachEntityDetailID != "")
                //    userViewModel.userDetail.AffiliateTypeID = Int32.Parse(userViewModel.OutreachEntityDetailID);

                bool validModel = true;

                if (userViewModel.userDetail.FirstName == null || userViewModel.userDetail.FirstName == "")
                {
                    ModelState.AddModelError("UserDetail_FirstName_Required", Resources.WebResources.UserDetail_FirstName_Required);
                    validModel = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(userViewModel.userDetail.FirstName);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_FirstName_Invalid", Resources.WebResources.UserDetail_FirstName_Invalid);
                        validModel = false;
                    }

                }

                if (userViewModel.userDetail.LastName == null || userViewModel.userDetail.LastName == "")
                {
                    ModelState.AddModelError("UserDetail_LastName_Required", Resources.WebResources.UserDetail_LastName_Required);
                    validModel = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(userViewModel.userDetail.LastName);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_LastName_Invalid", Resources.WebResources.UserDetail_LastName_Invalid);
                        validModel = false;
                    }

                }

                if (userViewModel.userDetail.Gender == null || userViewModel.userDetail.Gender == "")
                {
                    ModelState.AddModelError("UserDetail_Gender_Required", Resources.WebResources.UserDetail_Gender_Required);
                    validModel = false;
                }

                if (userViewModel.userDetail.MiddleInitial != null)
                {
                    string pattern = @"^[A-Z]$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(userViewModel.userDetail.MiddleInitial);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_MiddleInitial_Invalid", Resources.WebResources.UserDetail_MiddleInitial_Invalid);
                        validModel = false;
                    }

                }

                if (userViewModel.userDetail.Role != null)
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(userViewModel.userDetail.Role);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_Role_Invalid", Resources.WebResources.UserDetail_Role_Invalid);
                        validModel = false;
                    }

                }

                if (userViewModel.userDetail.Major != null)
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(userViewModel.userDetail.Major);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_Major_Invalid", Resources.WebResources.UserDetail_Major_Invalid);
                        validModel = false;
                    }

                }

                if (validModel)
                {
                    userViewModel.user.UpdateDate = DateTime.Now;
                    userViewModel.userDetail.UpdateDate = DateTime.Now;
                    db.Entry(userViewModel.user).State = EntityState.Modified;
                    db.Entry(userViewModel.userDetail).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Administracion", "Home", null);
                }

                else
                {
                    userViewModel.userTypes = getTypes();
                    return View(userViewModel);
                }

               
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
                try { 
                        db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                               
                }
            }
            return RedirectToAction("Administracion", "Home", null);
        }

        [HttpPost]
        public ActionResult Restaurar(int id = 0)
        {
            User user = db.Users.Find(id);
            UserDetail userDetail = db.UserDetails.FirstOrDefault(i => i.UserID == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                user.DeletionDate = null;
                userDetail.DeletionDate = null;
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

        public List<SelectListItem> getSchools()
        {
            var types = new List<SelectListItem>();
            var schools = (from school in db.Schools where school.DeletionDate == null select school).ToList();
            
            types.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });

            foreach (var school in schools)
            {
                types.Add(new SelectListItem()
                {
                    Text = school.SchoolName,
                    Value = school.SchoolID + ""
                });
            }

            return types;
        }

        public List<SelectListItem> getUnits()
        {
            var types = new List<SelectListItem>();
            var units = (from unit in db.Units where unit.DeletionDate == null select unit).ToList();

            types.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });

            foreach (var unit in units)
            {
                types.Add(new SelectListItem()
                {
                    Text = unit.UnitName,
                    Value = unit.UnitID + ""
                });
            }

            return types;
        }

        public List<SelectListItem> getOutreachEntities()
        {
            var types = new List<SelectListItem>();
            var outreachEntities = (from outreachEntity in db.OutreachEntityDetails where outreachEntity.DeletionDate == null select outreachEntity).ToList();

            types.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });

            foreach (var outreachEntity in outreachEntities)
            {
                types.Add(new SelectListItem()
                {
                    Text = outreachEntity.OutreachEntityName,
                    Value = outreachEntity.OutreachEntityDetailID + ""
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

        [HttpPost]
        public ActionResult IniciarSesionPopUp()
        {
            return PartialView("Login");
        }

        [HttpPost]
        public ActionResult IniciarSesion(UserViewModel userViewModel)
        {
            // TODO: passwords have no encryption at all - fix later with SimpleCrypto NuGet Package
            User user = db.Users.FirstOrDefault(u => u.Email == userViewModel.user.Email);   

            if (user != null)
            {
                if (userViewModel.user.UserPassword.ToString().Equals(user.UserPassword))
                {
                    //Session["user"] = userViewModel.user;
                    
                    FormsAuthentication.SetAuthCookie(user.UserID+"ut="+user.UserTypeID, false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // TODO: needs a validation message here
                    ModelState.AddModelError("", "Contraseña Invalida");
                }
            }
            else
            {
                // TODO: needs a validation message here
                ModelState.AddModelError("", "Usuario no existe");
            }
           
            return View("Login");
        }
       
        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Admin Methods
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<SelectListItem> getAllUserTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();
            foreach (var userType in db.UserTypes)
            {

                if (userType.UserTypeID != 3)
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
        
        
        public ActionResult CrearUsuario()
        {

            UserViewModel userViewModel = new UserViewModel
            {

                userTypes = getAllUserTypes()

            };

            return View("CrearUsuario", userViewModel);
        }


        [HttpPost]
        public ActionResult CrearUsuario(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                userViewModel.user.CreateDate = DateTime.Now;
                userViewModel.user.UpdateDate = DateTime.Now;
                userViewModel.user.UserStatus = false;
                bool validModel = true;

                User matchingUser = db.Users.FirstOrDefault(u => u.Email == userViewModel.user.Email);

                if (matchingUser != null)
                {
                    ModelState.AddModelError("User_Email_Exists", Resources.WebResources.User_Email_Exists);
                    validModel = false;
                }

                if (userViewModel.userDetail.FirstName == null || userViewModel.userDetail.FirstName == "")
                {
                    ModelState.AddModelError("UserDetail_FirstName_Required", Resources.WebResources.UserDetail_FirstName_Required);
                    validModel = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(userViewModel.userDetail.FirstName);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_FirstName_Invalid", Resources.WebResources.UserDetail_FirstName_Invalid);
                        validModel = false;
                    }

                }

                if (userViewModel.userDetail.LastName == null || userViewModel.userDetail.LastName == "")
                {
                    ModelState.AddModelError("UserDetail_LastName_Required", Resources.WebResources.UserDetail_LastName_Required);
                    validModel = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(userViewModel.userDetail.LastName);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_LastName_Invalid", Resources.WebResources.UserDetail_LastName_Invalid);
                        validModel = false;
                    }

                }

                if (userViewModel.userDetail.Gender == null || userViewModel.userDetail.Gender == "")
                {
                    ModelState.AddModelError("UserDetail_Gender_Required", Resources.WebResources.UserDetail_Gender_Required);
                    validModel = false;
                }

                if (userViewModel.userDetail.MiddleInitial != null)
                {
                    string pattern = @"^[A-Z]$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(userViewModel.userDetail.MiddleInitial);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_MiddleInitial_Invalid", Resources.WebResources.UserDetail_MiddleInitial_Invalid);
                        validModel = false;
                    }

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
                    userViewModel.userTypes = getAllUserTypes();
                    return View("CrearUsuario", userViewModel);
                }

                db.SaveChanges();
                return RedirectToAction("Administracion", "Home", null);
            }


                userViewModel.userTypes = getAllUserTypes();

                return View("CrearUsuario", userViewModel);
        }
    }
}

