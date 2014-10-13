using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPDB.Models
{
    public class SchoolViewModel
    {

        public School school { get; set; }
        public SchoolNote note { get; set; }
        public IEnumerable<SchoolNote> Notes { get; set; }
        public List<SelectListItem> NoteTypes { get; set; }
        
    }
}