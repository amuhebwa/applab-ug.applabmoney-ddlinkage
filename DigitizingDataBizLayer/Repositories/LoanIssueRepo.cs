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
    public class LoanIssueRepo:RepositoryBase<LoanIssue>
    {
        public LoanIssue FindLoanIssueByIdEx(int meetingId, int loanIdEx)
        {
            var loanIssue = (from m in SessionProxy.Query<LoanIssue>()
                              where m.LoanIdEx == loanIdEx && m.Meeting.MeetingId == meetingId
                              select m).FirstOrDefault();
            return loanIssue;
        }

        public LoanIssue FindLoanIssueByMemberAndLoadIdEx(int memberId, int loanIdEx)
        {
            //TODO: I am not sure whether to bring in the CycleId to ensure I grab the right Loan
            //I could pass the CycleId as a parameter but is it really necessary?
            var loanIssue = (from m in SessionProxy.Query<LoanIssue>()
                             where m.LoanIdEx == loanIdEx && m.Member.MemberId == memberId
                             select m).FirstOrDefault();
            return loanIssue;
        }

        public List<LoanIssue> FindMeetingLoanIssues(int meetingId)
        {
            var loanIssues = (from m in SessionProxy.Query<LoanIssue>()
                               where m.Meeting.MeetingId == meetingId
                               select m).ToList();
            return loanIssues;
        }
    }
}
