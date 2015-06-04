using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using DigitizingDataDomain.Model;
namespace DigitizingDataBizLayer.Repositories
{
    public class UsersRepo : RepositoryBase<Users>
    {
        public List<Users> getAllRegisteredUsers() { 
            var allUsers = (from u in SessionProxy.Query<Users>() select u).ToList();
            return allUsers;
        }
    }
}
