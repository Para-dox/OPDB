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
using System.Text.RegularExpressions;

namespace OPDB.Controllers
{
    public class UsuariosController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Usuarios/

        public ActionResult Index(string requested)
        {
            if (requested != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(requested))
                    {
            var users = from u in db.Users.Include(u => u.UserType).Include(u => u.UserDetails) where u.UserTypeID != 3 && u.DeletionDate == null select u;
            return PartialView("Index", users.ToList());
        }
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        public ActionResult MenuUsuarios(string requested)
        {
            if (requested != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(requested))
                    {
            return PartialView("Usuarios");
        }
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // GET: /Usuarios/Details/5

        public ActionResult Detalles(int id = 0)
        {
            UserViewModel userViewModel = new UserViewModel
            {
                User = db.Users.Find(id),
                UserDetail = (from ud in db.UserDetails where ud.UserID == id select ud).SingleOrDefault()
            };
            
            if (userViewModel == null)
            {
                return HttpNotFound();
            }

            if(Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2]))
            {
                int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);
                userViewModel.Notes = from note in db.UserNotes.Include(note => note.NoteType) where note.SubjectID == id && note.UserID == userID && note.DeletionDate == null select note;
        }
            else if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
            {
                userViewModel.Notes = from note in db.UserNotes.Include(note => note.NoteType) where note.SubjectID == id && note.DeletionDate == null select note;
            }

