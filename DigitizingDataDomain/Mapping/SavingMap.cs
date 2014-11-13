using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class SavingMap : ClassMap<Saving>
    {
        public SavingMap()
        {
            Id(a => a.SavingId);
            Map(a => a.SavingIdEx);
            Map(a => a.Amount).CustomSqlType("decimal(18,2)");

            //Has Foreign Key Columns
            References(a => a.Meeting)
                .Column("MeetingId")
                .Nullable()
                .ForeignKey("FK_Saving_Meeting");

            //Has Foreign Key Columns
            References(a => a.Member)
                .Column("MemberId")
                .Nullable()
                .ForeignKey("FK_Saving_Member");
        }
    }
}
