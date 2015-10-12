using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DigitizingDataAdminApp.Models
{
    public class AllMeetingsData
    {
        public List<WeeklyMeetingsData> meetingsSummary { get; set; }
    }
    public class WeeklyMeetingsData
    {
        public int meetingId { get; set; }

        public int vslaId { get; set; }

        public long cashExpenses { get; set; }
        public string fCashExpenses { get { return cashExpenses.ToString("#,##0"); } }

        public long cashFines { get; set; }
        public string fCashFines { get { return cashFines.ToString("#,##0"); } }

        public long cashFromBank { get; set; }
        public string fCashFromBank { get { return cashFromBank.ToString("#,##0"); } }

        public long cashFromBox { get; set; }
        public string fCashFromBox { get { return cashFromBox.ToString("#,##0"); } }

        public long cashSavedBank { get; set; }
        public string fCashSavedBank { get { return cashSavedBank.ToString("#,##0"); } }

        public long cashSavedBox { get; set; }
        public string fCashSavedBox { get { return cashSavedBox.ToString("#,##0"); } }

        public long cashWelfare { get; set; }
        public string fCashWelfare { get { return cashWelfare.ToString("#,##0"); } }

        [DisplayFormat(DataFormatString = "{0:MMM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? dateSent { get; set; }
        public string formattedDateSent { get { return dateSent.Value.ToString("MMM-dd-yyyy"); } }

        [DisplayFormat(DataFormatString = "{0:MMM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? meetingDate { get; set; }
        public string formattedMeetingDate { get { return meetingDate.Value.ToString("MMM-dd-yyyy"); } }

        public int countOfMembersPresent { get; set; }

        public long sumOfSavings { get; set; }
        public string fSumOfSavings { get { return sumOfSavings.ToString("#,##0"); } }

        public long sumOfLoansIssued { get; set; }
        public string fSumOfLoansIssued { get { return sumOfLoansIssued.ToString("#,##0"); } }

        public long sumOfLoanRepayments { get; set; }
        public string fLoanRepayment { get { return sumOfLoanRepayments.ToString("#,##0"); } }

        public string vslaName { get; set; }

        public string VslaCode { get; set; }

    }
}