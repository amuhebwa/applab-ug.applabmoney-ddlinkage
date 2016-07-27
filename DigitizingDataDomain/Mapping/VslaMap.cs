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
            Map(v => v.ContactPerson);
            Map(v => v.PositionInVsla);
            Map(v => v.PhoneNumber);
            Map(v => v.GroupAccountNumber);
            Map(v => v.NumberOfCycles);
            Map(v => v.Implementer);

            // Reference for technical trainer
            References(v => v.CBT)
                .Column("CBT")
                .Nullable()
                .ForeignKey("default_cbt");
            
            // Reference for region
            References(v => v.VslaRegion)
                .Column("RegionId")
                .Nullable()
                .ForeignKey("FK_Vsla_Region");

            //Vsla is Referenced by VslaDdActivation
            HasMany<VslaDdActivation>(r => r.VslaDdActivationList)
                .KeyColumn("VslaId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Vsla is Referenced by Member
            HasMany<Member>(r => r.MemberList)
                .KeyColumn("VslaId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();

            //Vsla is Referenced by VslaCycle
            HasMany<VslaCycle>(r => r.VslaCycleList)
                .KeyColumn("VslaId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();
        }
    }
}
