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
    
    public partial class UserNote
    {
        public int UserNoteID { get; set; }
        public int SubjectID { get; set; }
        public int UserID { get; set; }
        public string Note { get; set; }
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
