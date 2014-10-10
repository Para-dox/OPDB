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
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
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

            //return View("Search", searchViewModel);
            return View(searchViewModel);


        }

        public ActionResult _Buscar()
        {
            SearchViewModel search = new SearchViewModel();
            return PartialView("_Buscar", search);
        }
    }
}
