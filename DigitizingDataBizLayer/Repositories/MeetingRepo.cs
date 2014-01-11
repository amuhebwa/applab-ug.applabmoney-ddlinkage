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
    }
}
