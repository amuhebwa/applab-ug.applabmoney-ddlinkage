using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigitizingDataAdminApp.Models
{
    public class AllVslaInformation
    {
        public List<VslaInformation> AllVslaList { get; set; }
        public VslaInformation VslaDetails { get; set; }

    }
    public class VslaInformation
    {
        public int VslaId { get; set; }

        [Required(ErrorMessage = "VSLA Code is a required field", AllowEmptyStrings = false)]
        public string VslaCode { get; set; }

        [Required(ErrorMessage = "VSLA Name is a required field", AllowEmptyStrings = false)]
        public string VslaName { get; set; }

        public string RegionId { get; set; }

        [Required(ErrorMessage = "Date Registered is a require field", AllowEmptyStrings = false)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateRegistered { get; set; }
        public string formattedDateRegistered { get { return DateRegistered.Value.ToShortDateString(); } }

        [Required(ErrorMessage = "Date Linked is a require field", AllowEmptyStrings = false)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateLinked { get; set; }
        public string formattedDateLinked { get { return DateLinked.Value.ToShortDateString(); } }

        [Required(ErrorMessage = "Physical is a require field", AllowEmptyStrings = false)]
        public string PhysicalAddress { get; set; }

        [Required(ErrorMessage = "Phone MSISDN is a require field", AllowEmptyStrings = false)]
        public string VslaPhoneMsisdn { get; set; }

        [Required(ErrorMessage = "GPS Location is a require field", AllowEmptyStrings = false)]
        public string GpsLocation { get; set; }

        [Required(ErrorMessage = "Contact Person is a require field", AllowEmptyStrings = false)]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Position of Contact Person is a require field", AllowEmptyStrings = false)]
        public string PositionInVsla { get; set; }

        [Required(ErrorMessage = "Phone Number is a require field", AllowEmptyStrings = false)]
        public string PhoneNumber { get; set; }

        public string CBT { get; set; }

        public SelectList CbtModel { get; set; }

        public SelectList VslaRegionsModel { get; set; }

        public string Status { get; set; }

        public SelectList StatusType { get; set; }
    }
}