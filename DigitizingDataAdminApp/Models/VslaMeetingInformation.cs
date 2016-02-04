using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigitizingDataAdminApp.Models
{
    public class AllVslaMeetingInformation
    {
        public List<VslaMeetingInformation> allVslaMeetings { get; set; }
        public string vslaName { get; set; }
        public int vslaId { get; set; }
    }
    public class VslaMeetingInformation
    {
        public int MeetingId { get; set; }

        public long cashExpenses { get; set; }
        public string formattedCashExpenses { get { return cashExpenses.ToString("#,##0"); } }

        public long cashFromBank { get; set; }
        public string formattedCashFromBank { get { return cashFromBank.ToString("#,##0"); } }

        public long cashFromBox { get; set; }
        public string formattedCashFromBox { get { return cashFromBox.ToString("#,##0"); } }

        public long cashSavedBank { get; set; }
        public string formattedCashSavedBank { get { return cashSavedBank.ToString("#,##0"); } }

        public long cashSavedBox { get; set; }
        public string formattedCashSavedBox { get { return cashSavedBox.ToString("#,##0"); } }

        public decimal cashWelfare { get; set; }

        public DateTime? dateSent { get; set; }

        public Boolean isCurrent { get; set; }

        public Boolean isDataSent { get; set; }

        public int cycleId { get; set; }

        public long cashFines { get; set; }
        public string formattedCashFines { get { return cashFines.ToString("#,##0"); } }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? meetingDate { get; set; }
        public string formattedMeetingDate { get { return meetingDate.Value.ToString("dd-MMM-yyyy"); } }

        public int membersPresent { get; set; }

        public long totalSavings { get; set; }
        public string formattedTotalSavings { get { return totalSavings.ToString("#,##0"); } }

        public long totalLoans { get; set; }
        public string formattedTotalLoans { get { return totalLoans.ToString("#,##0"); } }

        public long totalLoanRepayment { get; set; }
        public string formattedLoanRepayment { get { return totalLoanRepayment.ToString("#,##0"); } }

        // Difference between meeting date and date of submission
        public int diffInDaysBtnHeld_Submit { get; set; }
    }
}