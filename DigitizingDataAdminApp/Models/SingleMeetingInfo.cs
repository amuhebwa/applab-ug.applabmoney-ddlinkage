using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class AllSingleMeetingInfo
    {
        public List<SingleMeetingInfo> allMeetingData { get; set; }

        public DateTime? meetingDate { get; set; }

        public int vslaId { get; set; }

    }
    public class SingleMeetingInfo
    {
        public int Id { get; set; }

        public string isPresent { get; set; }

        public int memberId { get; set; }

        public string memberName { get; set; }

        public long amountSaved { get; set; }
        public string fAmountSaved { get { return amountSaved.ToString("#,##0"); } }

        public long finedAmount { get; set; }
        public string fFinedAmount { get { return finedAmount.ToString("#,##0"); } }

        public long principleAmount { get; set; }
        public string fPrincipleAmount { get { return principleAmount.ToString("#,##0"); } }

        public long loanRepaymentAmount { get; set; }
        public string fLoanRepayment { get { return loanRepaymentAmount.ToString("#,##0"); } }

        public long remainingBalanceOnLoan { get; set; }
        public string fRemainingBalance { get { return remainingBalanceOnLoan.ToString("#,##0"); } }

        public int loanNumber { get; set; }
    }
}