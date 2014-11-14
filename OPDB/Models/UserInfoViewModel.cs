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
        public User User { get; set; }
        public Contact Contact { get; set; }
        public ActivityResource ActivityResource { get; set; }

        public bool Interested { get; set; }

        public UserDetail UserDetail { get; set; }
        public UserDetail CreateUser { get; set; }
        public UserDetail UpdateUser { get; set; }

        public OutreachEntityDetail CreateEntity { get; set; }
        public OutreachEntityDetail UpdateEntity { get; set; }

        public OutreachEntityDetail OutreachEntity { get; set; }

        public ActivityDynamic ActivityDynamic { get; set; }

        

    }
}