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
                var activityDynamic = new ActivityDynamic
                {
                    ActivityDynamic1 = ""
                };

                if (activity.ActivityDynamicID != null)
                    activityDynamic = db.ActivityDynamics.Find(activity.ActivityDynamicID);

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
                    Interested = interested,
                    ActivityDynamic = activityDynamic
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

            searchViewModel.Users = (from userDetail in db.UserDetails join user in db.Users on userDetail.UserID equals user.UserID 
                                    join userType in db.UserTypes on user.UserTypeID equals userType.UserTypeID
                                     where (userDetail.FirstName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                                        || userDetail.LastName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) ||
                                        userType.UserType1.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)) && 
                                        userDetail.DeletionDate == null
                                    select userDetail).ToList();

            searchViewModel.OutreachEntities = (from outreachEntity in db.OutreachEntityDetails join outreachType in db.OutreachEntityTypes on outreachEntity.OutreachEntityTypeID equals outreachType.OutreachEntityTypeID
                                                where (outreachEntity.OutreachEntityName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                                                   || outreachEntity.Mission.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) || outreachEntity.Vision.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                                                   || outreachEntity.Objectives.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) || outreachType.OutreachEntityType1.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)) && outreachEntity.DeletionDate == null
                                               select outreachEntity).ToList();

            searchViewModel.Activities = (from activity in db.Activities
                                          where (activity.Title.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) || activity.Purpose.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) ||
                                         activity.ActivityMajor.ActivityMajor1.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) || (activity.ActivityDynamic.ActivityDynamic1 ?? "").Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                                         && activity.DeletionDate == null
                                         select activity).ToList();

            searchViewModel.Schools = (from school in db.Schools
                                       where (school.SchoolName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                                          || school.Address.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                                          || school.SchoolRegion.SchoolRegion1.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)) 
                                          && school.DeletionDate == null select school).ToList();

            searchViewModel.Units = (from unit in db.Units where unit.UnitName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) && unit.DeletionDate == null select unit).ToList();

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
                UsuariosController controller = new UsuariosController();
                searchViewModel.Types = controller.getUserTypes("Admin");
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
            
            
            var result = (from user in db.UserDetails
                         where user.DeletionDate == null
                         select user).ToList();

            searchViewModel.Users = result;

            if (searchViewModel.User.User.UserTypeID != 0)
            {

                result = (from user in result
                         where user.User.UserTypeID == searchViewModel.User.User.UserTypeID
                         select user).ToList();

                searchViewModel.Users = result;
            }

            if (searchViewModel.User.FirstName != null)
            {

                result = (from user in result
                          where user.FirstName.Contains(searchViewModel.User.FirstName, StringComparison.InvariantCultureIgnoreCase) && user.DeletionDate == null
                         select user).ToList();

                searchViewModel.Users = result;
            }

            if (searchViewModel.User.LastName != null)
            {
                result = (from user in result
                          where user.LastName.Contains(searchViewModel.User.LastName, StringComparison.InvariantCultureIgnoreCase) && user.DeletionDate == null
                         select user).ToList();

                searchViewModel.Users = result;
            }

            if (searchViewModel.User.Role != null)
            {
                result = (from user in result
                          where user.Role.Contains(searchViewModel.User.Role, StringComparison.InvariantCultureIgnoreCase) && user.DeletionDate == null
                         select user).ToList();

                searchViewModel.Users = result;
            }

            if (searchViewModel.User.Major != null)
            {
                result = (from user in result
                          where user.Major.Contains(searchViewModel.User.Major, StringComparison.InvariantCultureIgnoreCase) && user.DeletionDate == null
                         select user).ToList();

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

            var result = (from activity in db.Activities
                         where activity.DeletionDate == null 
                         select activity).ToList();

            searchViewModel.Activities = result;

            if (searchViewModel.Activity.ActivityTypeID != 0)
            {
                result = (from activity in result
                         where activity.ActivityTypeID == searchViewModel.Activity.ActivityTypeID
                         && activity.DeletionDate == null
                         select activity).ToList();

                searchViewModel.Activities = result;
            }

            if (searchViewModel.Activity.ActivityMajorID != 0)
            {
                result = (from activity in result
                         where activity.ActivityMajorID == searchViewModel.Activity.ActivityMajorID
                         && activity.DeletionDate == null
                         select activity).ToList();

                searchViewModel.Activities = result;
            }

            if (searchViewModel.Activity.ActivityDynamicID != null && searchViewModel.Activity.ActivityDynamicID != 0)
            {
                result = (from activity in result
                         where activity.ActivityDynamicID == searchViewModel.Activity.ActivityDynamicID
                         && activity.DeletionDate == null
                         select activity).ToList();

                searchViewModel.Activities = result;
            }

            if (searchViewModel.Activity.Title != null)
            {
                result = (from activity in result
                          where activity.Title.Contains(searchViewModel.Activity.Title, StringComparison.InvariantCultureIgnoreCase)
                         && activity.DeletionDate == null
                         select activity).ToList();

                searchViewModel.Activities = result;
            }

            if(searchViewModel.Activity.Purpose != null){

                result = (from activity in result
                          where activity.Purpose.Contains(searchViewModel.Activity.Purpose, StringComparison.InvariantCultureIgnoreCase)
                         && activity.DeletionDate == null
                         select activity).ToList();

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

            var result = (from outreachEntity in db.OutreachEntityDetails
                         where outreachEntity.DeletionDate == null
                         select outreachEntity).ToList();

            searchViewModel.OutreachEntities = result;

            if (searchViewModel.OutreachEntity.OutreachEntityTypeID != 0)
            {

                result = (from outreachEntity in result
                         where outreachEntity.OutreachEntityTypeID == searchViewModel.OutreachEntity.OutreachEntityTypeID
                         && outreachEntity.DeletionDate == null
                         select outreachEntity).ToList();

                searchViewModel.OutreachEntities = result;
            }

            if (searchViewModel.OutreachEntity.OutreachEntityName != null)
            {

                result = (from outreachEntity in result
                          where outreachEntity.OutreachEntityName.Contains(searchViewModel.OutreachEntity.OutreachEntityName, StringComparison.InvariantCultureIgnoreCase)
                         && outreachEntity.DeletionDate == null
                         select outreachEntity).ToList();

                searchViewModel.OutreachEntities = result;
            }

            if (searchViewModel.OutreachEntity.Mission != null)
            {

                result = (from outreachEntity in result
                          where outreachEntity.Mission.Contains(searchViewModel.OutreachEntity.Mission, StringComparison.InvariantCultureIgnoreCase)
                         && outreachEntity.DeletionDate == null
                          select outreachEntity).ToList();

                searchViewModel.OutreachEntities = result;

            }

            if (searchViewModel.OutreachEntity.Vision != null)
            {

                result = (from outreachEntity in result
                          where outreachEntity.Vision.Contains(searchViewModel.OutreachEntity.Vision, StringComparison.InvariantCultureIgnoreCase)
                         && outreachEntity.DeletionDate == null
                          select outreachEntity).ToList();

                searchViewModel.OutreachEntities = result;
            }

            if (searchViewModel.OutreachEntity.Objectives != null)
            {

                result = (from outreachEntity in result
                          where outreachEntity.Objectives.Contains(searchViewModel.OutreachEntity.Objectives, StringComparison.InvariantCultureIgnoreCase)
                         && outreachEntity.DeletionDate == null
                         select outreachEntity).ToList();

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

            var result = (from school in db.Schools
                         where school.DeletionDate == null
                         select school).ToList();

            searchViewModel.Schools = result;

            if (searchViewModel.School.SchoolName != null)
            {
                result = (from school in result
                          where school.SchoolName.Contains(searchViewModel.School.SchoolName, StringComparison.InvariantCultureIgnoreCase)
                         && school.DeletionDate == null
                         select school).ToList();

                searchViewModel.Schools = result;
            }

            if (searchViewModel.School.Address != null)
            {
                result = (from school in result
                          where school.Address.Contains(searchViewModel.School.Address, StringComparison.InvariantCultureIgnoreCase)
                         && school.DeletionDate == null
                         select school).ToList();

                searchViewModel.Schools = result;
            }

            if (searchViewModel.School.Town != null)
            {
                result = (from school in result
                          where school.Town.Contains(searchViewModel.School.Town, StringComparison.InvariantCultureIgnoreCase) 
                         && school.DeletionDate == null
                         select school).ToList();

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

            searchViewModel.Units = (from unit in db.Units where unit.UnitName.Contains(searchViewModel.Unit.UnitName ?? "", StringComparison.InvariantCultureIgnoreCase) select unit).ToList();
            
            return View("Buscar", searchViewModel);
        }

        public ActionResult Administracion()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    return View();
                }
            }

            return RedirectToAction("AccesoDenegado");
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
                Text = null,
                Value = null
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
                Text = null,
                Value = null
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
                Text = null,
                Value = null
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
                Text = null,
                Value = null
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

        public List<SelectListItem> getTargetPopulations()
        {
            List<SelectListItem> types = new List<SelectListItem>();

            types.Add(new SelectListItem()
            {
                Text = null,
                Value = null

            });

            foreach (var populations in db.TargetPopulations)
            {
                types.Add(new SelectListItem()
                {
                    Text = populations.TargetPopulation1,
                    Value = populations.TargetPopulationID + ""

                });

            }

            return types;
        }

        public List<SelectListItem> getTowns()
        {
            List<SelectListItem> towns = new List<SelectListItem>();

            String[] town = new String[] { null, "Adjuntas", "Aguada", "Aguadilla", "Aguas Buenas", "Aibonito", "Añasco",
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

        public ActionResult FormularioReportes(string requested)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(requested))
                {
                    ReportViewModel reportViewModel = new ReportViewModel
                    {
                        Semesters = getSemesters(),
                        ActivityTypes = getActivityTypes(),
                        ActivityMajors = getActivityMajors(),
                        ActivityDynamics = getActivityDynamics(),
                        TargetPopulations = getTargetPopulations()
                    };

                    reportViewModel.ActivityTypes.Add(new SelectListItem
                    {
                        Text = "Todos",
                        Value = "All"
                    });

                    reportViewModel.ActivityMajors.Add(new SelectListItem
                    {
                        Text = "Todas",
                        Value = "All"
                    });

                    reportViewModel.ActivityDynamics.Add(new SelectListItem
                    {
                        Text = "Todas",
                        Value = "All"
                    });

                    return PartialView("FormularioReportes", reportViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado");
        }

        public ActionResult GenerarReporte(ReportViewModel reportViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    var result = (IEnumerable<Activity>) null;

                    if (reportViewModel.TargetMonths != null)
                    {
                        if (reportViewModel.TargetMonths.Contains("All"))
                        {
                            var academicYear = Int32.Parse(reportViewModel.TargetMonths.Split('-')[1]);

                            result = from activity in db.Activities
                                     where activity.DeletionDate == null
                                     && ((activity.ActivityDate.Value.Year == academicYear && (activity.ActivityDate.Value.Month >= 6 && activity.ActivityDate.Value.Month <= 12))
                                     || (activity.ActivityDate.Value.Year == (academicYear + 1) && (activity.ActivityDate.Value.Month >= 1 && activity.ActivityDate.Value.Month <= 5)))
                                     orderby activity.ActivityDate ascending
                                     select activity;

                            reportViewModel.Results = result;
                        }
                        else
                        {
                            var minMonth = Int32.Parse(reportViewModel.TargetMonths.Split('-')[0]);
                            var maxMonth = Int32.Parse(reportViewModel.TargetMonths.Split('-')[1]);
                            var academicYear = Int32.Parse(reportViewModel.TargetMonths.Split('-')[2]);

                            result = from activity in db.Activities
                                     where activity.DeletionDate == null
                                     && activity.ActivityDate.Value.Year == academicYear && (activity.ActivityDate.Value.Month >= minMonth && activity.ActivityDate.Value.Month <= maxMonth)
                                     orderby activity.ActivityDate ascending
                                     select activity;

                            reportViewModel.Results = result;
                        }

                        if (reportViewModel.Activity.ActivityTypeID != 0)
                        {


                            result = from activity in result
                                     where activity.ActivityTypeID == reportViewModel.Activity.ActivityTypeID
                                     && activity.DeletionDate == null
                                     select activity;

                            reportViewModel.Results = result;
                        }

                        if (reportViewModel.Activity.ActivityMajorID != 0)
                        {
                            result = from activity in result
                                     where activity.ActivityMajorID == reportViewModel.Activity.ActivityMajorID
                                     && activity.DeletionDate == null
                                     select activity;

                            reportViewModel.Results = result;
                        }

                        if (reportViewModel.Activity.ActivityDynamicID != null && reportViewModel.Activity.ActivityDynamicID != 0)
                        {
                            result = from activity in result
                                     where activity.ActivityDynamicID == reportViewModel.Activity.ActivityDynamicID
                                     && activity.DeletionDate == null
                                     select activity;

                            reportViewModel.Results = result;
                        }

                        if (reportViewModel.Activity.TargetPopulationID != 0)
                        {
                            result = from activity in result
                                     where activity.TargetPopulationID == reportViewModel.Activity.TargetPopulationID
                                     && activity.DeletionDate == null
                                     select activity;

                            reportViewModel.Results = result;
                        }
                    }

                    return generateReportFile(result);
                }
            }

            return RedirectToAction("AccesoDenegado");
        }

        public FileStreamResult generateReportFile(IEnumerable<Activity> results)
        {
            // TODO: acentos are not working in the CSV in excel
            string dataText = "Título,Tipo,Dinámica,Concentración,Fecha,Hora";
            string dataRow = "";

            if (results != null)
            {
                foreach (Activity activity in results.ToList())
                {
                    string activityTitle = null;
                    string activityType = null;
                    string activityDynamic = null;
                    string activityMajor = null;
                    string activityDate = null;
                    string activityTime = null;

                    try
                    {
                        activityTitle = activity.Title;
                    }
                    catch (NullReferenceException){}

                    try
                    {
                        activityType = activity.ActivityType.ActivityType1;
                    }
                    catch (NullReferenceException) { }

                    try
                    {
                        activityDynamic = activity.ActivityDynamic.ActivityDynamic1;
                    }
                    catch (NullReferenceException) { }

                    try
                    {
                        activityMajor = activity.ActivityMajor.ActivityMajor1;
                    }
                    catch (NullReferenceException) { }

                    try
                    {
                        activityDate = activity.ActivityDate.ToString().Split(' ')[0];
                    }
                    catch (NullReferenceException) { }

                    try
                    {
                        activityTime = activity.ActivityTime;
                    }
                    catch (NullReferenceException) { }
                    
                    dataRow = string.Format("{0},{1},{2},{3},{4},{5}", activityTitle, activityType, activityDynamic, activityMajor, activityDate, activityTime);
                    dataText = dataText + "\n" + dataRow;
                }
            }

            var byteArray = Encoding.Unicode.GetBytes(dataText);
            var stream = new System.IO.MemoryStream(byteArray);

            return File(stream, "text/csv", "reporte.csv");
        }

        public List<SelectListItem> getSemesters()
        {
            List<SelectListItem> semesters = new List<SelectListItem>();

            semesters.Add(new SelectListItem
            {
                Text = null,
                Value = null
            });

            var minYears = (from activity in db.Activities where activity.DeletionDate == null && activity.ActivityDate != null select activity.ActivityDate.Value.Year).ToList();
            var minYear = DateTime.Now.Year; 

            if(minYears.Count() > 0)
                    minYear = minYears.Min();

            var maxYears = (from activity in db.Activities where activity.DeletionDate == null && activity.ActivityDate != null select activity.ActivityDate.Value.Year).ToList();
            var maxYear = DateTime.Now.Year;

            if(maxYears.Count() > 0)
                    maxYear = maxYears.Min();

            for (int i = minYear; i <= maxYear; i++)
            {
                semesters.Add(new SelectListItem
                {
                    Text = "Verano " + i,
                    Value = "6-7-"+i,
                });

                semesters.Add(new SelectListItem
                {
                    Text = "Primer Semestre " + i,
                    Value = "8-12-"+i
                });

                semesters.Add(new SelectListItem
                {
                    Text = "Segundo Semestre " + i,
                    Value = "1-5-"+(i+1)
                });

                semesters.Add(new SelectListItem
                {
                    Text = "Año Académico " + i,
                    Value = "All-"+i
                });
            }

            return semesters;
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

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
