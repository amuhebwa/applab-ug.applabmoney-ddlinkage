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
    public class VslaDdActivationRepo :RepositoryBase<VslaDdActivation>
    {
        public VslaDdActivation FindActivationByImei(string imei)
        {
            try
            {
                
                var vslaDdActivation = (from a in SessionProxy.Query<VslaDdActivation>()
                                        where a.PhoneImei01 == imei || a.PhoneImei02 == imei
                                        select a).FirstOrDefault();

                //A Refresh of the ORM session would make this procedue call a bit slow but it is necessary for now 
                SessionProxy.Refresh(vslaDdActivation);

                return vslaDdActivation;
            }
            catch
            {
                return null;
            }
        }
    }
}

/*
 * public Meeting FindMeetingByIdEx(int cycleId, int meetingIdEx)
        {
            var meeting = (from m in SessionProxy.Query<Meeting>()
                         where m.MeetingIdEx == meetingIdEx && m.VslaCycle.CycleId == cycleId
                         select m).FirstOrDefault();
            return meeting;
        }
 * */
