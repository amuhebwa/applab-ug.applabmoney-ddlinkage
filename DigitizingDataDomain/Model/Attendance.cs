using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class Attendance
    {
        public virtual int AttendanceId { get; set; }
        public virtual int AttendanceIdEx { get; set; }
        public virtual Member Member { get; set; }
        public virtual Meeting Meeting { get; set; }
        public virtual bool IsPresent { get; set; }
        public virtual string Comments { get; set; }
    }
}

/*
 * ATTENDANCE_ID INT NOT NULL IDENTITY(1,1),
    ATTENDANCE_ID_EX INT NOT NULL,
    MEMBER_ID INT NOT NULL,
    MEETING_ID INT NOT NULL,
    IS_PRESENT BIT DEFAULT 0,
    COMMENTS NVARCHAR(50),
 * */

