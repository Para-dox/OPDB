using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPDB.Models
{
    public class SearchViewModel
    {
        public string SearchText { get; set; }
        public IEnumerable<UserDetail> Users {get; set;}
        public IEnumerable<OutreachEntityDetail> OutreachEntities { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
        public IEnumerable<Unit> Units { get; set; }
        public IEnumerable<School> Schools { get; set; }

        public UserDetail User { get; set; }
        public OutreachEntityDetail OutreachEntity { get; set; }
        public Activity Activity { get; set; }
        public School School { get; set; }
        public Unit Unit { get; set; }

        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> Majors { get; set; }
        public List<SelectListItem> Dynamics { get; set; }
        public List<SelectListItem> Towns { get; set; }

        public bool  BuscarUsuarios {get; set;}
        public bool  BuscarAlcance {get; set;}
        public bool  BuscarEscuelas {get; set;}
        public bool  BuscarUnidades {get; set;}
        public bool  BuscarActividades { get; set;}
    }
}