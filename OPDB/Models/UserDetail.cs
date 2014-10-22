//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OPDB.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserDetail
    {
        public int UserDetailID { get; set; }
        public int UserID { get; set; }
        [Required(ErrorMessageResourceName = "UserDetail_FirstName_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string FirstName { get; set; }
        [Required(ErrorMessageResourceName = "UserDetail_LastName_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        [Required(ErrorMessageResourceName = "UserDetail_Gender_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string Gender { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Role { get; set; }
        public string Major { get; set; }
        public string Grade { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<System.DateTime> DeletionDate { get; set; }
        public Nullable<int> AffiliateID { get; set; }
        public Nullable<int> AffiliateTypeID { get; set; }
    
        public virtual AffiliateType AffiliateType { get; set; }
        public virtual User User { get; set; }
    }
}
