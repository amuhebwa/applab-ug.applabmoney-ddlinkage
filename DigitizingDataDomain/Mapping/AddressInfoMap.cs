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
            Map(x => x.EMailAddress).Length(50);
            Map(x => x.FaxNo).Length(50);
            Map(x => x.PostalAddress).Length(50);
            Map(x => x.PostalCode).Length(50);
            Map(x => x.MobilePhoneNo1).Length(50);
            Map(x => x.MobilePhoneNo2).Length(50);
            Map(x => x.TelephoneNo1).Length(50);
            Map(x => x.TelephoneNo2).Length(50);
            Map(x => x.Town).Length(50);

        }
    }
}
