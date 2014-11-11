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


    public partial class School
    {
        public School()
        {
            this.Activities = new HashSet<Activity>();
            this.SchoolNotes = new HashSet<SchoolNote>();
        }

        public int SchoolID { get; set; }

        [RegularExpression(@"^[0-9]{1,50}$", ErrorMessageResourceName = "School_SchoolSequenceNumber_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "School_SchoolSequenceNumber_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string SchoolSequenceNumber { get; set; }

        [RegularExpression(@"^[a-zA-Z\u00c0-\u017e',\s]{1,100}$", ErrorMessageResourceName = "School_SchoolName_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "School_SchoolName_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string SchoolName { get; set; }

        [RegularExpression(@"^[a-zA-Z\u00c0-\u017e',\s]{1,100}$", ErrorMessageResourceName = "School_Address_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "School_Address_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string Address { get; set; }
        public string Town { get; set; }

        [RegularExpression(@"^([2-9]\d{2}|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$", ErrorMessageResourceName = "School_PhoneNumber_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "School_PhoneNumber_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string PhoneNumber { get; set; }

        public int CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<System.DateTime> DeletionDate { get; set; }
        public int SchoolRegionID { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<SchoolNote> SchoolNotes { get; set; }
        public virtual SchoolRegion SchoolRegion { get; set; }
    }
}
