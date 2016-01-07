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
    public class MeetingRepo : RepositoryBase<Meeting>
    {
        public Meeting FindMeetingByIdEx(int cycleId, int meetingIdEx)
        {
            var meeting = (from m in SessionProxy.Query<Meeting>()
                           where m.MeetingIdEx == meetingIdEx && m.VslaCycle.CycleId == cycleId
                           select m).FirstOrDefault();
            return meeting;
        }

        public Meeting FindMeetingByIdEx(string vslaCode, int meetingIdEx)
        {
            var meeting = (from m in SessionProxy.Query<Meeting>()
                           where m.MeetingIdEx == meetingIdEx && m.VslaCycle.Vsla.VslaCode.ToUpper() == vslaCode.ToUpper()
                           select m).FirstOrDefault();
            return meeting;
        }

        public Meeting FindMeetingByDate(int cycleId, DateTime? meetingDate)
        {
            var meeting = (from m in SessionProxy.Query<Meeting>()
                           where m.MeetingDate.GetValueOrDefault().Equals(meetingDate.GetValueOrDefault()) && m.VslaCycle.CycleId == cycleId
                           select m).FirstOrDefault();
            return meeting;
        }

        // Total Savings
        public double findTotalSavings()
        {
            var savings = (from m in SessionProxy.Query<Meeting>()
                           select m).Sum(x => (double)x.SumOfSavings);
            return savings;
        }

        // Total loans
        public double findTotalLoans()
        {
            var loans = (from m in SessionProxy.Query<Meeting>()
                         select m).Sum(x => (double)x.SumOfLoanIssues);
            return loans;
        }

        // Total Loan Repayment
        public double findTotalLoanRepayment()
        {
            var replayments = (from m in SessionProxy.Query<Meeting>()
                               select m).Sum(x => (double)x.SumOfLoanRepayments);
            return replayments;
        }

        // Total meetings
        public long totalMeetingsHeld()
        {
            var totalMeetings = (from m in SessionProxy.Query<Meeting>()
                                 select m).Count();
            return totalMeetings;
        }
    }
}
