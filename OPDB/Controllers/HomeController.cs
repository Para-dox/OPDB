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

        public ActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel
            {

                activities = (from activity in db.Activities.Include(a => a.ActivityType) orderby activity.UpdateDate descending select activity).Take(6).ToList()

            };

            return View(homeViewModel);
        }

        public ActionResult Calendario()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

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
                                    where user.FirstName.Contains(searchText)
                                        || user.LastName.Contains(searchText)
                                    select user;

            searchViewModel.outreachEntities = from outreachEntity in db.OutreachEntityDetails
                                               where outreachEntity.OutreachEntityName.Contains(searchText)
                                                   || outreachEntity.Mission.Contains(searchText) || outreachEntity.Vision.Contains(searchText)
                                                   || outreachEntity.Objectives.Contains(searchText)
                                               select outreachEntity;

            searchViewModel.activities = from activity in db.Activities
                                         where activity.Title.Contains(searchText) || activity.Purpose.Contains(searchText)
                                         select activity;

            searchViewModel.schools = from school in db.Schools where school.SchoolName.Contains(searchText) select school;

            searchViewModel.units = from unit in db.Units where unit.UnitName.Contains(searchText) select unit;

            return View(searchViewModel);


        }

        public ActionResult _Buscar()
        {
            SearchViewModel search = new SearchViewModel();
            return PartialView("_Buscar", search);
        }


        public ActionResult _PartialViewLoad(string view)
        {
            //SearchViewModel searchViewModel = new SearchViewModel();

            //if(view == "")
            
            return PartialView(view);

        }

        public ActionResult BusquedaAvanzada()
        {

            return View();

        }

        [HttpPost]
        public ActionResult BuscarUsuarios(SearchViewModel searchViewModel)
        {
            searchViewModel.buscarUsuarios = true;

            searchViewModel.users = from user in db.UserDetails
                                    where user.FirstName.Contains(searchViewModel.user.FirstName)
                                        || user.LastName.Contains(searchViewModel.user.LastName) || user.Role.Contains(searchViewModel.user.Role)
                                        || user.Major.Contains(searchViewModel.user.Major)
                                    select user;

            
            return View("Buscar", searchViewModel);


        }

    }
}
