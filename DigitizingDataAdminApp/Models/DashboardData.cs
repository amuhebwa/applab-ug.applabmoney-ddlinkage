using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class DashboardData
    {
        public int totalPresent { get; set; }

        public int totalAbsent { get; set; }

        public long totalVslas { get; set; }
        public string fTotalVslas { get { return totalVslas.ToString("#,##0"); } }

        public int femaleMembers { get; set; }
        public string fFemales { get { return femaleMembers.ToString("#,##0"); } }

        public int maleMembers { get; set; }
        public string fMales { get { return maleMembers.ToString("#,##0"); } }

        public int totalMembers { get; set; }
        public string fMembers { get { return totalMembers.ToString("#,##0"); } }

        public double totalSavings { get; set; }
        public string fTotalSavings { get { return totalSavings.ToString("#,##0"); } }

        public double totalLoans { get; set; }
        public string fLoans { get { return totalLoans.ToString("#,##0"); } }

        public double totalLoanRepayment { get; set; }
        public string fLoanRepayment { get { return totalLoanRepayment.ToString("#,##0"); } }

        public int totalSubmissions { get; set; }
        public string fTotalSubmissions { get { return totalSubmissions.ToString("#,##0"); } }

        public int totalMeeetings { get; set; }
        public string fTotalMeetings { get { return totalMeeetings.ToString("#,##0"); } }
    }
}