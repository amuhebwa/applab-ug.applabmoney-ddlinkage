using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using DigitizingDataDomain.Model;

namespace DigitizingDataBizLayer.Repositories
{
    public class VslaRegionRepo : RepositoryBase<VslaRegion>
    {
        public List<VslaRegion> findAllRegions()
        {
            var allRegions = (from r in SessionProxy.Query<VslaRegion>()
                              select r).ToList();
            return allRegions;
        }

        // Find region by Id
        public VslaRegion findVslaRegionById(int regionId)
        {
            var region = (from r in SessionProxy.Query<VslaRegion>()
                          where r.RegionId == regionId
                          select r).FirstOrDefault();
            return region;
        }
    }
}
