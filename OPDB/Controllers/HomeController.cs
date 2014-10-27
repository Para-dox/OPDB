using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPDB.Models;
using System.Data.Linq.SqlClient;

namespace OPDB.Controllers
{
    
    public class HomeController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        /// <summary>
        /// The Home Page.
        /// </summary>
        /// <returns>The Home Index, also known as the web portal home page.</returns>
        public ActionResult Index()
        {
            DateTime date = DateTime.Now.AddDays(7);

            HomeViewModel homeViewModel = new HomeViewModel
            {

                Information = new List<UserInfoViewModel>()

            };

            var activities = (from activity in db.Activities.Include(a => a.ActivityType) where activity.ActivityDate > date && activity.DeletionDate == null orderby activity.UpdateDate descending select activity).Take(6).ToList();

            foreach (var activity in activities)
            {
                if (activity.ActivityDate == null)
                    activity.ActivityDate = new DateTime();

                if (activity.ActivityTime == null)
                    activity.ActivityTime = "";

                var interest = (from i in db.Interests where i.UserID == 1 && i.ActivityID == activity.ActivityID select i).ToList();
                bool interested = false;

                if (interest.Count == 1)
                    interested = true;

                homeViewModel.Information.Add(new UserInfoViewModel
                {
                    Activity = activity,
                    Interested = interested
                });
            }

            return View(homeViewModel);
        }

        /// <summary>
        /// The activities calendar.
        /// </summary>
        /// <returns>The activities calendar view.</returns>
        public ActionResult Calendario()
        {
            return View();
        }

        /// <summary>
        /// The contact web page.
        /// </summary>
        /// <returns>The view containing all contact information.</returns>
        public ActionResult Contact()
        {
            return View();
        }

        /// <summary>
        /// The global search function. Searches all available entities in the system for the provided text.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        /// <returns>Returns the Search Results view with the results of the search.</returns>
        [HttpPost]
        public ActionResult Buscar(string searchText)
        {
            SearchViewModel searchViewModel = new SearchViewModel();

            searchViewModel.buscarActividades = true;
            searchViewModel.buscarAlcance = true;
            searchViewModel.buscarEscuelas = true;
            searchViewModel.buscarUnidades = true;
            searchViewModel.buscarUsuarios = true;

            searchViewModel.users = from user in db.UserDetails
                                    where (user.FirstName.Contains(searchText)
                                        || user.LastName.Contains(searchText)) && user.DeletionDate == null
                                    select user;

            searchViewModel.outreachEntities = from outreachEntity in db.OutreachEntityDetails
                                               where (outreachEntity.OutreachEntityName.Contains(searchText)
                                                   || outreachEntity.Mission.Contains(searchText) || outreachEntity.Vision.Contains(searchText)
                                                   || outreachEntity.Objectives.Contains(searchText)) && outreachEntity.DeletionDate == null
                                               select outreachEntity;

            searchViewModel.activities = from activity in db.Activities
                                         where (activity.Title.Contains(searchText) || activity.Purpose.Contains(searchText))
                                         && activity.DeletionDate == null
                                         select activity;

            searchViewModel.schools = from school in db.Schools where school.SchoolName.Contains(searchText) && school.DeletionDate == null select school;

            searchViewModel.units = from unit in db.Units where unit.UnitName.Contains(searchText) && unit.DeletionDate == null select unit;

            return View(searchViewModel);


        }

        /// <summary>
        /// The search bar partial view.
        /// </summary>
        /// <returns>The partial view displayed in the layout menu.</returns>
        public ActionResult _Buscar()
        {
            SearchViewModel search = new SearchViewModel();
            return PartialView("_Buscar", search);
        }

        /// <summary>
        /// Retrieves the required advanced search partial view containing the entity search forms.
        /// </summary>
        /// <param name="view">The view to return.</param>
        /// <returns>The specified partial view.</returns>
        public ActionResult _PartialViewLoad(string view)
        {
            SearchViewModel searchViewModel = new SearchViewModel();

            if (view == "_Alcance")
            {
                UsuariosController controller = new UsuariosController();
                searchViewModel.types = controller.getOutreachTypes();
            }

            if (view == "_Usuarios")
            {
               searchViewModel.types = getUserTypes();
            }
            return PartialView(view, searchViewModel);

        }

        /// <summary>
        /// Retrieves the advanced search view.
        /// </summary>
        /// <returns>The advanced search view.</returns>
        public ActionResult BusquedaAvanzada()
        {

            return View();

        }

        /// <summary>
        /// Searches all available users in the system and retrieves those matching search parameters.
        /// </summary>
        /// <param name="searchViewModel">The view model containing the search parameters.</param>
        /// <returns>The search results partial view with the results of the search.</returns>
        [HttpPost]
        public ActionResult BuscarUsuarios(SearchViewModel searchViewModel)
        {
            searchViewModel.buscarUsuarios = true;
            
            
            var result = from user in db.UserDetails
                         where user.User.UserTypeID == searchViewModel.user.User.UserTypeID 
                         && user.DeletionDate == null
                         select user;

            searchViewModel.users = result;

            if (searchViewModel.user.FirstName != null)
            {

                result = from user in result
                         where user.FirstName.Contains(searchViewModel.user.FirstName) && user.DeletionDate == null
                         select user;

                searchViewModel.users = result;


            }

            if (searchViewModel.user.LastName != null)
            {
                result = from user in result
                         where user.LastName.Contains(searchViewModel.user.LastName) && user.DeletionDate == null
                         select user;

                searchViewModel.users = result;
            }

            if (searchViewModel.user.Role != null)
            {
                result = from user in result
                         where user.Role.Contains(searchViewModel.user.Role) && user.DeletionDate == null
                         select user;

                searchViewModel.users = result;
            }

            if (searchViewModel.user.Major != null)
            {
                result = from user in result
                         where user.Major.Contains(searchViewModel.user.Major) && user.DeletionDate == null
                         select user;

                searchViewModel.users = result;
            }


            
            return View("Buscar", searchViewModel);


        }

