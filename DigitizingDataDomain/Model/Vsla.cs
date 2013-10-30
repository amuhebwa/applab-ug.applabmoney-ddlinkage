using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class Vsla
    {
        public virtual int VslaId { get; set; }
        public virtual string VslaCode { get; set; }
        public virtual string VslaName { get; set; }
        public virtual VslaRegion VslaRegion { get; set; }
        public virtual DateTime? DateRegistered { get; set; }
        public virtual DateTime? DateLinked { get; set; }
        public virtual string PhysicalAddress { get; set; }
        public virtual string VslaPhoneMsisdn { get; set; }
        public virtual string GpsLocation { get; set; }
    }
}
