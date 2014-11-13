using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPDB.Models;
using System.Data.Linq.SqlClient;
using System.Text;
using System.Globalization;

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
            User currentUser = null;

            if (User.Identity.IsAuthenticated)
            {
                int currentUserID = Int32.Parse(User.Identity.Name.Split(',')[0]);
                currentUser = db.Users.Find(currentUserID);
            }
            else
            {
                currentUser = null;
            }

            HomeViewModel homeViewModel = new HomeViewModel
            {
                Information = new List<UserInfoViewModel>()
            };

            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now.AddDays(7);

            var activities = (from activity in db.Activities.Include(a => a.ActivityType) where ((activity.ActivityDate >= start) && (activity.ActivityDate <= end)) && activity.DeletionDate == null orderby activity.UpdateDate descending select activity).Take(6).ToList();
           

            foreach (var activity in activities)
            {
                if (activity.ActivityDate == null)
                    activity.ActivityDate = new DateTime();

                if (activity.ActivityTime == null)
                    activity.ActivityTime = "";

                List<OPDB.Models.Interest> interest = new List<OPDB.Models.Interest>();

                if (currentUser !=  null)
                {
                    interest = (from i in db.Interests where i.UserID == currentUser.UserID && i.ActivityID == activity.ActivityID select i).ToList();
                }

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

            searchViewModel.BuscarActividades = true;
            searchViewModel.BuscarAlcance = true;
            searchViewModel.BuscarEscuelas = true;
            searchViewModel.BuscarUnidades = true;
            searchViewModel.BuscarUsuarios = true;

            searchViewModel.Users = from userDetail in db.UserDetails join user in db.Users on userDetail.UserID equals user.UserID 
                                    join userType in db.UserTypes on user.UserTypeID equals userType.UserTypeID
                                    where (userDetail.FirstName.Contains(searchText)
                                        || userDetail.LastName.Contains(searchText) || 
                                        userType.UserType1.Contains(searchText)) && 
                                        userDetail.DeletionDate == null
                                    select userDetail;

            searchViewModel.OutreachEntities = from outreachEntity in db.OutreachEntityDetails join outreachType in db.OutreachEntityTypes on outreachEntity.OutreachEntityTypeID equals outreachType.OutreachEntityTypeID
                                               where (outreachEntity.OutreachEntityName.Contains(searchText)
                                                   || outreachEntity.Mission.Contains(searchText) || outreachEntity.Vision.Contains(searchText)
                                                   || outreachEntity.Objectives.Contains(searchText) || outreachType.OutreachEntityType1.Contains(searchText)) && outreachEntity.DeletionDate == null
                                               select outreachEntity;

            searchViewModel.Activities = from activity in db.Activities
                                         where (activity.Title.Contains(searchText) || activity.Purpose.Contains(searchText) ||
                                         activity.ActivityMajor.ActivityMajor1.Contains(searchText) || (activity.ActivityDynamic.ActivityDynamic1 ?? "").Contains(searchText))
                                         && activity.DeletionDate == null
                                         select activity;

            searchViewModel.Schools = from school in db.Schools
                                      where (school.SchoolName.Contains(searchText)
                                          || school.Address.Contains(searchText)
                                          || school.SchoolRegion.SchoolRegion1.Contains(searchText)) 
                                          && school.DeletionDate == null select school;

            searchViewModel.Units = from unit in db.Units where unit.UnitName.Contains(searchText) && unit.DeletionDate == null select unit;

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
                searchViewModel.Types = getOutreachTypes();
            }

            if (view == "_Usuarios")
            {
               searchViewModel.Types = getUserTypes();
            }

            if (view == "_Actividades")
            {
                searchViewModel.Types = getActivityTypes();
                searchViewModel.Majors = getActivityMajors();
                searchViewModel.Dynamics = getActivityDynamics();
            }

            if (view == "_Escuelas")
            {
                searchViewModel.Towns = getTowns();
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
            searchViewModel.BuscarUsuarios = true;
            
            
            var result = from user in db.UserDetails
                         where user.DeletionDate == null
                         select user;

            searchViewModel.Users = result;

            if (searchViewModel.User.User.UserTypeID != 0)
            {

                result = from user in result
                         where user.User.UserTypeID == searchViewModel.User.User.UserTypeID
                         select user;

                searchViewModel.Users = result;
            }

            if (searchViewModel.User.FirstName != null)
            {

                result = from user in result
                         where user.FirstName.Contains(searchViewModel.User.FirstName) && user.DeletionDate == null
                         select user;

                searchViewModel.Users = result;
            }

            if (searchViewModel.User.LastName != null)
            {
                result = from user in result
                         where user.LastName.Contains(searchViewModel.User.LastName) && user.DeletionDate == null
                         select user;

                searchViewModel.Users = result;
            }

            if (searchViewModel.User.Role != null)
            {
                result = from user in result
                         where user.Role.Contains(searchViewModel.User.Role) && user.DeletionDate == null
                         select user;

                searchViewModel.Users = result;
            }

            if (searchViewModel.User.Major != null)
            {
                result = from user in result
                         where user.Major.Contains(searchViewModel.User.Major) && user.DeletionDate == null
                         select user;

                searchViewModel.Users = result;
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
            searchViewModel.BuscarActividades = true;

            var result = from activity in db.Activities
                         where activity.DeletionDate == null 
                         select activity;

            searchViewModel.Activities = result;

            if (searchViewModel.Activity.ActivityTypeID != 0)
            {
                result = from activity in result
                         where activity.ActivityTypeID == searchViewModel.Activity.ActivityTypeID
                         && activity.DeletionDate == null
                         select activity;

                searchViewModel.Activities = result;
            }

            if (searchViewModel.Activity.ActivityMajorID != 0)
            {
                result = from activity in result
                         where activity.ActivityMajorID == searchViewModel.Activity.ActivityMajorID
                         && activity.DeletionDate == null
                         select activity;

                searchViewModel.Activities = result;
            }

            if (searchViewModel.Activity.ActivityDynamicID != null && searchViewModel.Activity.ActivityDynamicID != 0)
            {
                result = from activity in result
                         where activity.ActivityDynamicID == searchViewModel.Activity.ActivityDynamicID
                         && activity.DeletionDate == null
                         select activity;

                searchViewModel.Activities = result;
            }

            if (searchViewModel.Activity.Title != null)
            {
                result = from activity in result
                         where activity.Title.Contains(searchViewModel.Activity.Title)
                         && activity.DeletionDate == null
                         select activity;

                searchViewModel.Activities = result;
            }

            if(searchViewModel.Activity.Purpose != null){

                result = from activity in result
                         where activity.Purpose.Contains(searchViewModel.Activity.Purpose)
                         && activity.DeletionDate == null
                         select activity;

                searchViewModel.Activities = result;
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
            searchViewModel.BuscarAlcance = true;

            var result = from outreachEntity in db.OutreachEntityDetails
                         where outreachEntity.DeletionDate == null
                         select outreachEntity;

            searchViewModel.OutreachEntities = result;

            if (searchViewModel.OutreachEntity.OutreachEntityTypeID != 0)
            {

                result = from outreachEntity in result
                         where outreachEntity.OutreachEntityTypeID == searchViewModel.OutreachEntity.OutreachEntityTypeID
                         && outreachEntity.DeletionDate == null
                         select outreachEntity;

                searchViewModel.OutreachEntities = result;
            }

            if (searchViewModel.OutreachEntity.OutreachEntityName != null)
            {

                result = from outreachEntity in result
                         where outreachEntity.OutreachEntityName.Contains(searchViewModel.OutreachEntity.OutreachEntityName)
                         && outreachEntity.DeletionDate == null
                         select outreachEntity;

                searchViewModel.OutreachEntities = result;
            }

            if (searchViewModel.OutreachEntity.Mission != null)
            {

                result = from outreachEntity in result
                         where outreachEntity.Mission.Contains(searchViewModel.OutreachEntity.Mission)
                         && outreachEntity.DeletionDate == null
                         select outreachEntity;

                searchViewModel.OutreachEntities = result;

            }

            if (searchViewModel.OutreachEntity.Vision != null)
            {

                result = from outreachEntity in result
                         where outreachEntity.Vision.Contains(searchViewModel.OutreachEntity.Vision)
                         && outreachEntity.DeletionDate == null
                         select outreachEntity;

                searchViewModel.OutreachEntities = result;
            }

            if (searchViewModel.OutreachEntity.Objectives != null)
            {

                result = from outreachEntity in result
                         where outreachEntity.Objectives.Contains(searchViewModel.OutreachEntity.Objectives)
                         && outreachEntity.DeletionDate == null
                         select outreachEntity;

                searchViewModel.OutreachEntities = result;

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
            searchViewModel.BuscarEscuelas = true;

            var result = from school in db.Schools
                         where school.DeletionDate == null
                         select school;

            searchViewModel.Schools = result;

            if (searchViewModel.School.SchoolName != null)
            {
                result = from school in result
                         where school.SchoolName.Contains(searchViewModel.School.SchoolName)
                         && school.DeletionDate == null
                         select school;

                searchViewModel.Schools = result;
            }

            if (searchViewModel.School.Address != null)
            {
                result = from school in result
                         where school.Address.Contains(searchViewModel.School.Address)
                         && school.DeletionDate == null
                         select school;

                searchViewModel.Schools = result;
            }

            if (searchViewModel.School.Town != null)
            {
                result = from school in result
                         where school.Town.Contains(searchViewModel.School.Town) 
                         && school.DeletionDate == null
                         select school;

                searchViewModel.Schools = result;
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
            searchViewModel.BuscarUnidades = true;

            searchViewModel.Units = from unit in db.Units where unit.UnitName.Contains(searchViewModel.Unit.UnitName ?? "") select unit;
            
            return View("Buscar", searchViewModel);
        }

        public ActionResult Administracion()
        {
            return View();
        }

        public List<SelectListItem> getUserTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();

            types.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });

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

        public JsonResult CalendarData()
        {
            List<CalendarActivity> events = new List<CalendarActivity>();
            List<Activity> activities = (from activity in db.Activities.Include(a => a.ActivityType) where activity.DeletionDate == null select activity).ToList();
            
            foreach (Activity a in activities)
            {
                string startDate = "";
                string startTime = "";
                string startDateTime = "";

                if (a.ActivityDate != null)
                {
                    startDate = Convert.ToDateTime(a.ActivityDate.Value.ToString()).ToString("yyyy-MM-ddTHH:mm:ss");
                }
                else
                {
                    startDate = null;
                }

                if (a.ActivityTime != null)
                {
                    startTime = DateTime.Parse(a.ActivityTime.ToString()).ToString("HH:mm:ss");
                }
                else
                {
                    startTime = null;
                }

                if (startDate != null)
                {
                    startDateTime = startDate.Substring(0, 11) + startTime;
                }
                else
                {
                    startDateTime = null;
                }

                CalendarActivity newEvent = new CalendarActivity
                {
                    id = a.ActivityID + "",
                    title = a.Title.ToString(),
                    start = startDateTime,
                    url = Url.Action("Detalles", "Actividades", new { id = a.ActivityID }),
                    allDay = false,
                };

                events.Add(newEvent);
            }

            return Json(events, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> getOutreachTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();

            types.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });

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

        public List<SelectListItem> getActivityTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();

            types.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });

            foreach (var activityType in db.ActivityTypes)
            {
                types.Add(new SelectListItem()
                {
                    Text = activityType.ActivityType1,
                    Value = activityType.ActivityTypeID + ""

                });

            }

            return types;
        }

        public List<SelectListItem> getActivityMajors()
        {
            List<SelectListItem> types = new List<SelectListItem>();

            types.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });

            foreach (var activityMajor in (from major in db.ActivityMajors orderby major.ActivityMajor1 ascending select major).ToList())
            {
                types.Add(new SelectListItem()
                {
                    Text = activityMajor.ActivityMajor1,
                    Value = activityMajor.ActivityMajorID + ""

                });

            }

            return types;
        }

        public List<SelectListItem> getActivityDynamics()
        {
            List<SelectListItem> types = new List<SelectListItem>();

            types.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });

            foreach (var activityDynamic in db.ActivityDynamics)
            {
                types.Add(new SelectListItem()
                {
                    Text = activityDynamic.ActivityDynamic1,
                    Value = activityDynamic.ActivityDynamicID + ""

                });

            }

            return types;
        }

        public List<SelectListItem> getTowns()
        {
            List<SelectListItem> towns = new List<SelectListItem>();

            String[] town = new String[] { "", "Adjuntas", "Aguada", "Aguadilla", "Aguas Buenas", "Aibonito", "Añasco",
            "Arecibo", "Arroyo", "Barceloneta", "Barranquitas", "Bayamón", "Cabo Rojo", "Caguas", "Camuy", "Canóvanas",
            "Carolina", "Cataño", "Cayey", "Ceiba", "Ciales", "Cidra", "Coamo", "Comerío", "Corozal", "Culebra", "Dorado",
            "Fajardo", "Florida", "Guánica", "Guayama", "Guayanilla", "Guaynabo", "Gurabo", "Hatillo", "Hormigueros", "Humacao",
            "Isabela", "Jayuya", "Juana Díaz", "Juncos", "Lajas", "Lares", "Las Marías", "Las Piedras", "Loíza", "Luquillo", "Manatí",
            "Maricao", "Maunabo", "Mayagüez", "Moca", "Morovis", "Naguabo", "Naranjito", "Orocovis", "Patillas", "Peñuelas", "Ponce",
            "Quebradillas", "Rincón", "Río Grande", "Sabana Grande", "Salinas", "San Germán", "San Juan", "San Lorenzo", "San Sebastián",
            "Santa Isabel", "Toa Alta", "Toa Baja", "Trujillo Alto", "Utuado", "Vega Alta", "Vega Baja", "Vieques", "Villalba", "Yabucoa",
            "Yauco"};

            for (int i = 0; i < town.Length; i++)
            {
                towns.Add(new SelectListItem()
                {
                    Text = town[i],
                    Value = town[i]
                });
            }

            return towns;

        }

        public ActionResult AccesoDenegado()
        {
            return View("AccesoDenegado");
        }
    }

    public static class StringExtensions
    {
        /// <summary>
        /// Remove accent from strings 
        /// </summary>
        /// <example>
        /// input: "Příliš žluťoučký kůň úpěl ďábelské ódy."
        /// result: "Prilis zlutoucky kun upel dabelske ody."
        /// </example>
        /// <param name="s"></param>
        /// <remarks>founded at http://stackoverflow.com/questions/249087/
        /// how-do-i-remove-diacritics-accents-from-a-string-in-net</remarks>
        /// <returns>string without accents</returns>

        public static string RemoveDiacritics(this string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
    }
}
