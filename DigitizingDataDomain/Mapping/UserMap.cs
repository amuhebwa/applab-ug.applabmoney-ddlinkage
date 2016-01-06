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
    public class UserMap : ClassMap<Users>
    {
        public UserMap() {
            Id(u => u.Id);
            Map(u => u.Username);
            Map(u => u.Password);
            Map(u => u.Fullname);
            Map(u => u.Email);
            Map(u => u.DateCreated);
            Map(u => u.UserLevel);
        }
    }
}
