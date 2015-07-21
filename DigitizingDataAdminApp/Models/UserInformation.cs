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
        public int SessionUserLevel {get;set;}
    }

    public class UserInformation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is a required field", AllowEmptyStrings = false)]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Username Format")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Full name is a required field", AllowEmptyStrings = false)]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Fullname Format")]
        public string Fullname { get; set; }

        [Required(ErrorMessage="Email is a required field", AllowEmptyStrings=false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is a required field", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string UserLevel { get; set; }

        public DateTime? DateCreated { get; set; }

        // Short Date Format ie yy/mm/dd
        public string FormattedDateCreated { get { return DateCreated.HasValue ? DateCreated.Value.ToShortDateString() : "-NA-"; } }

        public SelectList AccessLevel { get; set; }
    }
}