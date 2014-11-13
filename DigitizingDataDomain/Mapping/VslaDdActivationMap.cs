using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class VslaDdActivationMap:ClassMap<VslaDdActivation>
    {
        public VslaDdActivationMap()
        {
            Id(a => a.ActivationId);
            Map(a => a.ActivationDate);
            Map(a => a.IsActive);
            Map(a => a.PassKey);
            Map(a => a.PhoneImei01);
            Map(a => a.PhoneImei02);
            Map(a => a.SimImsiNo01);
            Map(a => a.SimImsiNo02);
            Map(a => a.SimNetworkOperator01);
            Map(a => a.SimNetworkOperator02);
            Map(a => a.SimSerialNo01);
            Map(a => a.SimSerialNo02);

            References(a => a.Vsla)
                .Column("VslaId")
                .Nullable()
                .ForeignKey("FK_DdActivation_Vsla");
        }

    }
}
