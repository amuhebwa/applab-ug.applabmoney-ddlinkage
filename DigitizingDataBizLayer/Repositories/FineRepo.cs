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
    public class FineRepo : RepositoryBase<Fine>
    {

        public object FindFineByIdEx(int issuedMeetingId, int fineIdEx)
        {
            var fine = (from m in SessionProxy.Query<Fine>()
                             where m.FineIdEx == fineIdEx && m.IssuedInMeeting.MeetingId == issuedMeetingId
                             select m).FirstOrDefault();
            return fine;
        }
    }
}
