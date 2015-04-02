using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigitizingDataAdminApp.Models
{
    public class AllVslaMemberInformation
    {
        public List<VslaMembersInformation> allVslaMembers { get; set; }
        public string vslaName { get; set; }
        public int vslaId { get; set; } // Id of the vsla on which members are attached
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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? dateArchived { get; set; }
        public string formattedDateArchived { get { return dateArchived.Value.ToShortDateString(); } }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
        public string formattedDateOfBirth { get { return DateOfBirth.Value.ToShortDateString(); } }

        public string isActive { get; set; }
        public string isArchive { get; set; }
        public string phoneNumber { get; set; }
    }
}