using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPDB.Models;
using System.Text.RegularExpressions;

namespace OPDB.Controllers
{
    public class AlcanceController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Alcance/

        public ActionResult Index(string requested)
        {
            if (requested != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(requested))
                    {
                        var users = from u in db.Users.Include(u => u.UserDetails) where u.UserTypeID == 3 && u.DeletionDate == null && u.UserStatus == true select u;
                        return PartialView("Index", users.ToList());
                    }

                }
            }

            return RedirectToAction("AccesoDenegado", "Home");

        }

        public ActionResult MenuAlcance(string requested)
        {
            if (requested != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(requested))
                    {
                        return PartialView("Alcance");
                    }
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // GET: /Alcance/Details/5

        public ActionResult Detalles(int id = 0)
        {
            var user = db.Users.Find(id);
            var outreachEntity = db.OutreachEntityDetails.First(outreach => outreach.UserID == id);

            UserViewModel outreachViewModel = new UserViewModel
            {
                User = user,
                OutreachEntity = outreachEntity,
                Notes = (from note in db.UserNotes.Include(note => note.NoteType) where note.SubjectID == id && note.DeletionDate == null select note).ToList(),
                Activities = (from activity in db.Activities where activity.UserID == id && activity.DeletionDate == null select activity).ToList()
            };

            foreach (var activity in outreachViewModel.Activities)
            {
                if (activity.ActivityDate == null)
                    activity.ActivityDate = new DateTime();

                if (activity.ActivityTime == null)
                    activity.ActivityTime = "";
            }

            if (outreachViewModel.OutreachEntity == null)
            {
                return HttpNotFound();
            }

            return View(outreachViewModel);
        }

        //
        // GET: /Alcance/Create

        public ActionResult Crear()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    HomeController controller = new HomeController();

                    UserViewModel userViewModel = new UserViewModel
                    {
                        OutreachTypes = controller.getOutreachTypes(),
                        User = new User
                        {
                            UserTypeID = 3
                        }
                    };

                    return View(userViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult Crear(UserViewModel userViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {

                    if (ModelState.IsValid)
                    {
                        bool validModel = true;

                        User matchingUser = db.Users.FirstOrDefault(u => u.Email == userViewModel.User.Email);

                        if (matchingUser != null)
                        {
                            ModelState.AddModelError("User_Email_Exists", Resources.WebResources.User_Email_Exists);
                            validModel = false;
                        }

                        if (String.Compare(userViewModel.User.UserPassword, userViewModel.ConfirmPassword) != 0)
                        {
                            ModelState.AddModelError("User_Password_NoMatch", Resources.WebResources.User_Password_NoMatch);
                            validModel = false;
                        }

                        if (userViewModel.OutreachEntity.OutreachEntityTypeID == 0)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_OutreachEntityTypeID_Required", Resources.WebResources.OutreachEntityDetail_OutreachEntityTypeID_Required);
                            validModel = false;
                        }

                        if (userViewModel.OutreachEntity.OutreachEntityName == null || userViewModel.OutreachEntity.OutreachEntityName == "")
                        {
                            ModelState.AddModelError("OutreachEntityDetail_OutreachEntityName_Required", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_Required);
                            validModel = false;
                        }
                        else
                        {
                            string pattern = @"^[a-zA-Z\u00c0-\u017e'\s]+[-]?[a-zA-Z\u00c0-\u017e'\s]+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.OutreachEntity.OutreachEntityName);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("OutreachEntityDetail_OutreachEntityName_Invalid", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.OutreachEntity.OutreachEntityName.Length > 100)
                            {
                                ModelState.AddModelError("OutreachEntityDetail_OutreachEntityName_LengthExceeded", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_LengthExceeded);
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
                            string pattern = @"^([a-zA-Z\u00c0-\u017e0-9¿?.,;:!¡()$""'/\s]+[-]?[a-zA-Z\u00c0-\u017e0-9?.,;:!)@$""'/\s]+)+$";
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
                            string pattern = @"^([a-zA-Z\u00c0-\u017e0-9¿?.,;:!¡()$""'/\s]+[-]?[a-zA-Z\u00c0-\u017e0-9?.,;:!)@$""'/\s]+)+$";
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
                            string pattern = @"^([a-zA-Z\u00c0-\u017e0-9¿?.,;:!¡()$""'/\s]+[-]?[a-zA-Z\u00c0-\u017e0-9?.,;:!)@$""'/\s]+)+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.OutreachEntity.Objectives);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("OutreachEntityDetail_Objectives_Invalid", Resources.WebResources.OutreachEntityDetail_Objectives_Invalid);
                                validModel = false;
                            }

                        }
                        if(userViewModel.OutreachEntity.Location != null && userViewModel.OutreachEntity.Location != "")
                        {
                            string pattern = @"^([a-zA-Z\u00c0-\u017e0-9.,\s]+[-]?[a-zA-Z\u00c0-\u017e0-9.,\s]+)+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.OutreachEntity.Location);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("OutreachEntityDetail_Location_Invalid", Resources.WebResources.OutreachEntityDetail_Location_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.OutreachEntity.Location.Length > 100)
                            {
                                ModelState.AddModelError("OutreachEntityDetail_Location_LengthExceeded", Resources.WebResources.OutreachEntityDetail_Location_LengthExceeded);
                                validModel = false;
                            }

                        }

                        if (validModel)
                        {

                            userViewModel.User.CreateDate = DateTime.Now;
                            userViewModel.User.UpdateDate = DateTime.Now;
                            userViewModel.User.UserStatus = true;
                            userViewModel.OutreachEntity.CreateDate = DateTime.Now;
                            userViewModel.OutreachEntity.UpdateDate = DateTime.Now;
                            userViewModel.User.OutreachEntityDetails = new List<OutreachEntityDetail>();
                            userViewModel.User.OutreachEntityDetails.Add(userViewModel.OutreachEntity);
                            db.Users.Add(userViewModel.User);
                            db.SaveChanges();

                            return RedirectToAction("Administracion", "Home");
                        }


                    }

                    HomeController controller = new HomeController();

                    GetErrorsFromModelState(userViewModel);
                    userViewModel.OutreachTypes = controller.getOutreachTypes();
                    return View(userViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }


        public ActionResult Editar(string source, int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((Int32.Parse(User.Identity.Name.Split(',')[0]) == id && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    var user = db.Users.Find(id);
                    var outreachEntity = db.OutreachEntityDetails.First(outreach => outreach.UserID == id);
                    HomeController controller = new HomeController();

                    UserViewModel outreachViewModel = new UserViewModel
                    {
                        User = user,
                        OutreachEntity = outreachEntity,
                        OutreachTypes = controller.getOutreachTypes(),
                        Source = source
                    };

                    if (outreachViewModel.OutreachEntity == null)
                    {
                        return HttpNotFound();
                    }

                    return View(outreachViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // POST: /Alcance/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(UserViewModel userViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((Int32.Parse(User.Identity.Name.Split(',')[0]) == userViewModel.User.UserID && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {

                    if (ModelState.IsValid)
                    {
                        userViewModel.User.UpdateDate = DateTime.Now;
                        userViewModel.OutreachEntity.UpdateDate = DateTime.Now;
                        bool validModel = true;

                        if (userViewModel.OutreachEntity.OutreachEntityTypeID == 0)
                        {
                            ModelState.AddModelError("OutreachEntityDetail_OutreachEntityTypeID_Required", Resources.WebResources.OutreachEntityDetail_OutreachEntityTypeID_Required);
                            validModel = false;
                        }

                        if (userViewModel.OutreachEntity.OutreachEntityName == null || userViewModel.OutreachEntity.OutreachEntityName == "")
                        {
                            ModelState.AddModelError("OutreachEntityDetail_OutreachEntityName_Required", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_Required);
                            validModel = false;
                        }
                        else
                        {
                            string pattern = @"^[a-zA-Z\u00c0-\u017e'\s]+[-]?[a-zA-Z\u00c0-\u017e'\s]+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.OutreachEntity.OutreachEntityName);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("OutreachEntityDetail_OutreachEntityName_Invalid", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.OutreachEntity.OutreachEntityName.Length > 100)
                            {
                                ModelState.AddModelError("OutreachEntityDetail_OutreachEntityName_LengthExceeded", Resources.WebResources.OutreachEntityDetail_OutreachEntityName_LengthExceeded);
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
                            string pattern = @"^([a-zA-Z\u00c0-\u017e0-9¿?.,;:!¡()$""'/\s]+[-]?[a-zA-Z\u00c0-\u017e0-9?.,;:!)@$""'/\s]+)+$";
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
                            string pattern = @"^([a-zA-Z\u00c0-\u017e0-9¿?.,;:!¡()$""'/\s]+[-]?[a-zA-Z\u00c0-\u017e0-9?.,;:!)@$""'/\s]+)+$";
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
                            string pattern = @"^([a-zA-Z\u00c0-\u017e0-9¿?.,;:!¡()$""'/\s]+[-]?[a-zA-Z\u00c0-\u017e0-9?.,;:!)@$""'/\s]+)+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.OutreachEntity.Objectives);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("OutreachEntityDetail_Objectives_Invalid", Resources.WebResources.OutreachEntityDetail_Objectives_Invalid);
                                validModel = false;
                            }

                        }

                        if (userViewModel.OutreachEntity.Location != null && userViewModel.OutreachEntity.Location != "")
                        {
                            string pattern = @"^([a-zA-Z\u00c0-\u017e0-9.,\s]+[-]?[a-zA-Z\u00c0-\u017e0-9.,\s]+)+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.OutreachEntity.Location);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("OutreachEntityDetail_Location_Invalid", Resources.WebResources.OutreachEntityDetail_Location_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.OutreachEntity.Location.Length > 100)
                            {
                                ModelState.AddModelError("OutreachEntityDetail_Location_LengthExceeded", Resources.WebResources.OutreachEntityDetail_Location_LengthExceeded);
                                validModel = false;
                            }

                        }
                        

                        if (validModel)
                        {
                            db.Entry(userViewModel.User).State = EntityState.Modified;
                            db.Entry(userViewModel.OutreachEntity).State = EntityState.Modified;
                            db.SaveChanges();

                            if (userViewModel.Source == "Detalles")
                                return RedirectToAction("Detalles", "Alcance", new { id = userViewModel.User.UserID });

                            else if(userViewModel.Source == "Administracion")
                                return RedirectToAction("Administracion", "Home");
                        }

                    }

                    HomeController controller = new HomeController();

                    userViewModel.OutreachTypes = controller.getOutreachTypes();
                    return View(userViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //[HttpPost]
        //public ActionResult PopUpRemover(int id)
        //{
        //    UserViewModel userViewModel = new UserViewModel
        //    {
        //        User = db.Users.Find(id)
        //    };
            
        //    return PartialView("RemovalReason", userViewModel);
        //}

       
        //[HttpPost]
        //public ActionResult Remover(UserViewModel userViewModel)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
        //        {
        //            User user = db.Users.Find(userViewModel.User.UserID);
        //            OutreachEntityDetail outreachDetail = db.OutreachEntityDetails.First(outreach => outreach.UserID == user.UserID);


        //            if (outreachDetail == null)
        //            {
        //                return HttpNotFound();
        //            }

        //            else
        //            {
        //                if (userViewModel.User.RemovalReason != null)
        //                {
        //                    user.RemovalReason = userViewModel.User.RemovalReason;
        //                    user.DeletionDate = DateTime.Now;
        //                    outreachDetail.DeletionDate = DateTime.Now;
        //                    db.Entry(user).State = EntityState.Modified;
        //                    db.Entry(outreachDetail).State = EntityState.Modified;
        //                    db.SaveChanges();
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("User_RemovalReason_Required", Resources.WebResources.User_RemovalReason_Required);
        //                }
        //            }

        //            UsuariosController controller = new UsuariosController();

        //            return Content(controller.GetErrorsFromModelState(userViewModel));
        //        }
        //    }

        //    return RedirectToAction("AccesoDenegado", "Home");
        //}

        public ActionResult Restaurar(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    User user = db.Users.Find(id);
                    OutreachEntityDetail outreachDetail = db.OutreachEntityDetails.First(outreach => outreach.UserID == id);


                    if (outreachDetail == null)
                    {
                        return HttpNotFound();
                    }

                    else
                    {
                        user.DeletionDate = null;
                        user.RemovalReason = null;
                        outreachDetail.DeletionDate = null;
                        db.Entry(user).State = EntityState.Modified;
                        db.Entry(outreachDetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Administracion", "Home");
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
                    return RedirectToAction("Detalles", "Alcance", new { id = note.SubjectID });
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult CrearNota(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    EscuelasController controller = new EscuelasController();

                    UserViewModel outreachViewModel = new UserViewModel
                     {

                         NoteTypes = controller.getNoteTypes(),
                         Note = new UserNote
                         {

                             SubjectID = id
                         }
                     };

                    return PartialView("CrearNota", outreachViewModel);
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

        public ActionResult EditarNota(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var outreachNote = db.UserNotes.Find(id);

                if ((Int32.Parse(User.Identity.Name.Split(',')[0]) == outreachNote.UserID && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    EscuelasController controller = new EscuelasController();

                    UserViewModel outreachViewModel = new UserViewModel
                    {

                        NoteTypes = controller.getNoteTypes(),
                        Note = outreachNote
                    };

                    return PartialView("EditarNota", outreachViewModel);
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
                userNote.NoteType = db.NoteTypes.Find(userNote.NoteTypeID);

                if ((Int32.Parse(User.Identity.Name.Split(',')[0]) == userNote.UserID && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {           
                    UserViewModel userViewModel = new UserViewModel
                    {
                        Note = userNote,
                        OutreachEntity = db.OutreachEntityDetails.First(user => user.UserID == userNote.SubjectID)
                    };


                    return PartialView("VerNota", userViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        public ActionResult Lista()
        {
            var users = (from outreachEntity in db.OutreachEntityDetails join user in db.Users on outreachEntity.UserID equals user.UserID where (user.UserTypeID == 3) && outreachEntity.DeletionDate == null && user.UserStatus == true orderby outreachEntity.OutreachEntityName ascending select outreachEntity).ToList();

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

        public ActionResult Removidos(string requested)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(requested))
                {
                    var users = from outreach in db.Users.Include(outreach => outreach.OutreachEntityDetails) where outreach.UserTypeID == 3 && outreach.DeletionDate != null select outreach;

                    return PartialView("Removidos", users.ToList());
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");

        }

        public ActionResult Pendientes(string requested)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(requested))
                {
                    var users = from u in db.Users.Include(u => u.UserDetails) where u.UserTypeID == 3 && u.DeletionDate == null && u.UserStatus != true select u;
                    return PartialView("Pendientes", users.ToList());
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

        [HttpPost]
        public ActionResult Aprobar(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    var user = db.Users.Find(id);

                    user.UserStatus = true;
                    user.UpdateDate = DateTime.Now;

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Administracion", "Home");
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}