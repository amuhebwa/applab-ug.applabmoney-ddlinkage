using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class LoanIssueMap : ClassMap<LoanIssue>
    {
        public LoanIssueMap()
        {
            Id(a => a.LoanId);
            Map(a => a.LoanIdEx);
            Map(a => a.LoanNo);
            Map(a => a.PrincipalAmount).CustomSqlType("decimal(18,2)");
            Map(a => a.InterestAmount).CustomSqlType("decimal(18,2)");
            Map(a => a.Balance).CustomSqlType("decimal(18,2)");
            Map(a => a.Comments).Length(50);
            Map(a => a.DateCleared);
            Map(a => a.DateDue);
            Map(a => a.IsCleared);
            Map(a => a.IsDefaulted);
            Map(a => a.TotalRepaid).CustomSqlType("decimal(18,2)");
            Map(a => a.IsWrittenOff);

            //Has Foreign Key Columns
            References(a => a.Meeting)
                .Column("MeetingId")
                .Nullable()
                .ForeignKey("FK_LoanIssue_Meeting");

            //Has Foreign Key Columns
            References(a => a.Member)
                .Column("MemberId")
                .Nullable()
                .ForeignKey("FK_LoanIssue_Member");

            //Referenced by LoanRepayment
            HasMany<LoanRepayment>(r => r.LoanRepaymentList)
                .KeyColumn("LoanId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();
        }
    }
}
