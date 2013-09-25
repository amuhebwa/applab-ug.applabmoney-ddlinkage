using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DigitizingDataWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class DigitizingDataService : IDigitizingDataService
    {
        public string RegisterVsla(string vslaCode, string vslaName, string locationGps, string sourceImei, string vslaPhoneImei)
        {
            throw new NotImplementedException();
        }

        public string GeneratePassKey(string vslaCode, string sourceImei)
        {
            int key = VslaUtils.GeneratePassKey();
            return key.ToString("00000");
        }

        public string ChangePassKey(string vslaCode, string sourceImei, string oldPassKey, string newPassKey)
        {
            int key = VslaUtils.GeneratePassKey();
            return key.ToString("00000");
        }

        public string ActivateDigData(string vslaCode, string sourceImei, string passKey)
        {
            return VslaUtils.ActivateVslaForDigitizingData(vslaCode, sourceImei);
        }

        public bool SubmitMeetingData(string vslaCode, string sourceImei, string jsonData)
        {
            throw new NotImplementedException();
        }

        public string GetMeetingData(string vslaCode, string sourceImei, string meetingDate)
        {
            throw new NotImplementedException();
        }
    }
}
