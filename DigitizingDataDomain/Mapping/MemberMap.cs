using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class MemberMap : ClassMap<Member>
    {
        public MemberMap()
        {
            Id(m => m.MemberId);
            Map(m => m.MemberIdEx);
            Map(m=>m.MemberNo);            
            Map(m => m.CyclesCompleted);
            Map(m => m.Surname).Not.Nullable().Length(30);
            Map(m => m.OtherNames).Length(50);
            Map(m => m.Gender).Length(10);
            Map(m => m.Occupation).Length(50);
            Map(m => m.DateArchived);
            Map(m => m.DateOfBirth);
            Map(m => m.IsActive);
            Map(m => m.IsArchived);
            Map(m => m.PhoneNo).Length(20);

            References(m => m.Vsla)
                .Column("VslaId")
                .Nullable()
                .ForeignKey("FK_Member_Vsla");

            //Referenced by LoanRepayment
            HasMany<LoanRepayment>(r => r.LoanRepaymentList)
                .KeyColumn("MemberId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Referenced by LoanIssue
            HasMany<LoanIssue>(r => r.LoanIssueList)
                .KeyColumn("MemberId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Referenced by Attendance
            HasMany<Attendance>(r => r.AttendanceList)
                .KeyColumn("MemberId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Referenced by Saving
            HasMany<Saving>(r => r.SavingList)
                .KeyColumn("MemberId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Referenced by FineIssues
            HasMany<Fine>(r => r.FineList)
                .KeyColumn("MemberId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

        }
    }
}

/*
MEMBER_ID INT NOT NULL IDENTITY(1,1),
    MEMBER_ID_EX INT NOT NULL,
    VSLA_ID INT NOT NULL,
    MEMBER_NO INT NOT NULL,
    SURNAME NVARCHAR(30) NOT NULL,
    OTHER_NAMES NVARCHAR(50),
    GENDER NVARCHAR(10),
    DATE_OF_BIRTH DATETIME,
    CYCLES_COMPLETED INT,
    OCCUPATION NVARCHAR(50),
    PHONE_NO NVARCHAR(15),
    IS_ACTIVE BIT DEFAULT 0,
    IS_ARCHIVED BIT DEFAULT 0,
    DATE_ARCHIVED DATETIME,
*/