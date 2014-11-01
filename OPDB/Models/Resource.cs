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
    
    public partial class Resource
    {
        public Resource()
        {
            this.ActivityResources = new HashSet<ActivityResource>();
        }
    
        public int ResourceID { get; set; }

        [RegularExpression(@"^[a-zA-Z\u00c0-\u017e''-'\s]{1,100}$", ErrorMessageResourceName = "Resource_ResourceName_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "Resource_ResourceName_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string Resource1 { get; set; }

        public int UnitID { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<System.DateTime> DeletionDate { get; set; }
    
        public virtual ICollection<ActivityResource> ActivityResources { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
