using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class AllVslaMeetingInformation
    {
        public List<VslaMeetingInformation> allVslaMeetings { get; set; }
    }
    public class VslaMeetingInformation
    {
        public int MeetingId { get; set; }
        public decimal cashExpenses { get; set; }
        public decimal cashFromBank { get; set; }
        public decimal cashFromBox { get; set; }
        public decimal cashSavedBank { get; set; }
        public decimal cashSavedBox { get; set; }
        public decimal cashWelfare { get; set; }
        public DateTime? dateSent { get; set; }
        public Boolean isCurrent { get; set; }
        public Boolean isDataSent { get; set; }
        public int cycleId { get; set; }
        public decimal cashFines { get; set; }
        public DateTime? meetingDate { get; set; }
        public int membersPresent { get; set; }
        public decimal totalSavings { get; set; }
        public decimal totalLoans { get; set; }
        public decimal totalLoanRepayment { get; set; }
    }
}