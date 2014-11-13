using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitizingDataDomain.Collections;

namespace DigitizingDataDomain.Model
{
    public class Meeting
    {
        public virtual int MeetingId { get; set; }
        public virtual int MeetingIdEx { get; set; }
        public virtual VslaCycle VslaCycle { get; set; }
        public virtual DateTime? MeetingDate { get; set; }
        public virtual bool IsDataSent { get; set; }
        public virtual DateTime? DateSent { get; set; }
        public virtual bool IsCurrent { get; set; }
        public virtual double CashFromBox { get; set; }
        public virtual double CashFromBank { get; set; }
        public virtual double CashFines { get; set; }
        public virtual double CashWelfare { get; set; }
        public virtual double CashExpenses { get; set; }
        public virtual double CashSavedBox { get; set; }
        public virtual double CashSavedBank { get; set; }

        //Summary Fields: Denormalized
        public virtual double SumOfSavings { get; set; }
        public virtual double SumOfLoanIssues { get; set; }
        public virtual double SumOfLoanRepayments { get; set; }
        public virtual int CountOfMembersPresent { get; set; }

        //Loan Repayments
        public virtual IList<LoanRepayment> LoanRepaymentList { get; set; }
        public virtual AggregationBindingList<LoanRepayment> LoanRepayments
        {
            get
            {
                return new AggregationBindingList<LoanRepayment>(LoanRepaymentList);
            }
        }

        //Loan Issues
        public virtual IList<LoanIssue> LoanIssueList { get; set; }
        public virtual AggregationBindingList<LoanIssue> LoanIssues
        {
            get
            {
                return new AggregationBindingList<LoanIssue>(LoanIssueList);
            }
        }

        //Attendances
        public virtual IList<Attendance> AttendanceList { get; set; }
        public virtual AggregationBindingList<Attendance> Attendances
        {
            get
            {
                return new AggregationBindingList<Attendance>(AttendanceList);
            }
        }

        //Savings
        public virtual IList<Saving> SavingList { get; set; }
        public virtual AggregationBindingList<Saving> Savings
        {
            get
            {
                return new AggregationBindingList<Saving>(SavingList);
            }
        }

        //Fines Issued
        public virtual IList<Fine> FineIssuedList { get; set; }
        public virtual AggregationBindingList<Fine> FinesIssued
        {
            get
            {
                return new AggregationBindingList<Fine>(FineIssuedList);
            }
        }

        //Fines Paid
        public virtual IList<Fine> FinePaidList { get; set; }
        public virtual AggregationBindingList<Fine> FinesPaid
        {
            get
            {
                return new AggregationBindingList<Fine>(FinePaidList);
            }
        }

    }
}

/*
 * MEETING_ID INT NOT NULL IDENTITY(1,1),
    MEETING_ID_EX INT NOT NULL,
    CYCLE_ID INT NOT NULL,
    MEETING_DATE DATETIME,
    IS_DATA_SENT BIT DEFAULT 0,
    DATE_SENT DATETIME,
    IS_CURRENT BIT DEFAULT 0,
    CASH_FROM_BOX DECIMAL(18,2) DEFAULT 0,
    CASH_FROM_BANK DECIMAL(18,2) DEFAULT 0,
    CASH_FINES DECIMAL(18,2) DEFAULT 0,
    CASH_WELFARE DECIMAL(18,2) DEFAULT 0,
    CASH_EXPENSES DECIMAL(18,2) DEFAULT 0,
    CASH_SAVED_BOX DECIMAL(18,2) DEFAULT 0,
    CASH_SAVED_BANK DECIMAL(18,2) DEFAULT 0,
 * */
