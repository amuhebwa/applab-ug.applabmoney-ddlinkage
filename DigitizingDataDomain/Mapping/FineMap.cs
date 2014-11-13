using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class FineMap : ClassMap<Fine>
    {
        public FineMap()
        {
            Id(a => a.FineId);
            Map(a => a.FineIdEx);
            Map(a => a.Amount).CustomSqlType("decimal(18,2)");
            Map(a => a.ExpectedDate);
            Map(a => a.IsCleared);
            Map(a => a.DateCleared);           

            //Has Foreign Key Columns
            References(a => a.IssuedInMeeting)
                .Column("IssuedInMeetingId")
                .Nullable()
                .ForeignKey("FK_Fine_Meeting_Issued");

            //Has Foreign Key Columns
            References(a => a.PaidInMeeting)
                .Column("PaidInMeetingId")
                .Nullable()
                .ForeignKey("FK_Fine_Meeting_Paid");

            //Has Foreign Key Columns
            References(a => a.Member)
                .Column("MemberId")
                .Nullable()
                .ForeignKey("FK_Fine_Member");
        }
    }
}
