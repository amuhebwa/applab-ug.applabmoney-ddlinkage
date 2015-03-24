using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class AllVslaMemberInformation
    {
        public List<VslaMembersInformation> allVslaMembers { get; set; }
        public string vslaName { get; set; }
    }
    public class VslaMembersInformation
    {
        public int memberId { get; set; }
        public int memberNumber { get; set; }
        public int cyclesCompleted { get; set; }
        public string surname { get; set; }
        public string otherNames { get; set; }
        public string gender { get; set; }
        public string occupation { get; set; }
        public DateTime? dateArchived { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string isActive { get; set; }
        public string isArchive { get; set; }
        public string phoneNumber { get; set; }
    }
}