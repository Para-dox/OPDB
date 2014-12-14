using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPDB.Models
{
    public class ReportViewModel
    {
        public List<SelectListItem> Semesters { get; set; }
        public List<SelectListItem> ActivityTypes { get; set; }
        public List<SelectListItem> ActivityMajors { get; set; }
        public List<SelectListItem> ActivityDynamics { get; set; }
        public List<SelectListItem> TargetPopulations { get; set; }
        public List<SelectListItem> SchoolRegions { get; set; }

        public Activity Activity { get; set; }

        public School School { get; set; }

        public string TargetMonths { get; set; }

        public IEnumerable<Activity> Results { get; set; }

       

    }
}