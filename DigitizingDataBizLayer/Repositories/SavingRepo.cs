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
    public class SavingRepo : RepositoryBase<Saving>
    {
        public Saving FindSavingByIdEx(int meetingId, int savingIdEx)
        {
            var saving = (from m in SessionProxy.Query<Saving>()
                              where m.SavingIdEx == savingIdEx && m.Meeting.MeetingId == meetingId
                              select m).FirstOrDefault();
            return saving;
        }

        public List<Saving> FindMeetingSavings(int meetingId)
        {
            var savings = (from m in SessionProxy.Query<Saving>()
                               where m.Meeting.MeetingId == meetingId
                               select m).ToList();
            return savings;
        }
    }
}
