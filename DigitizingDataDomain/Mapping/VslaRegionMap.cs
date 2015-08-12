using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class VslaRegionMap : ClassMap<VslaRegion>
    {
        public VslaRegionMap()
        {
            Id(r => r.RegionId);
            Map(r => r.RegionCode).UniqueKey("AK_RegionCode").Not.Nullable();
            Map(r => r.RegionName).UniqueKey("AK_RegionName").Not.Nullable();

            //VslaRegion is Referenced by Vsla
            HasMany<Vsla>(r => r.VslaList)
                .KeyColumn("RegionId")
                .Inverse()
                .AsBag()    //Use Bag instead of List to avoid index updating issues                
                .Cascade.SaveUpdate()
                .LazyLoad();
        }
    }
}
