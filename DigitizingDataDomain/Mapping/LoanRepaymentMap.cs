using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class LoanRepaymentMap : ClassMap<LoanRepayment>
    {
        public LoanRepaymentMap()
        {
            Id(a => a.RepaymentId);
            Map(a => a.RepaymentIdEx);
            Map(a => a.Amount).CustomSqlType("decimal(18,2)");
            Map(a => a.BalanceAfter).CustomSqlType("decimal(18,2)");
            Map(a => a.BalanceBefore).CustomSqlType("decimal(18,2)");
            Map(a => a.Comments).Length(50);
            Map(a => a.LastDateDue);
            Map(a => a.NextDateDue);
            Map(a => a.InterestAmount).CustomSqlType("decimal(18,2)");
            Map(a => a.RolloverAmount).CustomSqlType("decimal(18,2)");

            //Has Foreign Key Columns
            References(a => a.Meeting)
                .Column("MeetingId")
                .Nullable()
                .ForeignKey("FK_LoanRepayment_Meeting");

            //Has Foreign Key Columns
            References(a => a.Member)
                .Column("MemberId")
                .Nullable()
                .ForeignKey("FK_LoanRepayment_Member");

            //Has Foreign Key Columns
            References(a => a.LoanIssue)
                .Column("LoanId")
                .Nullable()
                .ForeignKey("FK_LoanRepayment_LoanIssue");
        }
    }
}
