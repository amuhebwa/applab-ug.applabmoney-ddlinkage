using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DigitizingDataAdminApp.Models
{
    public class UserLogin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Enter Username", AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please Enter Password", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; }
    }
}