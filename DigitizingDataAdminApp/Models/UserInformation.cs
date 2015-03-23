using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigitizingDataAdminApp.Models
{
    public class AllUsersInformation
    {
        public List<UserInformation> AllUsersList { get; set; }
        public UserInformation userDetails { get; set; }
    }
    // Setters and getters for the individual user information
    public class UserInformation
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is a required field", AllowEmptyStrings = false)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Full name is a required field", AllowEmptyStrings = false)]
        public string Fullname { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is a required field", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; }
        // public DateTime DateCreated { get; set; }
        public string UserLevel { get; set; }
        public SelectList UserTypes { get; set; }
        public SelectList AccessLevel { get; set; }
    }
}