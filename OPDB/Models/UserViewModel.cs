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

        public List<SelectListItem> AffiliateTypes {
          get{
                var types = new List<SelectListItem>();

                types.Add(new SelectListItem()
                {
                    Text = "",
                    Value = ""
                });

                types.Add(new SelectListItem(){
                    Text = "Escuela",
                    Value = "School"
                });

                types.Add(new SelectListItem(){
                    Text = "Entidad de Alcance",
                    Value = "Outreach Entity"
                });

                types.Add(new SelectListItem(){
                    Text = "Unidad",
                    Value = "Unit"
                });

                return types; 
            } 
        }

        public List<SelectListItem> Schools { get; set; }
        public List<SelectListItem> OutreachEntities { get; set; }
        public List<SelectListItem> Units { get; set; }

        public string SchoolID { get; set; }
        public string OutreachEntityDetailID { get; set; }
        public string UnitID { get; set; }
        
    }
}