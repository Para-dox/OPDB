using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPDB.Models
{
    public class SearchViewModel
    {
        public string searchText { get; set; }
        public IEnumerable<UserDetail> users {get; set;}
        public IEnumerable<OutreachEntityDetail> outreachEntities { get; set; }
        public IEnumerable<Activity> activities { get; set; }
        public IEnumerable<Unit> units { get; set; }
        public IEnumerable<School> schools { get; set; }

        public UserDetail user { get; set; }
        public OutreachEntityDetail outreachEntity { get; set; }
        public Activity activity { get; set; }
        public School school { get; set; }
        public Unit unit { get; set; }

        public List<SelectListItem> types { get; set; }

        public bool  buscarUsuarios {get; set;}
        public bool  buscarAlcance {get; set;}
        public bool  buscarEscuelas {get; set;}
        public bool  buscarUnidades {get; set;}
        public bool  buscarActividades { get; set;}
    }
}