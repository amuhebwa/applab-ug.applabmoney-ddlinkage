using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitizingDataDomain.Collections;

namespace DigitizingDataDomain.Model
{
    public class VslaRegion
    {
        public virtual int RegionId { get; set; }
        public virtual string RegionCode { get; set; }
        public virtual string RegionName { get; set; }

        //Collections: Hacked to allow for reporting
        public virtual IList<Vsla> VslaList { get; set; }
        public virtual AggregationBindingList<Vsla> Vslas
        {
            get
            {
                return new AggregationBindingList<Vsla>(VslaList);
            }
        }
    }
}
