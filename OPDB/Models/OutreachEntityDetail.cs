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
    
    public partial class OutreachEntityDetail
    {
        public int OutreachEntityDetailID { get; set; }
        public int UserID { get; set; }
        public int OutreachEntityTypeID { get; set; }
        public string OutreachEntityName { get; set; }
        public string Mission { get; set; }
        public string Vision { get; set; }
        public string Objectives { get; set; }
        public string Location { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<System.DateTime> DeletionDate { get; set; }
        public string RemovalReason { get; set; }
    
        public virtual User User { get; set; }
        public virtual OutreachEntityType OutreachEntityType { get; set; }
    }
}
