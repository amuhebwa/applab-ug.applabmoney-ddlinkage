using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class UsersMap : ClassMap<Users>
    {
        public UsersMap()
        {
            Id(u => u.UserId);
            Map(u => u.Username).Length(250);
            Map(u => u.Fullname).Length(250);
            Map(u => u.Password).Length(100);
            Map(u => u.Email).Length(100);
            Map(u => u.DateCreated);
            Map(u => u.UserLevel);
            Table("Users");
        }
    }
}
