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

    public partial class Unit
    {
        public Unit()
        {
            this.Resources = new HashSet<Resource>();
        }

        public int UnitID { get; set; }

        [StringLength(100, ErrorMessageResourceName = "Unit_UnitName_LengthExceeded", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [RegularExpression(@"^[a-zA-Z\u00c0-\u017e0-9\-\s]+$", ErrorMessageResourceName = "Unit_UnitName_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "Unit_UnitName_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string UnitName { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Unit_Building_LengthExceeded", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [RegularExpression(@"^[a-zA-Z\u00c0-\u017e\s]+$", ErrorMessageResourceName = "Unit_Building_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "Unit_Building_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string Building { get; set; }

        [StringLength(10, ErrorMessageResourceName = "Unit_RoomNumber_LengthExceeded", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [RegularExpression(@"^[a-zA-Z0-9\-\s]+$", ErrorMessageResourceName = "Unit_RoomNumber_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "Unit_RoomNumber_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string RoomNumber { get; set; }

        [RegularExpression(@"^([2-9]\d{2}|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$", ErrorMessageResourceName = "Unit_PhoneNumber_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        [Required(ErrorMessageResourceName = "Unit_PhoneNumber_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string PhoneNumber { get; set; }

        [RegularExpression(@"^[0-9\s]{4}$", ErrorMessageResourceName = "Unit_Extension_Invalid", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string Extension { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<System.DateTime> DeletionDate { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
