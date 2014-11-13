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
            Map(c => c.InterestRate).CustomSqlType("decimal(18,2)");
            Map(c => c.IsEnded);
            Map(c => c.MaxShareQuantity);
            Map(c => c.MaxStartShare).CustomSqlType("decimal(18,2)");
            Map(c => c.SharedAmount).CustomSqlType("decimal(18,2)"); 
            Map(c => c.SharePrice).CustomSqlType("decimal(18,2)"); 
            Map(c => c.StartDate);
            Map(c => c.MigratedInterest).CustomSqlType("decimal(18,2)"); 
            Map(c => c.MigratedFines).CustomSqlType("decimal(18,2)"); 

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
