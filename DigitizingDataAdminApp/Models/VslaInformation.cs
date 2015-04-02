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
        public string VslaCode { get; set; }
        public string VslaName { get; set; }
        public string RegionId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateRegistered { get; set; }
        public string formattedDateRegistered { get { return DateRegistered.Value.ToShortDateString(); } }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateLinked { get; set; }
        public string formattedDateLinked { get { return DateLinked.Value.ToShortDateString(); } }

        public string PhysicalAddress { get; set; }
        public string VslaPhoneMsisdn { get; set; }
        public string GpsLocation { get; set; }
        public string ContactPerson { get; set; }
        public string PositionInVsla { get; set; }
        public string PhoneNumber { get; set; }
        public string CBT { get; set; }
        public SelectList CbtModel { get; set; }
        public SelectList VslaRegionsModel { get; set; }
        public string Status { get; set; }
        public SelectList StatusType { get; set; }
    }
}