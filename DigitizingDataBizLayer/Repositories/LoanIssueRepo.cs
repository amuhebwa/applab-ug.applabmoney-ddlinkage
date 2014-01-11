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

        public List<LoanIssue> FindMeetingLoanIssues(int meetingId)
        {
            var loanIssues = (from m in SessionProxy.Query<LoanIssue>()
                               where m.Meeting.MeetingId == meetingId
                               select m).ToList();
            return loanIssues;
        }
    }
}
