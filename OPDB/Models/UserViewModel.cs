﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPDB.Models
{
    public class UserViewModel
    {
        public User User { get; set; }
        public UserType UserType { get; set; }
        public UserDetail UserDetail { get; set; }
        public OutreachEntityDetail OutreachEntity { get; set; }
        public List<SelectListItem> UserTypes { get; set; }
        public List<SelectListItem> OutreachTypes { get; set; }

        public string OldPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public UserNote Note { get; set; }

        public List<UserInfoViewModel> Notes { get; set; }

        public List<UserInfoViewModel> Information { get; set; }

        

        public List<SelectListItem> Schools { get; set; }
        public List<SelectListItem> OutreachEntities { get; set; }
        public List<SelectListItem> Units { get; set; }

        public string SchoolID { get; set; }
        public string OutreachEntityDetailID { get; set; }
        public string UnitID { get; set; }

        public List<Activity> Activities { get; set; }

        public string Source { get; set; }

        public string BirthDate { get; set; }

        public string Sender { get; set; }

        public string Reader { get; set; }
        
    }
}