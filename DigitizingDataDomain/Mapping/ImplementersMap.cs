using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;
namespace DigitizingDataDomain.Mapping
{
    public class ImplementersMap : ClassMap<Implementers>
    {
        public ImplementersMap() {
            Id(I => I.ImplementerId);
            Map(I => I.ImplementerName);
        }
    }
}
