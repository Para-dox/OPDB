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
    
    public partial class OutreachEntityType
    {
        public OutreachEntityType()
        {
            this.OutreachEntityDetails = new HashSet<OutreachEntityDetail>();
        }
    
        public int OutreachEntityTypeID { get; set; }
        public string OureachEntityType { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<System.DateTime> DeletionDate { get; set; }
    
        public virtual ICollection<OutreachEntityDetail> OutreachEntityDetails { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
