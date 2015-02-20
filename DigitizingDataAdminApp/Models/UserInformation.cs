using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class AllUsersInformation 
    {
        public List<UserInformation> AllUsersList { get; set;}
        public UserInformation userDetails { get; set; }
    }
    // Setters and getters for the individual user information
    public class UserInformation
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
       // public DateTime DateCreated { get; set; }
        public string UserLevel { get; set; }
    }
}