            return View(userViewModel);
        }

        //
        // GET: /Usuarios/Create

        public ActionResult Crear()
        {
           
            UserViewModel user = new UserViewModel{

                UserTypes = getTypes(),
                OutreachTypes = getOutreachTypes()
                        
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
                userViewModel.User.CreateDate = DateTime.Now;
                userViewModel.User.UpdateDate = DateTime.Now;
                userViewModel.User.UserStatus = false;
                bool validModel = true;

                User matchingUser = db.Users.FirstOrDefault(u => u.Email == userViewModel.User.Email);

                if (matchingUser != null)
                {
                    ModelState.AddModelError("User_Email_Exists", Resources.WebResources.User_Email_Exists);
                    validModel = false;
                }

                if (userViewModel.User.UserTypeID == 3)
                {

                    if (userViewModel.OutreachEntity.OutreachEntityName == null || userViewModel.OutreachEntity.OutreachEntityName == "")
                    {
                        ModelState.AddModelError("OutreachEntityDetail_OutreachEntityName_Required", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^([a-zA-Z\u00c0-\u017e'\s]+[^\s-][-]?){1,100}$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.OutreachEntity.OutreachEntityName);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("OutreachEntity_OutreachEntityName_Invalid", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_Invalid);
                            validModel = false;
                        }

                    }

                    if (userViewModel.OutreachEntity.Mission == null || userViewModel.OutreachEntity.Mission == "")
                    {
                        ModelState.AddModelError("OutreachEntityDetail_Mission_Required", Resources.WebResources.OutreachEntityDetail_Mission_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^([a-zA-Z\u00c0-\u017e¿\?.,;:¡!()""'\s]+[^\s-][-]?)+$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.OutreachEntity.Mission);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_Mission_Invalid", Resources.WebResources.OutreachEntityDetail_Mission_Invalid);
                            validModel = false;
                        }

                    }

                    if (userViewModel.OutreachEntity.Vision == null || userViewModel.OutreachEntity.Vision == "")
                    {
                        ModelState.AddModelError("OutreachEntityDetail_Vision_Required", Resources.WebResources.OutreachEntityDetail_Vision_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^([a-zA-Z\u00c0-\u017e¿\?.,;:¡!()""'\s]+[^\s-][-]?)+$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.OutreachEntity.Vision);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_Vision_Invalid", Resources.WebResources.OutreachEntityDetail_Vision_Invalid);
                            validModel = false;
                        }

                    }

                    if (userViewModel.OutreachEntity.Objectives == null || userViewModel.OutreachEntity.Objectives == "")
                    {
                        ModelState.AddModelError("OutreachEntityDetail_Objectives_Required", Resources.WebResources.OutreachEntityDetail_Objectives_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^([a-zA-Z\u00c0-\u017e¿\?.,;:¡!()""'\s]+[^\s-][-]?)+$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.OutreachEntity.Objectives);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_Objectives_Invalid", Resources.WebResources.OutreachEntityDetail_Objectives_Invalid);
                            validModel = false;
                        }

                    }

                    if (validModel)
                    {
                        userViewModel.OutreachEntity.CreateDate = DateTime.Now;
                        userViewModel.OutreachEntity.UpdateDate = DateTime.Now;
                        userViewModel.User.OutreachEntityDetails = new List<OutreachEntityDetail>();
                        userViewModel.User.OutreachEntityDetails.Add(userViewModel.OutreachEntity);
                        db.Users.Add(userViewModel.User);
                    }

                    else
                    {
                        userViewModel.UserTypes = getTypes();
                        userViewModel.OutreachTypes = getOutreachTypes();
                        return View(userViewModel);
                    }
                }

                else
                {

                    if (userViewModel.UserDetail.FirstName == null || userViewModel.UserDetail.FirstName == "")
                    {
                        ModelState.AddModelError("UserDetail_FirstName_Required", Resources.WebResources.UserDetail_FirstName_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.UserDetail.FirstName);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_FirstName_Invalid", Resources.WebResources.UserDetail_FirstName_Invalid);
                            validModel = false;
                        }

                    }

                    if (userViewModel.UserDetail.LastName == null || userViewModel.UserDetail.LastName == "")
                    {
                        ModelState.AddModelError("UserDetail_LastName_Required", Resources.WebResources.UserDetail_LastName_Required);
                        validModel = false;
                    }
                    else
                    {
                        string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.UserDetail.LastName);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_LastName_Invalid", Resources.WebResources.UserDetail_LastName_Invalid);
                            validModel = false;
                        }

                    }

                    if (userViewModel.UserDetail.Gender == null || userViewModel.UserDetail.Gender == "")
                    {
                        ModelState.AddModelError("UserDetail_Gender_Required", Resources.WebResources.UserDetail_Gender_Required);
                        validModel = false;
                    }

                    if (userViewModel.UserDetail.MiddleInitial != null)
                    {
                        string pattern = @"^[A-Z]$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.UserDetail.MiddleInitial);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_MiddleInitial_Invalid", Resources.WebResources.UserDetail_MiddleInitial_Invalid);
                            validModel = false;
                        }

                    }

                    if (validModel)
                    {
                        userViewModel.UserDetail.CreateDate = DateTime.Now;
                        userViewModel.UserDetail.UpdateDate = DateTime.Now;
                        userViewModel.User.UserDetails = new List<UserDetail>();
                        userViewModel.User.UserDetails.Add(userViewModel.UserDetail);
                        db.Users.Add(userViewModel.User);
                    }

                    else
                    {
                        userViewModel.UserTypes = getTypes();
                        userViewModel.OutreachTypes = getOutreachTypes();
                        return View(userViewModel);
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Index", "Home");

            }

            userViewModel.UserTypes = getTypes();
            userViewModel.OutreachTypes = getOutreachTypes();

            return View(userViewModel);
        }

        //
        // GET: /Usuarios/Edit/5

        public ActionResult Editar(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[0]) == id || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {

                    UserViewModel userViewModel = new UserViewModel
                    {
                        User = db.Users.Find(id),
                        UserDetail = db.UserDetails.FirstOrDefault(i => i.UserID == id),
                        Schools = getSchools(),
                        OutreachEntities = getOutreachEntities(),
                        Units = getUnits()
                    };

                    if (Int32.Parse(User.Identity.Name.Split(',')[0]) == id)
                        userViewModel.UserTypes = getUserTypes();

                    if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                        userViewModel.UserTypes = getAllUserTypes();

                    if (userViewModel.User == null)
                    {
                        return HttpNotFound();
                    }

                    return View(userViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // POST: /Usuarios/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(UserViewModel userViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[0]) == userViewModel.User.UserID || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
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

                        if (userViewModel.UserDetail.FirstName == null || userViewModel.UserDetail.FirstName == "")
                {
                    ModelState.AddModelError("UserDetail_FirstName_Required", Resources.WebResources.UserDetail_FirstName_Required);
                    validModel = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.FirstName);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_FirstName_Invalid", Resources.WebResources.UserDetail_FirstName_Invalid);
                        validModel = false;
                    }

                }

                        if (userViewModel.UserDetail.LastName == null || userViewModel.UserDetail.LastName == "")
                {
                    ModelState.AddModelError("UserDetail_LastName_Required", Resources.WebResources.UserDetail_LastName_Required);
                    validModel = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.LastName);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_LastName_Invalid", Resources.WebResources.UserDetail_LastName_Invalid);
                        validModel = false;
                    }

                }

                        if (userViewModel.UserDetail.Gender == null || userViewModel.UserDetail.Gender == "")
                {
                    ModelState.AddModelError("UserDetail_Gender_Required", Resources.WebResources.UserDetail_Gender_Required);
                    validModel = false;
                }

                        if (userViewModel.UserDetail.MiddleInitial != null)
                {
                    string pattern = @"^[A-Z]$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.MiddleInitial);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_MiddleInitial_Invalid", Resources.WebResources.UserDetail_MiddleInitial_Invalid);
                        validModel = false;
                    }

                }

                        if (userViewModel.UserDetail.Role != null)
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Role);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_Role_Invalid", Resources.WebResources.UserDetail_Role_Invalid);
                        validModel = false;
                    }

                }

                        if (userViewModel.UserDetail.Major != null)
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Major);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_Major_Invalid", Resources.WebResources.UserDetail_Major_Invalid);
                        validModel = false;
                    }

                }

                if (validModel)
                {
                            userViewModel.User.UpdateDate = DateTime.Now;
                            userViewModel.UserDetail.UpdateDate = DateTime.Now;
                            db.Entry(userViewModel.User).State = EntityState.Modified;
                            db.Entry(userViewModel.UserDetail).State = EntityState.Modified;
                    db.SaveChanges();

                            if (userViewModel.Source == "Administracion")
                                return RedirectToAction("Administracion", "Home");

                            else if (userViewModel.Source == "Detalles")
                                return RedirectToAction("Detalles", "Usuarios", new { id = userViewModel.User.UserID });
                }

                    }

                    if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                        userViewModel.UserTypes = getAllUserTypes();
                else
                        userViewModel.UserTypes = getUserTypes();

                    return View(userViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // GET: /Usuarios/Delete/5

        [HttpPost]
        public ActionResult Remover(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
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
                        try
                        {
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
                    return RedirectToAction("Administracion", "Home");
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult Restaurar(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
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
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        public ActionResult RemoverNota(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
            var note = db.UserNotes.Find(id);

                if ((Int32.Parse(User.Identity.Name.Split(',')[0]) == note.UserID && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {                    
            note.DeletionDate = DateTime.Now;
            db.Entry(note).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Detalles", "Usuarios", new { id = note.SubjectID });
        }
            }

            return RedirectToAction("AccesoDenegado", "Home");
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
            if (User.Identity.IsAuthenticated)
            {

                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2]))
                {
            EscuelasController controller = new EscuelasController();

            UserViewModel userViewModel = new UserViewModel
            {

                NoteTypes = controller.getNoteTypes(),
                        Note = new UserNote
                {

                    SubjectID = id
                }
            };

            return PartialView("CrearNota", userViewModel);
        }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }


        [HttpPost]
        public ActionResult GuardarNota(UserViewModel userViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {

                if ((Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                    userViewModel.Note.UpdateUser = userID;
                    userViewModel.Note.UpdateDate = DateTime.Now;

                    if (userViewModel.Note.UserNoteID == 0)
            {
                if (ModelState.IsValid) 
                { 
                            userViewModel.Note.CreateDate = DateTime.Now;

                            userViewModel.Note.UserID = userID;
                            userViewModel.Note.CreateUser = userID;

                            db.UserNotes.Add(userViewModel.Note);
                    db.SaveChanges();

                    return View("_Hack");
                }

                return Content(GetErrorsFromModelState(userViewModel));          
            }
                    else if (ModelState.IsValid)
            {

                        db.Entry(userViewModel.Note).State = EntityState.Modified;
                db.SaveChanges();
                return View("_Hack");                
                
            }

            return Content(GetErrorsFromModelState(userViewModel));
        }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult EditarNota(int id)
        {
            if (User.Identity.IsAuthenticated)
            {

                if ((Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
            EscuelasController controller = new EscuelasController();

            UserViewModel userViewModel = new UserViewModel
            {

                NoteTypes = controller.getNoteTypes(),
                        Note = db.UserNotes.Find(id)
            };

            return PartialView("EditarNota", userViewModel);
        }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }


        [HttpPost]
        public ActionResult VerNota(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
            UserNote userNote = db.UserNotes.Find(id);

                if ((Int32.Parse(User.Identity.Name.Split(',')[0]) == userNote.UserID && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    
            userNote.NoteType = db.NoteTypes.Find(userNote.NoteTypeID);
            

            UserViewModel userViewModel = new UserViewModel
            {
                        Note = userNote,
                        UserDetail = db.UserDetails.First(user => user.UserID == userNote.SubjectID)
            };

            
            return PartialView("VerNota", userViewModel);
        }
            }

            return RedirectToAction("AccesoDenegado", "Home");
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

        public ActionResult Removidos(string requested)
        {
            if (User.Identity.IsAuthenticated)
            {

                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(requested))
        {
            var users = from u in db.Users.Include(u => u.UserType).Include(u => u.UserDetails) where (u.UserTypeID != 3) && u.DeletionDate != null select u;

            return PartialView("Removidos", users.ToList());
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");

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
            User user = db.Users.FirstOrDefault(u => u.Email == userViewModel.User.Email);   

            if (!user.UserStatus)
            {
                ModelState.AddModelError("", Resources.WebResources.User_Email_NotApproved);

                return Content(GetErrorsFromModelState(userViewModel));
            }
            else
            {
            if (user != null)
            {
                if (userViewModel.User.UserPassword.ToString().Equals(user.UserPassword))
                {
                        FormsAuthentication.SetAuthCookie(user.UserID + "," + user.UserTypeID + "," + user.UserStatus, false);

                    return PartialView("_Hack");
                }
                else
                {
                    ModelState.AddModelError("", Resources.WebResources.User_UserPassword_NoMatch);
                }
            }
            else
            {
                ModelState.AddModelError("", Resources.WebResources.User_Email_NoMatch);
            }

            return Content(GetErrorsFromModelState(userViewModel));
        }
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
            if (User.Identity.IsAuthenticated)
            {

                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
            UserViewModel userViewModel = new UserViewModel
            {

                        UserTypes = getAllUserTypes()

            };

            return View("CrearUsuario", userViewModel);
        }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }


        [HttpPost]
        public ActionResult CrearUsuario(UserViewModel userViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {

                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
            if (ModelState.IsValid)
            {
                        userViewModel.User.CreateDate = DateTime.Now;
                        userViewModel.User.UpdateDate = DateTime.Now;
                        userViewModel.User.UserStatus = true;
                bool validModel = true;

                        User matchingUser = db.Users.FirstOrDefault(u => u.Email == userViewModel.User.Email);

                if (matchingUser != null)
                {
                    ModelState.AddModelError("User_Email_Exists", Resources.WebResources.User_Email_Exists);
                    validModel = false;
                }

                        if (userViewModel.UserDetail.FirstName == null || userViewModel.UserDetail.FirstName == "")
                {
                    ModelState.AddModelError("UserDetail_FirstName_Required", Resources.WebResources.UserDetail_FirstName_Required);
                    validModel = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.FirstName);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_FirstName_Invalid", Resources.WebResources.UserDetail_FirstName_Invalid);
                        validModel = false;
                    }

                }

                        if (userViewModel.UserDetail.LastName == null || userViewModel.UserDetail.LastName == "")
                {
                    ModelState.AddModelError("UserDetail_LastName_Required", Resources.WebResources.UserDetail_LastName_Required);
                    validModel = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\u00c0-\u017e''-'\s]{1,50}$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.LastName);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_LastName_Invalid", Resources.WebResources.UserDetail_LastName_Invalid);
                        validModel = false;
                    }

                }

                        if (userViewModel.UserDetail.Gender == null || userViewModel.UserDetail.Gender == "")
                {
                    ModelState.AddModelError("UserDetail_Gender_Required", Resources.WebResources.UserDetail_Gender_Required);
                    validModel = false;
                }

                        if (userViewModel.UserDetail.MiddleInitial != null)
                {
                    string pattern = @"^[A-Z]$";
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.MiddleInitial);
                    if (matches.Count == 0)
                    {
                        ModelState.AddModelError("UserDetail_MiddleInitial_Invalid", Resources.WebResources.UserDetail_MiddleInitial_Invalid);
                        validModel = false;
                    }

                }

                if (validModel)
                {
                            userViewModel.UserDetail.CreateDate = DateTime.Now;
                            userViewModel.UserDetail.UpdateDate = DateTime.Now;
                            userViewModel.User.UserDetails = new List<UserDetail>();
                            userViewModel.User.UserDetails.Add(userViewModel.UserDetail);
                            db.Users.Add(userViewModel.User);
                            db.SaveChanges();
                            return RedirectToAction("Administracion", "Home", null);
                        }
                        
                }

                    userViewModel.UserTypes = getAllUserTypes();
                    return View("CrearUsuario", userViewModel);
                }

            }

            return RedirectToAction("AccesoDenegado", "Home");
        }
    }
}

