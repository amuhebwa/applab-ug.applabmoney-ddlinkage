using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class AddressInfoMap : ComponentMap<AddressInfo>
    {
        public AddressInfoMap()
        {
            Map(x => x.EMailAddress);
            Map(x => x.FaxNo);
            Map(x => x.PostalAddress);
            Map(x => x.PostalCode);
            Map(x => x.MobilePhoneNo1);
            Map(x => x.MobilePhoneNo2);
            Map(x => x.TelephoneNo1);
            Map(x => x.TelephoneNo2);
            Map(x => x.Town);

        }
    }
}
