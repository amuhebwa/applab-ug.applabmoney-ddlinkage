using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class AddressInfo
    {
        public virtual string PostalAddress { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string Town { get; set; }
        public virtual string EMailAddress { get; set; }
        public virtual string MobilePhoneNo1 { get; set; }
        public virtual string MobilePhoneNo2 { get; set; }
        public virtual string TelephoneNo1 { get; set; }
        public virtual string TelephoneNo2 { get; set; }
        public virtual string FaxNo { get; set; }

        public override string ToString()
        {
            return String.Format("P.O. Box {0}-{1}, {2}", this.PostalAddress, this.PostalCode, this.Town);
        }
    }
}
