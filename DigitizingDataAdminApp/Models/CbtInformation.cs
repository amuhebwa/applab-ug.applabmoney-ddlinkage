using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitizingDataAdminApp.Models
{
    public class AllCbtInformation
    {
        public List<CbtInformation> AllCbtList { get; set; }
        public CbtInformation cbtDetails { get; set; }
    }
    public class CbtInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public SelectList VslaRegionsModel { get; set; }
        public SelectList StatusType { get; set; }
    }

}