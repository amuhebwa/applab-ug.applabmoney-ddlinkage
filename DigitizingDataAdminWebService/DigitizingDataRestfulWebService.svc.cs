using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DigitizingDataAdminWebService
{
    public class DigitizingDataRestfulWebService : IDigitizingDataRestfulWebService
    {
        /**
         * Get all users
         */
        public List<UsersDetails> getRegisteredUsers()
        {
            ledgerlinkDataContext database = new ledgerlinkDataContext();
            List<UsersDetails> results = new List<UsersDetails>();
            foreach (var user in database.Users)
            {
                results.Add(new UsersDetails()
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Fullname = user.Fullname,
                    // Password = user.Password,
                    Email = user.Email,
                    DateCreated = user.DateCreated,
                    UserLevel = user.UserLevel
                });
            }
            return results;
        }
        /**
         * Get the list of all VSLAs
         */
        public List<VslaDetails> getAllVslas() {
            List<VslaDetails> results = new List<VslaDetails>();

            return results;
        }
    }
}
