using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using DigitizingDataDomain.Model;

namespace DigitizingDataBizLayer.Repositories
{
    public class LoanRepaymentRepo:RepositoryBase<LoanRepayment>
    {
        public LoanRepayment FindLoanRepaymentByIdEx(int meetingId, int repaymentIdEx)
        {
            var loanRepayment = (from m in SessionProxy.Query<LoanRepayment>()
                             where m.RepaymentIdEx == repaymentIdEx && m.Meeting.MeetingId == meetingId
                             select m).FirstOrDefault();
            return loanRepayment;
        }

        public List<LoanRepayment> FindMeetingLoanRepayments(int meetingId)
        {
            var loanRepayments = (from m in SessionProxy.Query<LoanRepayment>()
                              where m.Meeting.MeetingId == meetingId
                              select m).ToList();
            return loanRepayments;
        }

        public LoanRepayment findMemberMeetingRepayment(int meetingId, int memberId) {
            var repayment = (from r in SessionProxy.Query<LoanRepayment>()
                             where r.Meeting.MeetingId == meetingId && r.Member.MemberId == memberId
                             select r).FirstOrDefault();
            return repayment;
        }
    }
}
