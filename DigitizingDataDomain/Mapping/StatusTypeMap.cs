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
    public class StatusTypeMap : ClassMap<StatusType>
    {
        public StatusTypeMap() {
            Id(s => s.Status_Id);
            Map(s => s.CurrentStatus);
        }
    }
}