        /// <summary>
        /// Searches all available activities in the system and retrieves those matching search parameters.
        /// </summary>
        /// <param name="searchViewModel">The view model containing the search parameters.</param>
        /// <returns>The search results partial view with the results of the search.</returns>
        [HttpPost]
        public ActionResult BuscarActividades(SearchViewModel searchViewModel)
        {
            searchViewModel.buscarActividades = true;

            var result = from activity in db.Activities
                         where activity.DeletionDate == null 
                         select activity;

            searchViewModel.activities = result;

            if (searchViewModel.activity.Title != null)
            {
                result = from activity in result
                         where activity.Title.Contains(searchViewModel.activity.Title)
                         && activity.DeletionDate == null
                         select activity;

                searchViewModel.activities = result;
            }

            if(searchViewModel.activity.Purpose != null){

                result = from activity in result
                         where activity.Purpose.Contains(searchViewModel.activity.Purpose)
                         && activity.DeletionDate == null
                         select activity;

                searchViewModel.activities = result;
            }


            return View("Buscar", searchViewModel);
        }

        /// <summary>
        /// Searches all available outreach entities in the system and retrieves those matching search parameters.
        /// </summary>
        /// <param name="searchViewModel">The view model containing the search parameters.</param>
        /// <returns>The search results partial view with the results of the search.</returns>
        [HttpPost]
        public ActionResult BuscarAlcance(SearchViewModel searchViewModel)
        {
            searchViewModel.buscarAlcance = true;

            var result = from outreachEntity in db.OutreachEntityDetails
                         where outreachEntity.OutreachEntityTypeID == searchViewModel.outreachEntity.OutreachEntityTypeID
                         && outreachEntity.DeletionDate == null
                         select outreachEntity;

            searchViewModel.outreachEntities = result;

            if(searchViewModel.outreachEntity.OutreachEntityName != null){

                result = from outreachEntity in result
                         where outreachEntity.OutreachEntityName.Contains(searchViewModel.outreachEntity.OutreachEntityName)
                         && outreachEntity.DeletionDate == null
                         select outreachEntity;

                searchViewModel.outreachEntities = result;
            }

            if(searchViewModel.outreachEntity.Mission != null){

                result = from outreachEntity in result
                         where outreachEntity.Mission.Contains(searchViewModel.outreachEntity.Mission)
                         && outreachEntity.DeletionDate == null
                         select outreachEntity;

                searchViewModel.outreachEntities = result;

            }

            if (searchViewModel.outreachEntity.Vision != null)
            {

                result = from outreachEntity in result
                         where outreachEntity.Vision.Contains(searchViewModel.outreachEntity.Vision)
                         && outreachEntity.DeletionDate == null
                         select outreachEntity;

                searchViewModel.outreachEntities = result;
            }

            if (searchViewModel.outreachEntity.Objectives != null)
            {

                result = from outreachEntity in result
                         where outreachEntity.Objectives.Contains(searchViewModel.outreachEntity.Objectives)
                         && outreachEntity.DeletionDate == null
                         select outreachEntity;

                searchViewModel.outreachEntities = result;

            }


            return View("Buscar", searchViewModel);
        }

        /// <summary>
        /// Searches all available schools in the system and retrieves those matching search parameters.
        /// </summary>
        /// <param name="searchViewModel">The view model containing the search parameters.</param>
        /// <returns>The search results partial view with the results of the search.</returns>
        [HttpPost]
        public ActionResult BuscarEscuelas(SearchViewModel searchViewModel)
        {
            searchViewModel.buscarEscuelas = true;

            var result = from school in db.Schools
                         where school.DeletionDate == null
                         select school;

            searchViewModel.schools = result;

            if (searchViewModel.school.SchoolName != null)
            {
                result = from school in result
                         where school.SchoolName.Contains(searchViewModel.school.SchoolName)
                         && school.DeletionDate == null
                         select school;

                searchViewModel.schools = result;
            }

            if (searchViewModel.school.Town != null)
            {
                result = from school in result
                         where school.Town.Contains(searchViewModel.school.Town) 
                         && school.DeletionDate == null
                         select school;

                searchViewModel.schools = result;
            }


            return View("Buscar", searchViewModel);
        }

        /// <summary>
        /// Searches all available units in the system and retrieves those matching search parameters.
        /// </summary>
        /// <param name="searchViewModel">The view model containing the search parameters.</param>
        /// <returns>The search results partial view with the results of the search.</returns>
        [HttpPost]
        public ActionResult BuscarUnidades(SearchViewModel searchViewModel)
        {
            searchViewModel.buscarUnidades = true;

            searchViewModel.units = from unit in db.Units where unit.UnitName.Contains(searchViewModel.unit.UnitName ?? "") select unit;
            
            return View("Buscar", searchViewModel);
        }

        public ActionResult Administracion()
        {
            return View();
        }

        public List<SelectListItem> getUserTypes()
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
    }
}
