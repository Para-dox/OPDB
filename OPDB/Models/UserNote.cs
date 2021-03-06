//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OPDB.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public partial class UserNote
    {
        public int UserNoteID { get; set; }
        public int SubjectID { get; set; }
        public int UserID { get; set; }

        [StringLength(100, ErrorMessageResourceName = "UserNote_Subject_LengthExceeded", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [RegularExpression(@"([a-zA-Z\u00c0-\u017e0-9�?.,;:!�()$""'/\s]+[-]?[a-zA-Z\u00c0-\u017e0-9?.,;:!)@$""'/\s]+)+$", ErrorMessageResourceName = "UserNote_Subject_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "UserNote_Subject_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string Subject { get; set; }
        
        [RegularExpression(@"([a-zA-Z\u00c0-\u017e0-9�?.,;:!�()$""'/\s]+[-]?[a-zA-Z\u00c0-\u017e0-9?.,;:!)@$""'/\s]+)+$", ErrorMessageResourceName = "UserNote_Note_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "UserNote_Note_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string Note { get; set; }
        public bool IsRead { get; set; }       
        public int CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<System.DateTime> DeletionDate { get; set; }

        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        public virtual User User3 { get; set; }
    }
}
