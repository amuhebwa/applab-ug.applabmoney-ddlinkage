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
    public class UserPermissionsMap : ClassMap<UserPermissions>
    {
        public UserPermissionsMap()
        {
            Id(u => u.Level_Id);
            Map(u => u.UserType);
        }
    }
}
