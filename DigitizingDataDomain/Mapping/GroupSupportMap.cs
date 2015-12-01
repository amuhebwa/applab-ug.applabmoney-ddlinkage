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
    public class GroupSupportMap : ClassMap<GroupSupport>
    {
        public GroupSupportMap()
        {
            Id(g => g.SupportId);
            Map(g => g.SupportType);
            Map(g => g.SupportDate);

            //Reference for VSLA
            References(g => g.VslaId)
                .Column("VslaId")
                .Nullable()
                .ForeignKey("FK_GroupSupport_Vsla");

            // References for Technical trainer/CBT
            References(g => g.TrainerId)
                .Column("TrainerId")
                .Nullable()
                .ForeignKey("FK_GroupSupport_Cbt_info");

        }
    }
}
