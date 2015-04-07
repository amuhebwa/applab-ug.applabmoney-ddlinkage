using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class AllSingleMeetingProcedures
    {
        public List<SingleMeetingProcedures> allMeetingData { get; set; }
        public DateTime? meetingDate { get; set; }
        public int vslaId { get; set; }
    }
    public class SingleMeetingProcedures
    {
        public int Id { get; set; }
        public string isPresent { get; set; }
        public int memberId { get; set; }
        public string memberName { get; set; }
        public long amountSaved { get; set; }
        public long finedAmount { get; set; }
        public long principleAmount { get; set; }
        public long loanRepaymentAmount { get; set; }
        public long remainingBalanceOnLoan { get; set; }
        public int loanNumber { get; set; }
    }
}