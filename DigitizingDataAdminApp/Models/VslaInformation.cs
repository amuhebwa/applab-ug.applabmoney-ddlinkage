using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigitizingDataAdminApp.Models
{
    public class VslaGroupsInformation : VslaInformation
    {
        public List<VslaInformation> AllGroupsList { get; set; }
        public VslaInformation GroupDetails { get; set; }
        public int sessionUserLevel { get; set; }

    }
    public class VslaInformation
    {
        public int VslaId { get; set; }

        [Required(ErrorMessage = "VSLA Code is a required field", AllowEmptyStrings = false)]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid VSLA Code Format")]
        public string VslaCode { get; set; }

        [Required(ErrorMessage = "VSLA Name is a required field", AllowEmptyStrings = false)]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid VSLA Name")]
        public string VslaName { get; set; }

        public string RegionId { get; set; }

        [Required(ErrorMessage = "Date Registered is a require field", AllowEmptyStrings = false)]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateRegistered { get; set; }

        [Required(ErrorMessage = "Date Linked is a require field", AllowEmptyStrings = false)]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateLinked { get; set; }

        [Required(ErrorMessage = "Physical is a require field", AllowEmptyStrings = false)]
        public string PhysicalAddress { get; set; }

        [Required(ErrorMessage = "Phone MSISDN is a require field", AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9]{10,20}$", ErrorMessage = "Minimum of 10 characters ")]
        public string VslaPhoneMsisdn { get; set; }

        [Required(ErrorMessage = "GPS Location is a require field", AllowEmptyStrings = false)]
        public string GpsLocation { get; set; }

        [Required(ErrorMessage = "Contact Person is a require field", AllowEmptyStrings = false)]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Name format")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Position of Contact Person is a require field", AllowEmptyStrings = false)]
        public string PositionInVsla { get; set; }

        [Required(ErrorMessage = "Phone Number is a require field", AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9]{10,14}$", ErrorMessage = "Minimum of 10 characters ")]
        public string PhoneNumber { get; set; }

        public string TechnicalTrainer { get; set; }

        public SelectList AllTechnicalTrainers { get; set; }

        public SelectList VslaRegions { get; set; }

        public SelectList StatusType { get; set; }

        public string RegionName { get; set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "Account Number is a required Field", AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9]{10,10}$", ErrorMessage = "A/C is 10 characters ")]
        public string GroupAccountNumber { get; set; }
    }
}