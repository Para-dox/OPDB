using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPDB.Models
{
    public class UserInfoViewModel
    {
        public Resource Resource { get; set; }
        public Feedback Feedback { get; set; }
        public Activity Activity { get; set; }
        public School School { get; set; }
        public Unit Unit { get; set; }

        public UserDetail User { get; set; }
        public UserDetail CreateUser { get; set; }
        public UserDetail UpdateUser { get; set; }

        public OutreachEntityDetail CreateEntity { get; set; }
        public OutreachEntityDetail UpdateEntity { get; set; }

        public OutreachEntityDetail OutreachEntity { get; set; }

        

    }
}