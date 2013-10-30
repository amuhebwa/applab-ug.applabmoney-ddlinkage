using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class VslaMap : ClassMap<Vsla>
    {
        public VslaMap()
        {
            Id(v => v.VslaId);
            Map(v => v.VslaCode).UniqueKey("AK_VslaCode").Not.Nullable();
            Map(v => v.VslaName).Length(255).UniqueKey("AK_VslaName").Not.Nullable();
            Map(v => v.VslaPhoneMsisdn).Length(20);
            Map(v => v.PhysicalAddress).Length(255);
            Map(v => v.GpsLocation).Length(100);
            Map(v => v.DateRegistered);
            Map(v => v.DateLinked);

            References(v => v.VslaRegion)
                .Column("RegionId")
                .Nullable()
                .ForeignKey("FK_Vsla_Region");
        }
    }
}
