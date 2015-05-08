using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class DashboardSummary
    {
        public int totalPresent { get; set; }
        public int totalAbsent { get; set; }
        public long totalVslas { get; set; }
        public int femaleMembers { get; set; }
        public int maleMembers { get; set; }
        public int totalMembers { get; set; }
        public double totalSavings { get; set; }
        public double totalLoans { get; set; }
        public double totalLoanRepayment { get; set; }
        public int totalSubmissions { get; set; }
        public int totalMeeetings { get; set; }
    }
}