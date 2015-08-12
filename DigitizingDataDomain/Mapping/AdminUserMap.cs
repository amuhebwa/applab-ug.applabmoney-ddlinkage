using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class AdminUserMap : ClassMap<AdminUser>
    {
        public AdminUserMap()
        {
            Id(u => u.UserId);
            Map(u => u.Username).Length(50).UniqueKey("Ak_Username").Not.Nullable();
            Map(u => u.Surname).Length(50);
            Map(u => u.OtherNames).Length(65);
            Map(u => u.SecurityToken).Length(65);
            Map(u => u.Password).Length(65);
            Map(u => u.ActivationDate);
            Map(u => u.ActivationPhoneImei);
            Component(u => u.AddressInfo);
        }
    }
}
