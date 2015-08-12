using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitizingDataDomain.Collections;

namespace DigitizingDataDomain.Model
{
    public class LoanIssue
    {
        public virtual int LoanId { get; set; }
        public virtual int LoanIdEx { get; set; }
        public virtual Member Member { get; set; }
        public virtual Meeting Meeting { get; set; }
        public virtual int LoanNo { get; set; }
        public virtual double PrincipalAmount { get; set; }
        public virtual double InterestAmount { get; set; }
        public virtual double Balance { get; set; }
        public virtual double TotalRepaid { get; set; }
        public virtual DateTime? DateDue { get; set; }
        public virtual bool IsCleared { get; set; }
        public virtual DateTime? DateCleared { get; set; }
        public virtual bool IsDefaulted { get; set; }
        public virtual string Comments { get; set; }
        public virtual bool IsWrittenOff { get; set; }

        //Loan Repayments
        public virtual IList<LoanRepayment> LoanRepaymentList { get; set; }
        public virtual AggregationBindingList<LoanRepayment> LoanRepayments
        {
            get
            {
                return new AggregationBindingList<LoanRepayment>(LoanRepaymentList);
            }
        }
    }
}

/*
 * LOAN_ID INT NOT NULL IDENTITY(1,1),
    LOAN_ID_EX INT NOT NULL,
    MEMBER_ID INT NOT NULL,
    MEETING_ID INT NOT NULL,
    LOAN_NO INT NOT NULL,
    AMOUNT DECIMAL(18,2) NOT NULL,
    BALANCE DECIMAL(18,2) DEFAULT 0,
    TOTAL_REPAID DECIMAL(18,2) DEFAULT 0,
	DATE_DUE DATETIME,
    IS_CLEARED BIT DEFAULT 0,
    DATE_CLEARED DATETIME,
    IS_DEFAULTED BIT DEFAULT 0,
    COMMENTS NVARCHAR(50),
 * */
