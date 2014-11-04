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
        public ActionResult Index(string requested){

            if (requested != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                    {
            List<Resource> resources = (from resource in db.Resources where resource.DeletionDate == null select resource).ToList();
            
            ResourceViewModel resourceViewModel = new ResourceViewModel
            {
                Information = new List<UserInfoViewModel>()
            };


            foreach (var resource in resources)
            {
                resourceViewModel.Information.Add(new UserInfoViewModel
                {
                    Resource = resource,
                    CreateUser = db.UserDetails.First(r => r.UserID == resource.CreateUser),
                    UpdateUser = db.UserDetails.First(r => r.UserID == resource.UpdateUser)

                });
            }

           return PartialView("Index", resourceViewModel);
        }
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        /// <summary>
        /// Displays the resource detail page.
        /// </summary>
        /// <param name="id">The id of the resource to retrieve from the database.</param>
        /// <returns>The specified resource's details page.</returns>
        [HttpPost]
        public ActionResult Detalles(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
            Resource resource = db.Resources.Find(id);

            ResourceViewModel resourceViewModel = new ResourceViewModel
            {
                Resource = resource,
                Unit = db.Units.Find(resource.UnitID),
                Units = getUnits()
            };

            if (resourceViewModel.Resource == null)
            {
                return HttpNotFound();
            }

            return PartialView("Detalles", resourceViewModel);
        }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }
       
       
        /// <summary>
        /// Create resource view.
        /// </summary>
        /// <returns>The create new resource view.</returns>
        [HttpPost]
        public ActionResult PopUpCrear()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
            ResourceViewModel resourceViewModel = new ResourceViewModel
            {
                Units = getUnits()
            };
            return PartialView("Crear", resourceViewModel);
        }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }
       
       
        /// <summary>
        /// Create a new resource.
        /// </summary>
        /// <param name="resourceViewModel">The ResourceViewModel containing the information of the resource to be created.</param>
        /// <returns>The resource admin view if successful, the create new resource view if an error occurs.</returns>
        [HttpPost]
        public ActionResult Crear(ResourceViewModel resourceViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);
           
                    try
                    {
                if (ModelState.IsValid)
                {
                    resourceViewModel.Resource.CreateDate = DateTime.Now;
                    resourceViewModel.Resource.UpdateDate = DateTime.Now;

                    // Change after login implementation.
                            resourceViewModel.Resource.CreateUser = userID;
                            resourceViewModel.Resource.UpdateUser = userID;

                    db.Resources.Add(resourceViewModel.Resource);
                    db.SaveChanges();
                    return View("_Hack");
                }

                return Content(GetErrorsFromModelState(resourceViewModel));               
            }
                        
                    catch (Exception)
            {
                return Content(GetErrorsFromModelState(resourceViewModel));
            }
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");

        }

      
        /// <summary>
        /// Edit resource view.
        /// </summary>
        /// <param name="id">The id of the resource to be edited.</param>
        /// <returns>The edit view with the specified resource.</returns>
        [HttpPost]
        public ActionResult PopUpEditar(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {

            ResourceViewModel resourceViewModel = new ResourceViewModel
            {
                Resource = db.Resources.Find(id),
                Units = getUnits()

            };

            if (resourceViewModel.Resource == null)
            {
                return HttpNotFound();
            }

             
            return PartialView("Editar", resourceViewModel);
        }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

      
        /// <summary>
        /// Edit a resource.
        /// </summary>
        /// <param name="resourceViewModel">The ResourceViewModel containing the edited resource parameters.</param>
        /// <returns>The admin resource page if successful, the edit resource view if an error occurs.</returns>
        [HttpPost]
        public ActionResult Editar(ResourceViewModel resourceViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

            if (ModelState.IsValid)
            {
                //To be changed with login.
                        resourceViewModel.Resource.UpdateUser = userID;

                resourceViewModel.Resource.UpdateDate = DateTime.Now;                
                db.Entry(resourceViewModel.Resource).State = EntityState.Modified;
                db.SaveChanges();
                return View("_Hack");
            }

            return Content(GetErrorsFromModelState(resourceViewModel));   
        }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        /// <summary>
        /// Remove a resource from the system.
        /// </summary>
        /// <param name="id">The id of the resource to be removed.</param>
        /// <returns>The admin resource page if successful, a "Resource Not Found" page if an error occurs.</returns>
        [HttpPost]
        public ActionResult Remover(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
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
            }

            return RedirectToAction("AccesoDenegado", "Home");
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

        public String GetErrorsFromModelState(ResourceViewModel resourceViewModel)
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