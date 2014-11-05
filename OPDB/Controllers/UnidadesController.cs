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

        public ActionResult Index(string requested)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse((User.Identity.Name.Split(',')[1])) == 1 && Boolean.Parse(requested))
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
                else
                {
                    return RedirectToAction("AccesoDenegado", "Home");
                }
            }
            else
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
        }

        [HttpPost]
        public ActionResult Detalles(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse((User.Identity.Name.Split(',')[1])) == 1)
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
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // GET: /Unidades/Create

        [HttpPost]
        public ActionResult PopUpCrear()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse((User.Identity.Name.Split(',')[1])) == 1)
                {
                    return PartialView("Crear");
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // POST: /Unidades/Create

        [HttpPost]
        public ActionResult Crear(UnitViewModel unitViewModel)
        {
              if (ModelState.IsValid)
                {

                    if (Request.IsAuthenticated)
                    {
                        if (Int32.Parse((User.Identity.Name.Split(',')[1])) == 1)
                        {
                            unitViewModel.Unit.CreateDate = DateTime.Now;
                            unitViewModel.Unit.UpdateDate = DateTime.Now;

                            unitViewModel.Unit.CreateUser = Int32.Parse(User.Identity.Name.Split(',')[0]);
                            unitViewModel.Unit.UpdateUser = Int32.Parse(User.Identity.Name.Split(',')[0]);

                            db.Units.Add(unitViewModel.Unit);
                            db.SaveChanges();

                            return View("_Hack");
                        }
                    }
                    
                }

                return Content(GetErrorsFromModelState(unitViewModel));
            

        }

        [HttpPost]
        public ActionResult PopUpEditar(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse((User.Identity.Name.Split(',')[1])) == 1)
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
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // POST: /Unidades/Edit/5

        [HttpPost]
        public ActionResult Editar(UnitViewModel unitViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse((User.Identity.Name.Split(',')[1])) == 1)
                {
                    if (ModelState.IsValid)
                    {
                        unitViewModel.Unit.UpdateUser = Int32.Parse(User.Identity.Name.Split(',')[0]);

                        unitViewModel.Unit.UpdateDate = DateTime.Now;
                        db.Entry(unitViewModel.Unit).State = EntityState.Modified;
                        db.SaveChanges();
                        return View("_Hack");
                    }

                    return Content(GetErrorsFromModelState(unitViewModel));
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // GET: /Unidades/Delete/5

        public ActionResult Remover(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse((User.Identity.Name.Split(',')[1])) == 1)
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
            }

            return RedirectToAction("AccesoDenegado", "Home");
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