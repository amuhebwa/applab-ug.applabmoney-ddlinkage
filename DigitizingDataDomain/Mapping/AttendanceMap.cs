using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class AttendanceMap : ClassMap<Attendance>
    {
        public AttendanceMap()
        {
            Id(a => a.AttendanceId);
            Map(a => a.AttendanceIdEx);
            Map(a => a.Comments).Length(50);
            Map(a => a.IsPresent);

            //Has Foreign Key Columns
            References(a => a.Meeting)
                .Column("MeetingId")
                .Nullable()
                .ForeignKey("FK_Attendance_Meeting");

            //Has Foreign Key Columns
            References(a => a.Member)
                .Column("MemberId")
                .Nullable()
                .ForeignKey("FK_Attendance_Member");
        }
    }
}
