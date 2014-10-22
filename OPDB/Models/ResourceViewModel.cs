using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPDB.Models
{
    public class ResourceViewModel
    {
        public Resource Resource { get; set; }

        public Unit Unit { get; set; }

        public List<UserInfoViewModel> Information { get; set; }

        public List<SelectListItem> Units { get; set; }
    }
}