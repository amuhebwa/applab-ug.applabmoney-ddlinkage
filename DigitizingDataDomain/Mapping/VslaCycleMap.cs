using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class VslaCycleMap:ClassMap<VslaCycle>
    {
        public VslaCycleMap()
        {
            Id(c => c.CycleId);
            Map(c => c.CycleIdEx);
            Map(c => c.DateEnded);
            Map(c => c.EndDate);
            Map(c => c.CycleCode).Length(20);
            Map(c => c.InterestRate);
            Map(c => c.IsEnded);
            Map(c => c.MaxShareQuantity);
            Map(c => c.MaxStartShare);
            Map(c => c.SharedAmount);
            Map(c => c.SharePrice);
            Map(c => c.StartDate);

            //Has Foreign Key Columns
            References(c => c.Vsla)
                .Column("VslaId")
                .Nullable()
                .ForeignKey("FK_VslaCycle_Vsla");

            //VSLA CYCLE is Referenced by Meeting
            HasMany<Meeting>(r => r.MeetingList)
                .KeyColumn("CycleId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();
        }
    }
}
