using System;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class TechnicalTrainerMap : ClassMap<TechnicalTrainer>
    {
        public TechnicalTrainerMap() {
            Id(t => t.Id);
            Map(t => t.Name);
            Map(t => t.PhoneNumber);
            Map(t => t.Email);
            Map(t => t.Status);
            Map(t => t.FirstName);
            Map(t => t.LastName);
            Map(t => t.Username);
            Map(t => t.Passkey);

            // Has Foreign key columns
            References(t => t.VslaRegion)
                .Column("Region")
                .Nullable()
                .ForeignKey("FK_Cbt_info_VslaRegion");

        
        }
    }
}
