using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using DigitizingDataDomain.Model;
using NHibernate.Criterion;

namespace DigitizingDataBizLayer.Repositories
{
    public class UserRepo : RepositoryBase<Users>
    {
        public Users checkIfUserExists(string username, string password)
        {
            DigitizingDataDomain.Helpers.PasswordHashHelper hashHelper = new DigitizingDataDomain.Helpers.PasswordHashHelper();
            string hashedPwd = hashHelper.hashedPassword(password);

            var result = (from u in SessionProxy.Query<Users>()
                          where u.Username == username && u.Password == hashedPwd
                          select u).FirstOrDefault();
            return result;

        }

        // Get all registered system users
        public List<Users> findAllUsers()
        {
            var users = (from u in SessionProxy.Query<Users>()
                         select u).ToList();
            return users;
        }

        // Get a  information for a particular
        public List<Users> findParticularUser(int userLevel, string userName)
        {
            var user = (from u in SessionProxy.Query<Users>()
                        where u.UserLevel == userLevel && u.Username == userName
                        select u).ToList();
            return user;
        }

        // Get a single user's details
        public Users findUserDetails(int id)
        {
            var user = (from u in SessionProxy.Query<Users>()
                        where u.Id == id
                        select u).FirstOrDefault();
            return user;
        }

        // Find user by Id
        public Users findUserById(int id)
        {
            var userx = (from u in SessionProxy.Query<Users>()
                         where u.Id == id
                         select u).FirstOrDefault();
            return userx;
        }

    }
}
