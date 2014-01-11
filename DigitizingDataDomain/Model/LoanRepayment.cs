using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class LoanRepayment
    {
        public virtual int RepaymentId { get; set; }
        public virtual int RepaymentIdEx { get; set; }
        public virtual LoanIssue LoanIssue { get; set; }
        public virtual Member Member { get; set; }
        public virtual Meeting Meeting { get; set; }
        public virtual double Amount { get; set; }
        public virtual double BalanceBefore { get; set; }
        public virtual double BalanceAfter { get; set; }
        public virtual double InterestAmount { get; set; }
        public virtual double RolloverAmount { get; set; }
        public virtual DateTime? DateDue { get; set; }
        public virtual string Comments { get; set; }
    }
}

/*
 * REPAYMENT_ID INT NOT NULL IDENTITY(1,1),
    REPAYMENT_ID_EX INT NOT NULL,
    LOAN_ID INT,
    MEMBER_ID INT NOT NULL,
    MEETING_ID INT NOT NULL,
    AMOUNT DECIMAL(18,2) NOT NULL,
    BALANCE_BEFORE DECIMAL(18,2) DEFAULT 0,
    BALANCE_AFTER DECIMAL(18,2) DEFAULT 0,
    INTEREST_AMOUNT DECIMAL(18,2) DEFAULT 0,
    ROLLOVER_AMOUNT DECIMAL(18,2) DEFAULT 0,
	DATE_DUE DATETIME,
    COMMENTS NVARCHAR(50),
 * */
