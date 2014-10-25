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
        public UserType userType { get; set; }
        public UserDetail userDetail { get; set; }
        public OutreachEntityDetail outreachEntity { get; set; }
        public List<SelectListItem> userTypes { get; set; }
        public List<SelectListItem> outreachTypes { get; set; }

        public UserNote note { get; set; }
        public List<SelectListItem> NoteTypes { get; set; }
        public IEnumerable<UserNote> Notes { get; set; }

        public List<UserInfoViewModel> Information { get; set; }
        
    }
}