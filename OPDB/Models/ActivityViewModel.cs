using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace OPDB.Models
{
    public class ActivityViewModel
    {
        public Activity activity { get; set; }
        public ActivityNote note { get; set; }
        public IEnumerable<ActivityNote> Notes { get; set; }
        public List<SelectListItem> NoteTypes { get; set; }
    }
}