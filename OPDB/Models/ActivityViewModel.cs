using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace OPDB.Models
{
    public class ActivityViewModel
    {
        public Activity Activity { get; set; }
        public Feedback Feedback { get; set; }
        public ActivityNote Note { get; set; }
        public Medium Media { get; set; }
        public User User { get; set; }
        public OutreachEntityDetail OutreachEntity { get; set; }

        public List<UserInfoViewModel> Feedbacks { get; set; }

        public List<UserInfoViewModel> ActivityContacts { get; set; }

        public List<UserInfoViewModel> ActivityResources { get; set; }

        public IEnumerable<ActivityNote> Notes { get; set; }
        
        public List<SelectListItem> NoteTypes { get; set; }
        public List<SelectListItem> ActivityTypes { get; set; }
        public List<SelectListItem> SchoolList { get; set; }
        public List<SelectListItem> OutreachEntities { get; set; }

        public string ActivityDate { get; set; }

        public List<UserInfoViewModel> Information { get; set; }

        public List<SelectListItem> MediaTypes { get; set; }

        public bool Interested { get; set; }

        public List<Medium> Videos { get; set; }

        public List<Medium> Photos { get; set; }

        public List<SelectListItem> Contacts { get; set; }

        public List<int> ContactIDs { get; set; }

        public List<SelectListItem> Resources { get; set; }

        public List<int> ResourceIDs { get; set; }

        public bool ForceCreate { get; set; }

        public string Action { get; set; }
    }
}