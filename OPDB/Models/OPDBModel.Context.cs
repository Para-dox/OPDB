﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OPDBEntities : DbContext
    {
        public OPDBEntities()
            : base("name=OPDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityNote> ActivityNotes { get; set; }
        public DbSet<ActivityResource> ActivityResources { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<AffiliateType> AffiliateTypes { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<NoteType> NoteTypes { get; set; }
        public DbSet<OutreachEntityDetail> OutreachEntityDetails { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<SchoolNote> SchoolNotes { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<UserNote> UserNotes { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OutreachEntityType> OutreachEntityTypes { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Medium> Media { get; set; }
        public DbSet<ActivityDynamic> ActivityDynamics { get; set; }
        public DbSet<ActivityMajor> ActivityMajors { get; set; }
        public DbSet<SchoolRegion> SchoolRegions { get; set; }
    }
}
