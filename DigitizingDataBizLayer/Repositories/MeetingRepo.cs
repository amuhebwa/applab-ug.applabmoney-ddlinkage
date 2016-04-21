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

        // Get weekly meetings summary
        public List<Meeting> findWeeklyMeetings(DateTime startDate)
        {
            var allMeetings = (from m in SessionProxy.Query<Meeting>()
                               where m.DateSent >= startDate
                               select m).ToList();
            return allMeetings;
        }

        // Find meeting by meeting id
        public List<Meeting> findMeetingByVslaId(int vslaId)
        {
            var meeting = (from m in SessionProxy.Query<Meeting>()
                           where m.VslaCycle.Vsla.VslaId == vslaId
                           select m).ToList();
            return meeting;
        }

        // Get meeting information by Id
        public Meeting findMeetingByMeetingId(int id)
        {
            var meeting = (from m in SessionProxy.Query<Meeting>()
                           where m.MeetingId == id
                           select m).FirstOrDefault();
            return meeting;
        }

        // Last date that a group submitted group information
        public string lastDateOfSubmission(int vslaId)
        {
            var y = (from m in SessionProxy.Query<Meeting>()
                     where m.VslaCycle.Vsla.VslaId == vslaId
                     orderby m.DateSent descending
                     select m.DateSent).FirstOrDefault();
            //var x = (from m in SessionProxy.Query<Meeting>()
            //         where m.VslaCycle.Vsla.VslaId == vslaId
            //         select m.DateSent).OrderByDescending(t => t.Value).FirstOrDefault();
            return Convert.ToString(y);
        }
    }
}
