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
    public class UnidadesController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Unidades/

        public ActionResult Index()
        {
            List<Unit> units = (from unit in db.Units where unit.DeletionDate == null select unit).ToList();

            UnitViewModel unitViewModel = new UnitViewModel
            {
                Information = new List<UserInfoViewModel>()
            };


            foreach (var unit in units)
            {
                unitViewModel.Information.Add(new UserInfoViewModel
                {
                    Unit = unit,
                    CreateUser = db.UserDetails.First(u => u.UserID == unit.CreateUser),
                    UpdateUser = db.UserDetails.First(u => u.UserID == unit.UpdateUser)

                });
            }

            return PartialView("Index", unitViewModel);
        }

        [HttpPost]
        public ActionResult Detalles(int id = 0)
        {
            UnitViewModel unitViewModel = new UnitViewModel
            {
                Unit = db.Units.Find(id)
            };

            if (unitViewModel.Unit == null)
            {
                return HttpNotFound();
            }

            return PartialView("Detalles", unitViewModel);
        }

        //
        // GET: /Unidades/Create

        [HttpPost]
        public ActionResult PopUpCrear()
        {
            return PartialView("Crear");
        }

        //
        // POST: /Unidades/Create

        [HttpPost]
        public ActionResult Crear(UnitViewModel unitViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitViewModel.Unit.CreateDate = DateTime.Now;
                    unitViewModel.Unit.UpdateDate = DateTime.Now;

                    // Change after login implementation.
                    unitViewModel.Unit.CreateUser = 1;
                    unitViewModel.Unit.UpdateUser = 1;

                    db.Units.Add(unitViewModel.Unit);
                    db.SaveChanges();

                    return View("_Hack");
                }

                return Content(GetErrorsFromModelState(unitViewModel));
            }

            catch (Exception)
            {
                return Content(GetErrorsFromModelState(unitViewModel));
            }
        }

        [HttpPost]
        public ActionResult PopUpEditar(int id = 0)
        {
            UnitViewModel unitViewModel = new UnitViewModel
            {
                Unit = db.Units.Find(id)
            };

            if (unitViewModel.Unit == null)
            {
                return HttpNotFound();
            }

            return PartialView("Editar", unitViewModel);
        }

        //
        // POST: /Unidades/Edit/5

        [HttpPost]
        public ActionResult Editar(UnitViewModel unitViewModel)
        {
            if (ModelState.IsValid)
            {
                //To be changed with login.
                unitViewModel.Unit.UpdateUser = 1;

                unitViewModel.Unit.UpdateDate = DateTime.Now;
                db.Entry(unitViewModel.Unit).State = EntityState.Modified;
                db.SaveChanges();
                return View("_Hack");
            }

            return Content(GetErrorsFromModelState(unitViewModel));   
        }

        //
        // GET: /Unidades/Delete/5

        public ActionResult Remover(int id = 0)
        {
            Unit unit = db.Units.Find(id);

            if (unit == null)
            {
                return HttpNotFound();
            }
            else
            {
                unit.DeletionDate = DateTime.Now;
                db.Entry(unit).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        public String GetErrorsFromModelState(UnitViewModel unitViewModel)
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

        public ActionResult Lista()
        {
            var units = from unit in db.Units where unit.DeletionDate == null orderby unit.UnitName ascending select unit;

            return View(units.ToList());
        }
    }
}