using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        
    }
}