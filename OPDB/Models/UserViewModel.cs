using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPDB.Models
{
    public class UserViewModel
    {
        public User user { get; set; }
        public UserDetail userDetail { get; set; }
        public List<SelectListItem> userTypes { get; set; }
    }
}