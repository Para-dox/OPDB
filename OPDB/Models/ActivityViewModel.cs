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
        public ActivityDynamic ActivityDynamic { get; set; }

        public List<UserInfoViewModel> Feedbacks { get; set; }

        public List<UserInfoViewModel> ActivityContacts { get; set; }

        public List<UserInfoViewModel> ActivityResources { get; set; }

        public List<UserInfoViewModel> Semesters { get; set; }

        public IEnumerable<ActivityNote> Notes { get; set; }
        
        public List<SelectListItem> NoteTypes { get; set; }
        public List<SelectListItem> ActivityTypes { get; set; }
        public List<SelectListItem> SchoolList { get; set; }
        public List<SelectListItem> OutreachEntities { get; set; }

        public List<SelectListItem> ActivityMajors { get; set; }
        public List<SelectListItem> ActivityDynamics { get; set; }

        public List<SelectListItem> TargetPopulations { get; set; }

        public string ActivityDate { get; set; }

        public DateTime PreviousDate { get; set; }
        public int PreviousSchool { get; set; }
        public string PreviousTime { get; set; }

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

        public string Source { get; set; }

        public int Duration { get; set; }

        public string Measurement { get; set; }

        public DateTime EndDate { get; set; }

        public List<SelectListItem> Measurements
        {
            get
            {
                var measurements = new List<SelectListItem>();

                measurements.Add(new SelectListItem()
                {
                    Text = "",
                    Value = ""
                });

                measurements.Add(new SelectListItem()
                {
                    Text = "Minuto(s)",
                    Value = "Minutes"
                });

                measurements.Add(new SelectListItem()
                {
                    Text = "Hora(s)",
                    Value = "Hours"
                });

                measurements.Add(new SelectListItem()
                {
                    Text = "Día(s)",
                    Value = "Days"
                });

                return measurements;
            }
        }

        
    }
}