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
    public class AttendanceRepo:RepositoryBase<Attendance>
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
    }
}
