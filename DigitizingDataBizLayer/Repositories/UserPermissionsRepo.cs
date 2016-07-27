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
    public class UserPermissionsRepo : RepositoryBase<UserPermissions>
    {
        public List<UserPermissions> allUserPermissions()
        {
            var permissions = (from p in SessionProxy.Query<UserPermissions>()
                               select p).ToList();
            return permissions;
        }
    }
}
