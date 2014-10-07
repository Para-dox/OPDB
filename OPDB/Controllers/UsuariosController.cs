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
            var users = db.Users.Include(u => u.UserType);
            return View(users.ToList());
        }

        //
        // GET: /Usuarios/Details/5

        public ActionResult Detalles(int id = 0)
        {
            User user = db.Users.Find(id);
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
           
            UserViewModel user = new UserViewModel{userTypes = getUserTypes()};            
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
                    userViewModel.user.UserStatus = true;
                    userViewModel.userDetail.CreateDate = DateTime.Now;
                    userViewModel.userDetail.UpdateDate = DateTime.Now;
                    userViewModel.user.UserDetails = new List<UserDetail>();
                    userViewModel.user.UserDetails.Add(userViewModel.userDetail);
                    db.Users.Add(userViewModel.user);
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

            //ViewBag.UserTypeID = new SelectList(db.UserTypes, "UserTypeID", "UserType1", userViewModel.user.UserTypeID);
            userViewModel.userTypes = getUserTypes();
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
                userDetail = db.UserDetails.FirstOrDefault(i => i.UserID == id),
                userTypes = getUserTypes()

            };

            if (user == null)
            {
                return HttpNotFound();
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

        public ActionResult Remover(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Usuarios/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
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
    }
}