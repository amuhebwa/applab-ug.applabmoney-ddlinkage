using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitizingDataDomain.Collections;

namespace DigitizingDataDomain.Model
{
    public class Member
    {
        public virtual int MemberId { get; set; }
        public virtual int MemberIdEx { get; set; }
        public virtual int MemberNo { get; set; }
        public virtual Vsla Vsla { get; set; }
        public virtual string Surname { get; set; }
        public virtual string OtherNames { get; set; }
        public virtual string Gender { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual int CyclesCompleted { get; set; }
        public virtual string Occupation { get; set; }
        public virtual string PhoneNo { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsArchived { get; set; }        
        public virtual DateTime? DateArchived { get; set; }

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

        //Fines
        public virtual IList<Fine> FineList { get; set; }
        public virtual AggregationBindingList<Fine> Fines
        {
            get
            {
                return new AggregationBindingList<Fine>(FineList);
            }
        }
    }
}

/*
 * CREATE TABLE MEMBERS
(
    MEMBER_ID INT NOT NULL IDENTITY(1,1),
    MEMBER_ID_EX INT NOT NULL,
    VSLA_ID INT NOT NULL,
    MEMBER_NO INT NOT NULL,
    SURNAME NVARCHAR(30) NOT NULL,
    OTHER_NAMES NVARCHAR(50),
    GENDER NVARCHAR(10),
    DATE_OF_BIRTH DATETIME,
    CYCLES_COMPLETED INT,
    OCCUPATION NVARCHAR(50),
    PHONE_NO NVARCHAR(15),
    IS_ACTIVE BIT DEFAULT 0,
    IS_ARCHIVED BIT DEFAULT 0,
    DATE_ARCHIVED DATETIME,

	CONSTRAINT PK_MEMBERS PRIMARY KEY(MEMBER_ID),
	CONSTRAINT FK_MEMBERS_VSLAS FOREIGN KEY(VSLA_ID) REFERENCES VSLAS(VSLA_ID),
	CONSTRAINT AK_VSLA_MEMBER_NO UNIQUE(VSLA_ID,MEMBER_NO)
);
 */
