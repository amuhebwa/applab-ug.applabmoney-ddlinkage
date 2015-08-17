using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class AllMeeetingsSummary
    {
        public List<WeeklyMeetingsSummary> meetingsSummary { get; set; }
    }
    public class WeeklyMeetingsSummary
    {
        public int meetingId { get; set; }
        public int? vslaId { get; set; }
        public decimal? cashExpenses { get; set; }
        public decimal? cashFines { get; set; }
        public decimal? cashFromBank { get; set; }
        public decimal? cashFromBox { get; set; }
        public decimal? cashSavedBank { get; set; }
        public decimal? cashSavedBox { get; set; }
        public decimal? cashWelfare { get; set; }

        public DateTime? dateSent { get; set; }
        public string formattedDateSent { get { return dateSent.Value.ToShortDateString(); } }

        public DateTime? meetingDate { get; set; }
        public string formattedMeetingDate { get { return meetingDate.Value.ToShortDateString(); } }
        public int? countOfMembersPresent { get; set; }
        public decimal? sumOfSavings { get; set; }
        public decimal? sumOfLoansIssued { get; set; }
        public decimal? sumOfLoanRepayments { get; set; }
        public string vslaName { get; set; }
        public string VslaCode { get; set; }

    }
}