using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class AllVslaMeetingInformation
    {
        public List<VslaMeetingInformation> allVslaMeetings { get; set; }
        public string vslaName { get; set; }
    }
    public class VslaMeetingInformation
    {
        public int MeetingId { get; set; }
        public long cashExpenses { get; set; }
        public long cashFromBank { get; set; }
        public long cashFromBox { get; set; }
        public long cashSavedBank { get; set; }
        public long cashSavedBox { get; set; }
        public decimal cashWelfare { get; set; }
        public DateTime? dateSent { get; set; }
        public Boolean isCurrent { get; set; }
        public Boolean isDataSent { get; set; }
        public int cycleId { get; set; }
        public long cashFines { get; set; }
        public DateTime? meetingDate { get; set; }
        public int membersPresent { get; set; }
        public long totalSavings { get; set; }
        public long totalLoans { get; set; }
        public long totalLoanRepayment { get; set; }
    }
}