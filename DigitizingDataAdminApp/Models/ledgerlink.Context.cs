﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DigitizingDataAdminApp.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ledgerlinkEntities : DbContext
    {
        public ledgerlinkEntities()
            : base("name=ledgerlinkEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Audit_Log> Audit_Log { get; set; }
        public DbSet<Cbt_info> Cbt_info { get; set; }
        public DbSet<DataSubmission> DataSubmissions { get; set; }
        public DbSet<DataSubmission_20140113> DataSubmission_20140113 { get; set; }
        public DbSet<DataSubmission_20140113_2> DataSubmission_20140113_2 { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<LoanIssue> LoanIssues { get; set; }
        public DbSet<LoanRepayment> LoanRepayments { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Saving> Savings { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vsla> Vslas { get; set; }
        public DbSet<VslaCycle> VslaCycles { get; set; }
        public DbSet<VslaDdActivation> VslaDdActivations { get; set; }
        public DbSet<VslaRegion> VslaRegions { get; set; }
    }
}
