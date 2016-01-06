using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using DigitizingDataDomain.Model;
using DigitizingDataDomain.Helpers;
namespace DigitizingDataBizLayer.Repositories
{
    public class UserRepo : RepositoryBase<Users>
    {
        public Users checkIfUserExists(string username, string password)
        {
            PasswordHashHelper hashHelper = new PasswordHashHelper();
            string hashedPwd = hashHelper.hashedPassword(password);

            var result = (from u in SessionProxy.Query<Users>()
                          where u.Username == username && u.Password == hashedPwd
                          select u).FirstOrDefault();
            return result;
        }

    }
}
