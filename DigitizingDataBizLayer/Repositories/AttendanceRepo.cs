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
    public class AttendanceRepo : RepositoryBase<Attendance>
    {
        public Attendance FindAttendanceByIdEx(int meetingId, int attendanceIdEx)
        {
            var attendance = (from m in SessionProxy.Query<Attendance>()
                              where m.AttendanceIdEx == attendanceIdEx && m.Meeting.MeetingId == meetingId
                              select m).FirstOrDefault();
            return attendance;
        }

        public List<Attendance> FindMeetingAttendances(int meetingId)
        {
            var attendances = (from m in SessionProxy.Query<Attendance>()
                               where m.Meeting.MeetingId == meetingId
                               select m).ToList();
            return attendances;
        }

        // Total members present 
        public long totalMembersPresent()
        {
            var present = (from a in SessionProxy.Query<Attendance>()
                           where a.IsPresent == true
                           select a).Count();
            return present;
        }

        // Total members absent
        public long totalMembersAbsent()
        {
            var absent = (from a in SessionProxy.Query<Attendance>()
                          where a.IsPresent == false
                          select a).Count();
            return absent;
        }

        // Total attendance per meeting
        public int totalAttendancePerMeeting(int meetingId)
        {
            int count = (from a in SessionProxy.Query<Attendance>()
                         where a.Meeting.MeetingId == meetingId
                         && a.IsPresent == true
                         select a).Count();
            return count;

        }

    }
}
