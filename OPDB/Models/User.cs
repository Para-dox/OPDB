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
    
    public partial class User
    {
        public User()
        {
            this.Activities = new HashSet<Activity>();
            this.Activities1 = new HashSet<Activity>();
            this.Activities2 = new HashSet<Activity>();
            this.ActivityNotes = new HashSet<ActivityNote>();
            this.ActivityNotes1 = new HashSet<ActivityNote>();
            this.ActivityNotes2 = new HashSet<ActivityNote>();
            this.ActivityResources = new HashSet<ActivityResource>();
            this.ActivityResources1 = new HashSet<ActivityResource>();
            this.ActivityTypes = new HashSet<ActivityType>();
            this.ActivityTypes1 = new HashSet<ActivityType>();
            this.AffiliateTypes = new HashSet<AffiliateType>();
            this.AffiliateTypes1 = new HashSet<AffiliateType>();
            this.Contacts = new HashSet<Contact>();
            this.Contacts1 = new HashSet<Contact>();
            this.Contacts2 = new HashSet<Contact>();
            this.Documents = new HashSet<Document>();
            this.Documents1 = new HashSet<Document>();
            this.Documents2 = new HashSet<Document>();
            this.Feedbacks = new HashSet<Feedback>();
            this.Feedbacks1 = new HashSet<Feedback>();
            this.Feedbacks2 = new HashSet<Feedback>();
            this.Goals = new HashSet<Goal>();
            this.Goals1 = new HashSet<Goal>();
            this.Goals2 = new HashSet<Goal>();
            this.Interests = new HashSet<Interest>();
            this.Interests1 = new HashSet<Interest>();
            this.Interests2 = new HashSet<Interest>();
            this.Media = new HashSet<Medium>();
            this.Media1 = new HashSet<Medium>();
            this.NoteTypes = new HashSet<NoteType>();
            this.NoteTypes1 = new HashSet<NoteType>();
            this.OutreachEntityDetails = new HashSet<OutreachEntityDetail>();
            this.Resources = new HashSet<Resource>();
            this.Resources1 = new HashSet<Resource>();
            this.Schools = new HashSet<School>();
            this.Schools1 = new HashSet<School>();
            this.SchoolNotes = new HashSet<SchoolNote>();
            this.Units = new HashSet<Unit>();
            this.Units1 = new HashSet<Unit>();
            this.UserDetails = new HashSet<UserDetail>();
            this.UserNotes = new HashSet<UserNote>();
            this.UserNotes1 = new HashSet<UserNote>();
            this.UserNotes2 = new HashSet<UserNote>();
            this.UserNotes3 = new HashSet<UserNote>();
            this.OutreachEntityTypes = new HashSet<OutreachEntityType>();
            this.OutreachEntityTypes1 = new HashSet<OutreachEntityType>();
        }
    
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        [Required(ErrorMessageResourceName = "User_UserPassword_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string UserPassword { get; set; }
        [Required(ErrorMessageResourceName = "User_UserEmail_Required", ErrorMessageResourceType = typeof(Resources.WebResources))]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool UserStatus { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<System.DateTime> DeletionDate { get; set; }
    
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<Activity> Activities1 { get; set; }
        public virtual ICollection<Activity> Activities2 { get; set; }
        public virtual ICollection<ActivityNote> ActivityNotes { get; set; }
        public virtual ICollection<ActivityNote> ActivityNotes1 { get; set; }
        public virtual ICollection<ActivityNote> ActivityNotes2 { get; set; }
        public virtual ICollection<ActivityResource> ActivityResources { get; set; }
        public virtual ICollection<ActivityResource> ActivityResources1 { get; set; }
        public virtual ICollection<ActivityType> ActivityTypes { get; set; }
        public virtual ICollection<ActivityType> ActivityTypes1 { get; set; }
        public virtual ICollection<AffiliateType> AffiliateTypes { get; set; }
        public virtual ICollection<AffiliateType> AffiliateTypes1 { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Contact> Contacts1 { get; set; }
        public virtual ICollection<Contact> Contacts2 { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Document> Documents1 { get; set; }
        public virtual ICollection<Document> Documents2 { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Feedback> Feedbacks1 { get; set; }
        public virtual ICollection<Feedback> Feedbacks2 { get; set; }
        public virtual ICollection<Goal> Goals { get; set; }
        public virtual ICollection<Goal> Goals1 { get; set; }
        public virtual ICollection<Goal> Goals2 { get; set; }
        public virtual ICollection<Interest> Interests { get; set; }
        public virtual ICollection<Interest> Interests1 { get; set; }
        public virtual ICollection<Interest> Interests2 { get; set; }
        public virtual ICollection<Medium> Media { get; set; }
        public virtual ICollection<Medium> Media1 { get; set; }
        public virtual ICollection<NoteType> NoteTypes { get; set; }
        public virtual ICollection<NoteType> NoteTypes1 { get; set; }
        public virtual ICollection<OutreachEntityDetail> OutreachEntityDetails { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
        public virtual ICollection<Resource> Resources1 { get; set; }
        public virtual ICollection<School> Schools { get; set; }
        public virtual ICollection<School> Schools1 { get; set; }
        public virtual ICollection<SchoolNote> SchoolNotes { get; set; }
        public virtual ICollection<Unit> Units { get; set; }
        public virtual ICollection<Unit> Units1 { get; set; }
        public virtual UserType UserType { get; set; }
        public virtual ICollection<UserDetail> UserDetails { get; set; }
        public virtual ICollection<UserNote> UserNotes { get; set; }
        public virtual ICollection<UserNote> UserNotes1 { get; set; }
        public virtual ICollection<UserNote> UserNotes2 { get; set; }
        public virtual ICollection<UserNote> UserNotes3 { get; set; }
        public virtual ICollection<OutreachEntityType> OutreachEntityTypes { get; set; }
        public virtual ICollection<OutreachEntityType> OutreachEntityTypes1 { get; set; }
    }
}
