using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DigitizingDataWebService
{
    [ServiceContract]
    public interface IDigitizingDataService
    {
        [OperationContract]
        string RegisterVsla(string vslaCode, string vslaName, string locationGps, string sourceImei, string vslaPhoneImei);

        [OperationContract]
        string GeneratePassKey(string vslaCode, string sourceImei);

        [OperationContract]
        string ChangePassKey(string vslaCode, string sourceImei, string oldPassKey, string newPassKey);

        [OperationContract]
        string ActivateDigData(string vslaCode, string sourceImei, string passKey);

        [OperationContract]
        bool SubmitMeetingData(string vslaCode, string sourceImei, string jsonData);

        [OperationContract]
        string GetMeetingData(string vslaCode, string sourceImei, string meetingDate);
    }

}
