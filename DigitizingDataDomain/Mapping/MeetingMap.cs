using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class MeetingMap : ClassMap<Meeting>
    {
        public MeetingMap()
        {
            Id(a => a.MeetingId);
            Map(a => a.MeetingIdEx);
            Map(a => a.CashExpenses).CustomSqlType("decimal(18,2)");
            Map(a => a.CashFines).CustomSqlType("decimal(18,2)");
            Map(a => a.CashFromBank).CustomSqlType("decimal(18,2)");
            Map(a => a.CashFromBox).CustomSqlType("decimal(18,2)");
            Map(a => a.CashSavedBank).CustomSqlType("decimal(18,2)");
            Map(a => a.CashSavedBox).CustomSqlType("decimal(18,2)");
            Map(a => a.CashWelfare).CustomSqlType("decimal(18,2)");
            Map(a => a.DateSent);
            Map(a => a.IsCurrent);
            Map(a => a.IsDataSent);
            Map(a => a.MeetingDate);
            Map(a => a.CountOfMembersPresent);
            Map(a => a.SumOfSavings).CustomSqlType("decimal(18,2)");
            Map(a => a.SumOfLoanIssues).CustomSqlType("decimal(18,2)");
            Map(a => a.SumOfLoanRepayments).CustomSqlType("decimal(18,2)");

            //Has Foreign Key Columns
            References(c => c.VslaCycle)
                .Column("CycleId")
                .Nullable()
                .ForeignKey("FK_Meeting_VslaCycle");

            //Referenced by LoanRepayment
            HasMany<LoanRepayment>(r => r.LoanRepaymentList)
                .KeyColumn("MeetingId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Referenced by LoanIssue
            HasMany<LoanIssue>(r => r.LoanIssueList)
                .KeyColumn("MeetingId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Referenced by Attendance
            HasMany<Attendance>(r => r.AttendanceList)
                .KeyColumn("MeetingId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Referenced by Saving
            HasMany<Saving>(r => r.SavingList)
                .KeyColumn("MeetingId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Referenced by FinesIssued
            HasMany<Fine>(r => r.FineIssuedList)
                .KeyColumn("MeetingId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Referenced by FinesPaid
            HasMany<Fine>(r => r.FinePaidList)
                .KeyColumn("MeetingId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();
        }
    }
}
