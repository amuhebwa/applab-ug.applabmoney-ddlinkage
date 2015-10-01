using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigitizingDataAdminApp.Models
{
    public class AllTrainersInformation : TrainerInformation
    {
        public List<TrainerInformation> AllTrainersList { get; set; }
        public TrainerInformation trainerDetails { get; set; }
        public int SessionUserLevel { get; set; }
    }
    public class TrainerInformation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "full Name is a required field", AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(ErrorMessage="FirstName is a required field", AllowEmptyStrings=false)]
        public string FirstName { get; set; }

        [Required(ErrorMessage="LastName is a required field", AllowEmptyStrings=false)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is a required field", AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Passkey is a required field", AllowEmptyStrings = false)]
        public string Passkey { get; set; }

        public string Region { get; set; }

        [Required(ErrorMessage = "Phone Number is a required field", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.PhoneNumber)]
        [StringLength(10, ErrorMessage = "10 Characters")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is a required field", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        public string Email { get; set; }

        public string Status { get; set; }

        public SelectList VslaRegionsModel { get; set; }

        public SelectList StatusType { get; set; }
    }

}