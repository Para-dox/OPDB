using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPDB.Models
{
    public class ResourceViewModel
    {
        public Resource resource { get; set; }
        public Unit unit { get; set; }

        public List<SelectListItem> units { get; set; }
    }
}