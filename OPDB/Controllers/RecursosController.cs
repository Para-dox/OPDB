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
    public class RecursosController : Controller
    {
        /// <summary>
        /// A private instance of the OPDB Database model.
        /// </summary>
        private OPDBEntities db = new OPDBEntities();

       
        /// <summary>
        /// The resource index, returns the system admin view.
        /// </summary>
        /// <returns>The system admin view for resources.</returns>
        public ActionResult Index()
        {
            var resources = db.Resources.Include(r => r.Unit);
            return View(resources.ToList());
        }

        /// <summary>
        /// Displays the resource detail page.
        /// </summary>
        /// <param name="id">The id of the resource to retrieve from the database.</param>
        /// <returns>The specified resource's details page.</returns>
        public ActionResult Detalles(int id = 0)
        {
            Resource r = db.Resources.Find(id);

            ResourceViewModel resourceViewModel = new ResourceViewModel
            {
                resource = r,
                unit = db.Units.Find(r.UnitID),
                units = getUnits()
            };

            if (resourceViewModel.resource == null)
            {
                return HttpNotFound();
            }
            //return View(resourceViewModel);
            return PartialView("Detalles", resourceViewModel);
        }

       
        /// <summary>
        /// Create resource view.
        /// </summary>
        /// <returns>The create new resource view.</returns>
        public ActionResult Crear()
        {
            ResourceViewModel resourceViewModel = new ResourceViewModel
            {
                units = getUnits()
            };
            return View(resourceViewModel);
        }

       
        /// <summary>
        /// Create a new resource.
        /// </summary>
        /// <param name="resourceViewModel">The ResourceViewModel containing the information of the resource to be created.</param>
        /// <returns>The resource admin view if successful, the create new resource view if an error occurs.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(ResourceViewModel resourceViewModel)
        {
            if (ModelState.IsValid)
            {
                resourceViewModel.resource.CreateDate = DateTime.Now;
                resourceViewModel.resource.UpdateDate = DateTime.Now;

                // Change after login implementation.
                resourceViewModel.resource.CreateUser = 1;
                resourceViewModel.resource.UpdateUser = 1;

                db.Resources.Add(resourceViewModel.resource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(resourceViewModel);
        }

      
        /// <summary>
        /// Edit resource view.
        /// </summary>
        /// <param name="id">The id of the resource to be edited.</param>
        /// <returns>The edit view with the specified resource.</returns>
        public ActionResult Editar(int id = 0)
        {

            ResourceViewModel resourceViewModel = new ResourceViewModel
            {
                resource = db.Resources.Find(id),
                units = getUnits()

            };

            if (resourceViewModel.resource == null)
            {
                return HttpNotFound();
            }

            //return View(resourceViewModel);
            return PartialView("Editar", resourceViewModel);
        }

      
        /// <summary>
        /// Edit a resource.
        /// </summary>
        /// <param name="resourceViewModel">The ResourceViewModel containing the edited resource parameters.</param>
        /// <returns>The admin resource page if successful, the edit resource view if an error occurs.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ResourceViewModel resourceViewModel)
        {
            if (ModelState.IsValid)
            {
                resourceViewModel.resource.UpdateDate = DateTime.Now;
                db.Entry(resourceViewModel.resource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(resourceViewModel);
        }

        /// <summary>
        /// Remove a resource from the system.
        /// </summary>
        /// <param name="id">The id of the resource to be removed.</param>
        /// <returns>The admin resource page if successful, a "Resource Not Found" page if an error occurs.</returns>
        [HttpPost]
        public ActionResult Remover(int id = 0)
        {
            Resource resource = db.Resources.Find(id);
            if (resource == null)
            {
                return HttpNotFound();
            }
            else
            {
                resource.DeletionDate = DateTime.Now;
                db.Entry(resource).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Retrieve all available Units from the database.
        /// </summary>
        /// <returns>A list containing select list items representing the system Units.</returns>
        public List<SelectListItem> getUnits()
        {
            List<SelectListItem> types = new List<SelectListItem>();
            foreach (var unit in db.Units)
            {

                types.Add(new SelectListItem()
                {
                    Text = unit.UnitName,
                    Value = unit.UnitID + ""

                });

            }

            return types;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}