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
using System.Globalization;
using System.Text;

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
                UserDetail = (from ud in db.UserDetails where ud.UserID == id select ud).SingleOrDefault(),
                Notes = new List<UserInfoViewModel>()
            };

            if (userViewModel.UserDetail.DateOfBirth == null)
                userViewModel.UserDetail.DateOfBirth = new DateTime();

            if (userViewModel == null)
            {
                return HttpNotFound();
            }

            if (Request.IsAuthenticated)
            {
                var Notes = new List<UserNote>();

                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 || Int32.Parse(User.Identity.Name.Split(',')[0]) == id)
                {
                    Notes = (from note in db.UserNotes where note.SubjectID == id && note.DeletionDate == null orderby note.CreateDate descending select note).ToList();
                }
                else if(Int32.Parse(User.Identity.Name.Split(',')[0]) != id && Boolean.Parse(User.Identity.Name.Split(',')[2]))
                {
                    int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);
                    Notes = (from note in db.UserNotes where note.SubjectID == id && note.UserID == userID && note.DeletionDate == null orderby note.CreateDate descending select note).ToList();
                }

                foreach (var note in Notes)
                {
                    string sender = "";

                    var user = db.Users.Find(note.UserID);

                    if (user.UserTypeID != 3)
                    {
                        var userDetail = db.UserDetails.FirstOrDefault(u => u.UserID == user.UserID);

                        if(userDetail.MiddleInitial != null)
                            sender = userDetail.FirstName + " " + userDetail.MiddleInitial + " " + userDetail.LastName;
                        else
                            sender = userDetail.FirstName + " " + userDetail.LastName;
                        
                    }
                    else
                    {
                        var outreachEntity = db.OutreachEntityDetails.FirstOrDefault(u => u.UserID == user.UserID);
                        sender = outreachEntity.OutreachEntityName;
                    }

                    userViewModel.Notes.Add(new UserInfoViewModel
                    {
                        UserNote = note,
                        Sender = sender
                    });
                }
         
               
            }

            return View(userViewModel);
        }

        //
        // GET: /Usuarios/Create

        public ActionResult Crear()
        {
            HomeController controller = new HomeController();

            UserViewModel user = new UserViewModel
            {

                UserTypes = getUserTypes("New"),
                OutreachTypes = controller.getOutreachTypes()

            };

            return View(user);
        }

        //
        // POST: /Usuarios/Create

        [HttpPost]
        public ActionResult Crear(UserViewModel userViewModel)
        {
            HomeController controller = new HomeController();

            if (ModelState.IsValid)
            {
                userViewModel.User.CreateDate = DateTime.Now;
                userViewModel.User.UpdateDate = DateTime.Now;
                bool validModel = true;

                User matchingUser = (from user in db.Users where (String.Compare(user.Email, userViewModel.User.Email, true) == 0) && user.DeletionDate == null select user).FirstOrDefault();

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

                if (userViewModel.User.UserTypeID == 3)
                {
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
                        var crypto = new SimpleCrypto.PBKDF2();
                        crypto.HashIterations = 5000;
                        string passwordHash = crypto.Compute(userViewModel.User.UserPassword);

                        userViewModel.User.UserPassword = passwordHash;
                        userViewModel.User.PasswordSalt = crypto.Salt;

                        userViewModel.OutreachEntity.CreateDate = DateTime.Now;
                        userViewModel.OutreachEntity.UpdateDate = DateTime.Now;
                        userViewModel.User.OutreachEntityDetails = new List<OutreachEntityDetail>();
                        userViewModel.User.OutreachEntityDetails.Add(userViewModel.OutreachEntity);
                        userViewModel.User.UserStatus = false;
                        db.Users.Add(userViewModel.User);
                    }

                    else
                    {
                        userViewModel.UserTypes = getUserTypes("New");
                        userViewModel.OutreachTypes = controller.getOutreachTypes();
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
                        string pattern = @"^[a-zA-Z\u00c0-\u017e']+[-]?[a-zA-Z\u00c0-\u017e']+$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.UserDetail.FirstName);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_FirstName_Invalid", Resources.WebResources.UserDetail_FirstName_Invalid);
                            validModel = false;
                        }
                        else if (userViewModel.UserDetail.FirstName.Length > 50)
                        {
                            ModelState.AddModelError("UserDetail_FirstName_LengthExceeded", Resources.WebResources.UserDetail_FirstName_LengthExceeded);
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
                        string pattern = @"^[a-zA-Z\u00c0-\u017e']+[-]?[a-zA-Z\u00c0-\u017e']+$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.UserDetail.LastName);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_LastName_Invalid", Resources.WebResources.UserDetail_LastName_Invalid);
                            validModel = false;
                        }
                        else if (userViewModel.UserDetail.LastName.Length > 50)
                        {
                            ModelState.AddModelError("UserDetail_LastName_LengthExceeded", Resources.WebResources.UserDetail_LastName_LengthExceeded);
                            validModel = false;
                        }

                    }

                    if (userViewModel.UserDetail.Gender == null || userViewModel.UserDetail.Gender == "")
                    {
                        ModelState.AddModelError("UserDetail_Gender_Required", Resources.WebResources.UserDetail_Gender_Required);
                        validModel = false;
                    }

                    if (userViewModel.UserDetail.MiddleInitial != null && userViewModel.UserDetail.MiddleInitial != "")
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

                    if (userViewModel.BirthDate != null && userViewModel.BirthDate != "")
                    {
                        string pattern = @"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.BirthDate);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_DateOfBirth_Invalid", Resources.WebResources.UserDetail_DateOfBirth_Invalid);
                            validModel = false;
                        }
                        else
                        {
                            userViewModel.UserDetail.DateOfBirth = DateTime.ParseExact(userViewModel.BirthDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }

                    }

                    if (userViewModel.UserDetail.Role != null && userViewModel.UserDetail.Role != "")
                    {
                        string pattern = @"^[a-zA-Z\u00c0-\u017e\s]+[-]?[a-zA-Z\u00c0-\u017e\s]+$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Role);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_Role_Invalid", Resources.WebResources.UserDetail_Role_Invalid);
                            validModel = false;
                        }
                        else if (userViewModel.UserDetail.Role.Length > 100)
                        {
                            ModelState.AddModelError("UserDetail_Role_LengthExceeded", Resources.WebResources.UserDetail_Role_LengthExceeded);
                            validModel = false;
                        }

                    }

                    if (userViewModel.UserDetail.Major != null && userViewModel.UserDetail.Major != "")
                    {
                        string pattern = @"^[a-zA-Z\u00c0-\u017e\s]+[-]?[a-zA-Z\u00c0-\u017e\s]+$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Major);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_Major_Invalid", Resources.WebResources.UserDetail_Major_Invalid);
                            validModel = false;
                        }
                        else if (userViewModel.UserDetail.Major.Length > 100)
                        {
                            ModelState.AddModelError("UserDetail_Major_LengthExceeded", Resources.WebResources.UserDetail_Major_LengthExceeded);
                            validModel = false;
                        }
                    }

                    if (userViewModel.UserDetail.Grade != null && userViewModel.UserDetail.Grade != "")
                    {
                        string pattern = @"^(([1]?[0-2])|[1-9]|[K])$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Grade);
                        if (matches.Count == 0)
                        {
                            ModelState.AddModelError("UserDetail_Grade_Invalid", Resources.WebResources.UserDetail_Grade_Invalid);
                            validModel = false;
                        }
                        else if (userViewModel.UserDetail.Grade.Length > 2)
                        {
                            ModelState.AddModelError("UserDetail_Grade_LengthExceeded", Resources.WebResources.UserDetail_Grade_LengthExceeded);
                            validModel = false;
                        }
                    }

                    if (validModel)
                    {
                        var crypto = new SimpleCrypto.PBKDF2();
                        crypto.HashIterations = 5000;
                        string passwordHash = crypto.Compute(userViewModel.User.UserPassword);

                        userViewModel.User.UserPassword = passwordHash;
                        userViewModel.User.PasswordSalt = crypto.Salt;

                        userViewModel.UserDetail.CreateDate = DateTime.Now;
                        userViewModel.UserDetail.UpdateDate = DateTime.Now;
                        userViewModel.User.UserDetails = new List<UserDetail>();
                        userViewModel.User.UserDetails.Add(userViewModel.UserDetail);
                        userViewModel.User.UserStatus = true;
                        db.Users.Add(userViewModel.User);

                       

                    }

                    else
                    {
                        userViewModel.UserTypes = getUserTypes("New");
                        userViewModel.OutreachTypes = controller.getOutreachTypes();
                        return View(userViewModel);
                    }
                }

                db.SaveChanges();                   
                

                return RedirectToAction("Index", "Home");

            }

            //userViewModel.UserTypes = getUserTypes("New");
            //userViewModel.OutreachTypes = controller.getOutreachTypes();

            return Content(GetErrorsFromModelState(userViewModel));
        }

        //
        // GET: /Usuarios/Edit/5

        public ActionResult Editar(string source, int id = 0)
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
                        Source = source,
                        Units = getUnits()
                    };

                    if (userViewModel.UserDetail.DateOfBirth != null)
                        userViewModel.BirthDate = userViewModel.UserDetail.DateOfBirth.Value.ToString("dd/MM/yyyy");

                    if (Int32.Parse(User.Identity.Name.Split(',')[0]) == id)
                        userViewModel.UserTypes = getUserTypes("User");

                    if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                        userViewModel.UserTypes = getUserTypes("Admin");

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
                        bool validModel = true;

                        if (userViewModel.UserDetail.FirstName == null || userViewModel.UserDetail.FirstName == "")
                        {
                            ModelState.AddModelError("UserDetail_FirstName_Required", Resources.WebResources.UserDetail_FirstName_Required);
                            validModel = false;
                        }
                        else
                        {
                            string pattern = @"^[a-zA-Z\u00c0-\u017e']+[-]?[a-zA-Z\u00c0-\u017e']+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.FirstName);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_FirstName_Invalid", Resources.WebResources.UserDetail_FirstName_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.UserDetail.FirstName.Length > 50)
                            {
                                ModelState.AddModelError("UserDetail_FirstName_LengthExceeded", Resources.WebResources.UserDetail_FirstName_LengthExceeded);
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
                            string pattern = @"^[a-zA-Z\u00c0-\u017e']+[-]?[a-zA-Z\u00c0-\u017e']+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.LastName);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_LastName_Invalid", Resources.WebResources.UserDetail_LastName_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.UserDetail.LastName.Length > 50)
                            {
                                ModelState.AddModelError("UserDetail_LastName_LengthExceeded", Resources.WebResources.UserDetail_LastName_LengthExceeded);
                                validModel = false;
                            }

                        }

                        if (userViewModel.UserDetail.Gender == null || userViewModel.UserDetail.Gender == "")
                        {
                            ModelState.AddModelError("UserDetail_Gender_Required", Resources.WebResources.UserDetail_Gender_Required);
                            validModel = false;
                        }

                        if (userViewModel.UserDetail.MiddleInitial != null && userViewModel.UserDetail.MiddleInitial != "")
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

                        if (userViewModel.BirthDate != null && userViewModel.BirthDate != "")
                        {
                            string pattern = @"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.BirthDate);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_DateOfBirth_Invalid", Resources.WebResources.UserDetail_DateOfBirth_Invalid);
                                validModel = false;
                            }
                            else
                            {
                                userViewModel.UserDetail.DateOfBirth = DateTime.ParseExact(userViewModel.BirthDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            }

                        }

                        if (userViewModel.UserDetail.Role != null && userViewModel.UserDetail.Role != "")
                        {
                            string pattern = @"^[a-zA-Z\u00c0-\u017e\s]+[-]?[a-zA-Z\u00c0-\u017e\s]+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Role);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_Role_Invalid", Resources.WebResources.UserDetail_Role_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.UserDetail.Role.Length > 100)
                            {
                                ModelState.AddModelError("UserDetail_Role_LengthExceeded", Resources.WebResources.UserDetail_Role_LengthExceeded);
                                validModel = false;
                            }

                        }

                        if (userViewModel.UserDetail.Major != null && userViewModel.UserDetail.Major != "")
                        {
                            string pattern = @"^[a-zA-Z\u00c0-\u017e\s]+[-]?[a-zA-Z\u00c0-\u017e\s]+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Major);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_Major_Invalid", Resources.WebResources.UserDetail_Major_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.UserDetail.Major.Length > 100)
                            {
                                ModelState.AddModelError("UserDetail_Major_LengthExceeded", Resources.WebResources.UserDetail_Major_LengthExceeded);
                                validModel = false;
                            }

                        }

                        if (userViewModel.UserDetail.Grade != null && userViewModel.UserDetail.Grade != "")
                        {
                            string pattern = @"^(([1]?[0-2])|[1-9]|[K])$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Grade);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_Grade_Invalid", Resources.WebResources.UserDetail_Grade_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.UserDetail.Grade.Length > 2)
                            {
                                ModelState.AddModelError("UserDetail_Grade_LengthExceeded", Resources.WebResources.UserDetail_Grade_LengthExceeded);
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
                            {
                                return RedirectToAction("Administracion", "Home");
                            }

                            else if (userViewModel.Source == "Detalles")
                            {
                                return RedirectToAction("Detalles", "Usuarios", new { id = userViewModel.User.UserID });
                            }
                        }

                    }

                    if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                        userViewModel.UserTypes = getUserTypes("Admin");
                    else
                        userViewModel.UserTypes = getUserTypes("User");

                    return View(userViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult PopUpRemover(int id)
        {
            UserViewModel userViewModel = new UserViewModel
            {
                User = db.Users.Find(id)
            };

            return PartialView("RemovalReason", userViewModel);
        }

        [HttpPost]
        public ActionResult Remover(UserViewModel userViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {

                    if (userViewModel.User.UserTypeID != 3)
                    {
                        userViewModel.UserDetail = db.UserDetails.FirstOrDefault(i => i.UserID == userViewModel.User.UserID);

                        if (userViewModel.User.RemovalReason != null)
                        {
                            userViewModel.User.UserStatus = false;
                            userViewModel.User.DeletionDate = DateTime.Now;
                            userViewModel.UserDetail.DeletionDate = DateTime.Now;
                            db.Entry(userViewModel.User).State = EntityState.Modified;
                            db.Entry(userViewModel.UserDetail).State = EntityState.Modified;
                            db.SaveChanges();

                            return View("_Hack");
                        }
                        else
                        {
                            ModelState.AddModelError("User_RemovalReason_Required", Resources.WebResources.User_RemovalReason_Required);
                        }
                    }

                    else if (userViewModel.User.UserTypeID == 3)
                    {
                        userViewModel.OutreachEntity = db.OutreachEntityDetails.FirstOrDefault(i => i.UserID == userViewModel.User.UserID);

                        if (userViewModel.User.RemovalReason != null)
                        {
                            userViewModel.User.UserStatus = false;
                            userViewModel.User.DeletionDate = DateTime.Now;
                            userViewModel.OutreachEntity.DeletionDate = DateTime.Now;
                            db.Entry(userViewModel.User).State = EntityState.Modified;
                            db.Entry(userViewModel.OutreachEntity).State = EntityState.Modified;
                            db.SaveChanges();

                            return View("_Hack");
                        }
                        else
                        {
                            ModelState.AddModelError("User_RemovalReason_Required", Resources.WebResources.User_RemovalReason_Required);
                        }
                    }
                    return Content(GetErrorsFromModelState(userViewModel));
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
                        user.UserStatus = true;
                        user.DeletionDate = null;
                        user.RemovalReason = null;
                        userDetail.DeletionDate = null;
                        db.Entry(user).State = EntityState.Modified;
                        db.Entry(userDetail).State = EntityState.Modified;
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

        

        public List<SelectListItem> getUserTypes(string request)
        {
            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem()
            {
                Text = null,
                Value = null

            });

            if (request == "Admin")
            {
                foreach (var userType in (from userType in db.UserTypes where userType.UserTypeID != 3 select userType))
                {
                        types.Add(new SelectListItem()
                        {
                            Text = userType.UserType1,
                            Value = userType.UserTypeID + ""

                        });
                }
            }
            else if(request == "User")
            {
                foreach (var userType in (from userType in db.UserTypes where userType.UserTypeID > 3 select userType))
                {

                        types.Add(new SelectListItem()
                        {
                            Text = userType.UserType1,
                            Value = userType.UserTypeID + ""

                        });
                }

            }
            else if(request == "New")
            {
                foreach (var userType in (from userType in db.UserTypes where userType.UserTypeID > 2 select userType))
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


        public List<SelectListItem> getSchools()
        {
            var types = new List<SelectListItem>();
            var schools = (from school in db.Schools where school.DeletionDate == null select school).ToList();

            types.Add(new SelectListItem()
            {
                Text = null,
                Value = null
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
                Text = null,
                Value = null
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
            var outreachEntities = (from outreachEntity in db.OutreachEntityDetails join user in db.Users on outreachEntity.UserID equals user.UserID where outreachEntity.DeletionDate == null && user.UserStatus == true select outreachEntity).ToList();

            types.Add(new SelectListItem()
            {
                Text = null,
                Value = null
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
            if (Request.IsAuthenticated)
            {

                if (Boolean.Parse(User.Identity.Name.Split(',')[2]))
                {
                    EscuelasController controller = new EscuelasController();

                    UserViewModel userViewModel = new UserViewModel
                    {

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

                if (Boolean.Parse(User.Identity.Name.Split(',')[2]) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
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
                            userViewModel.Note.IsRead = false;

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

                if (Boolean.Parse(User.Identity.Name.Split(',')[2]) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    EscuelasController controller = new EscuelasController();

                    UserViewModel userViewModel = new UserViewModel
                    {

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

                if (Int32.Parse(User.Identity.Name.Split(',')[0]) == userNote.UserID || Int32.Parse(User.Identity.Name.Split(',')[0]) == userNote.SubjectID || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(User.Identity.Name.Split(',')[2]))
                {

                
                    UserViewModel userViewModel = new UserViewModel
                    {
                        Note = userNote,
                        UserDetail = db.UserDetails.FirstOrDefault(u => u.UserID == userNote.SubjectID)
                        
                    };

                    var user = db.Users.Find(userNote.UserID);

                    if (user.UserTypeID != 3)
                    {
                        var userDetail = db.UserDetails.FirstOrDefault(u => u.UserID == user.UserID);

                        if(userDetail.MiddleInitial != null)
                            userViewModel.Sender = userDetail.FirstName + " " + userDetail.MiddleInitial + " " + userDetail.LastName;
                        else
                            userViewModel.Sender = userDetail.FirstName + " " + userDetail.LastName;
                        
                    }
                    else
                    {
                        var outreachEntity = db.OutreachEntityDetails.FirstOrDefault(u => u.UserID == user.UserID);
                        userViewModel.Sender = outreachEntity.OutreachEntityName;
                    }

                    if (Int32.Parse(User.Identity.Name.Split(',')[0]) == userNote.SubjectID && !userNote.IsRead)
                    {
                        userViewModel.Note.IsRead = true;
                        db.Entry(userNote).CurrentValues.SetValues(userViewModel.Note);
                        db.SaveChanges();                        
                    }



                    if ((Int32.Parse(User.Identity.Name.Split(',')[0]) == userNote.UserID || (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Int32.Parse(User.Identity.Name.Split(',')[0]) != userNote.SubjectID)) && Boolean.Parse(User.Identity.Name.Split(',')[2]))
                       {
                            userViewModel.Reader = "Emisor";
                        }

                        else if (Int32.Parse(User.Identity.Name.Split(',')[0]) == userNote.SubjectID && Boolean.Parse(User.Identity.Name.Split(',')[2]))
                        {
                            userViewModel.Reader = "Receptor";
                        }


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
            User matchingUser = (from user in db.Users where (String.Compare(user.Email, userViewModel.User.Email, true) == 0) && user.DeletionDate == null select user).FirstOrDefault();

            if (matchingUser != null)
            {
                var crypto = new SimpleCrypto.PBKDF2();
                crypto.HashIterations = 5000;
                string hashedLoginPass = crypto.Compute(userViewModel.User.UserPassword, matchingUser.PasswordSalt);

                if (!matchingUser.UserStatus)
                {
                    ModelState.AddModelError("", Resources.WebResources.User_Email_NotApproved);

                    return Content(GetErrorsFromModelState(userViewModel));
                }

                //if (user.UserPassword == userViewModel.User.UserPassword)
                if (matchingUser.UserPassword == hashedLoginPass)
                {
                    FormsAuthentication.SetAuthCookie(matchingUser.UserID + "," + matchingUser.UserTypeID + "," + matchingUser.UserStatus, false);

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

        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Admin Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult CrearUsuario()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    UserViewModel userViewModel = new UserViewModel
                    {

                        UserTypes = getUserTypes("Admin")

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
                        bool validModel = true;

                        User matchingUser = (from user in db.Users where (String.Compare(user.Email, userViewModel.User.Email, true) == 0) && user.DeletionDate == null select user).FirstOrDefault();

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

                        if (userViewModel.UserDetail.FirstName == null || userViewModel.UserDetail.FirstName == "")
                        {
                            ModelState.AddModelError("UserDetail_FirstName_Required", Resources.WebResources.UserDetail_FirstName_Required);
                            validModel = false;
                        }
                        else
                        {
                            string pattern = @"^[a-zA-Z\u00c0-\u017e']+[-]?[a-zA-Z\u00c0-\u017e]+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.FirstName);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_FirstName_Invalid", Resources.WebResources.UserDetail_FirstName_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.UserDetail.FirstName.Length > 50)
                            {
                                ModelState.AddModelError("UserDetail_FirstName_LengthExceeded", Resources.WebResources.UserDetail_FirstName_LengthExceeded);
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
                            string pattern = @"^[a-zA-Z\u00c0-\u017e']+[-]?[a-zA-Z\u00c0-\u017e]+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.LastName);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_LastName_Invalid", Resources.WebResources.UserDetail_LastName_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.UserDetail.LastName.Length > 50)
                            {
                                ModelState.AddModelError("UserDetail_LastName_LengthExceeded", Resources.WebResources.UserDetail_LastName_LengthExceeded);
                                validModel = false;
                            }
                        }

                        if (userViewModel.UserDetail.Gender == null || userViewModel.UserDetail.Gender == "")
                        {
                            ModelState.AddModelError("UserDetail_Gender_Required", Resources.WebResources.UserDetail_Gender_Required);
                            validModel = false;
                        }

                        if (userViewModel.UserDetail.MiddleInitial != null && userViewModel.UserDetail.MiddleInitial != "")
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

                        if (userViewModel.UserDetail.Role != null && userViewModel.UserDetail.Role != "")
                        {
                            string pattern = @"^[a-zA-Z\u00c0-\u017e\s]+[-]?[a-zA-Z\u00c0-\u017e\s]+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Role);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_Role_Invalid", Resources.WebResources.UserDetail_Role_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.UserDetail.Role.Length > 100)
                            {
                                ModelState.AddModelError("UserDetail_Role_LengthExceeded", Resources.WebResources.UserDetail_Role_LengthExceeded);
                                validModel = false;
                            }

                        }

                        if (userViewModel.BirthDate != null && userViewModel.BirthDate != "")
                        {
                            string pattern = @"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.BirthDate);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_DateOfBirth_Invalid", Resources.WebResources.UserDetail_DateOfBirth_Invalid);
                                validModel = false;
                            }
                            else
                            {
                                userViewModel.UserDetail.DateOfBirth = DateTime.ParseExact(userViewModel.BirthDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            }

                        }

                        if (userViewModel.UserDetail.Major != null && userViewModel.UserDetail.Major != "")
                        {
                            string pattern = @"^[a-zA-Z\u00c0-\u017e\s]+[-]?[a-zA-Z\u00c0-\u017e\s]+$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Major);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_Major_Invalid", Resources.WebResources.UserDetail_Major_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.UserDetail.Major.Length > 100)
                            {
                                ModelState.AddModelError("UserDetail_Major_LengthExceeded", Resources.WebResources.UserDetail_Major_LengthExceeded);
                                validModel = false;
                            }
                        }

                        if (userViewModel.UserDetail.Grade != null && userViewModel.UserDetail.Grade != "")
                        {
                            string pattern = @"^(([1]?[0-2])|[1-9]|[K])$";
                            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            MatchCollection matches = rgx.Matches(userViewModel.UserDetail.Grade);
                            if (matches.Count == 0)
                            {
                                ModelState.AddModelError("UserDetail_Grade_Invalid", Resources.WebResources.UserDetail_Grade_Invalid);
                                validModel = false;
                            }
                            else if (userViewModel.UserDetail.Grade.Length > 2)
                            {
                                ModelState.AddModelError("UserDetail_Grade_LengthExceeded", Resources.WebResources.UserDetail_Grade_LengthExceeded);
                                validModel = false;
                            }
                        }

                        if (validModel)
                        {
                            var crypto = new SimpleCrypto.PBKDF2();
                            crypto.HashIterations = 5000;
                            string passwordHash = crypto.Compute(userViewModel.User.UserPassword);

                            userViewModel.User.UserPassword = passwordHash;
                            userViewModel.User.PasswordSalt = crypto.Salt;

                            userViewModel.UserDetail.CreateDate = DateTime.Now;
                            userViewModel.UserDetail.UpdateDate = DateTime.Now;
                            userViewModel.User.UserDetails = new List<UserDetail>();
                            userViewModel.User.UserDetails.Add(userViewModel.UserDetail);
                            userViewModel.User.UserStatus = true;
                            db.Users.Add(userViewModel.User);
                            db.SaveChanges();

                            return RedirectToAction("Administracion", "Home");
                        }
                    }

                    userViewModel.UserTypes = getUserTypes("Admin");
                    return View("CrearUsuario", userViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult ChangePasswordView(string source, int id)
        {
            var user = db.Users.Find(id);

            if (user == null)
                return Content("");

           

            UserViewModel userViewModel = new UserViewModel
            {
                User = user,
                Source = source,

            };

            if (Request.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                    userViewModel.Reader = "Admin";
                else
                    userViewModel.Reader = "User";
            }

            return PartialView("ChangePassword", userViewModel);
        }

        [HttpPost]
        public ActionResult ChangePassword(UserViewModel userViewModel)
        {
            // TODO: is it required to retrieve from the db the user again? it was done on the method that calls this Action
            bool validModel = true;
            var user = db.Users.Find(userViewModel.User.UserID);

            if (user != null)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) != 1) // TODO: added this so the system doesnt verify the old password if the current logged user is an admin
                {
                    if (userViewModel.OldPassword == null)
                    {
                        ModelState.AddModelError("User_Password_MissingPassword", Resources.WebResources.User_Password_MissingPassword);
                        validModel = false;
                    }
                    else
                    {
                        var crypto = new SimpleCrypto.PBKDF2();
                        crypto.HashIterations = 5000;
                        string passwordHash = crypto.Compute(userViewModel.OldPassword, user.PasswordSalt);

                        if (userViewModel.OldPassword != null && String.Compare(user.UserPassword, passwordHash) != 0)
                        {
                            ModelState.AddModelError("User_Password_IncorrectPassword", Resources.WebResources.User_Password_IncorrectPassword);
                            validModel = false;
                        }
                    }
                }

                

                if (userViewModel.ConfirmPassword != null && String.Compare(userViewModel.User.UserPassword, userViewModel.ConfirmPassword) != 0)
                {
                    ModelState.AddModelError("User_Password_NoMatch", Resources.WebResources.User_Password_NoMatch);
                    validModel = false;
                }

                if (validModel && ModelState.IsValid)
                {
                    var crypto = new SimpleCrypto.PBKDF2();
                    crypto.HashIterations = 5000;
                    string passwordHash = crypto.Compute(userViewModel.User.UserPassword);

                    userViewModel.User.UserPassword = passwordHash;
                    userViewModel.User.PasswordSalt = crypto.Salt;

                    userViewModel.User.UpdateDate = DateTime.Now;
                    db.Entry(user).CurrentValues.SetValues(userViewModel.User);
                    db.SaveChanges();

                    return View("_Hack");
                }
            }

            return Content(GetErrorsFromModelState(userViewModel));


        }
    }
}

