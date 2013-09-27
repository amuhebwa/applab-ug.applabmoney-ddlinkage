using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace DigitizingDataWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DigitizingDataRestfulService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DigitizingDataRestfulService.svc or DigitizingDataRestfulService.svc.cs at the Solution Explorer and start debugging.
    public class DigitizingDataRestfulService : IDigitizingDataRestfulService
    {
        public string ActivateVslaForDigData(string vslaCode, string sourceImei)
        {
            return VslaUtils.ActivateVslaForDigitizingData(vslaCode, sourceImei);
        }

        public ActivateVslaForDdResponse ActivateVslaForDigitizingData(ActivateVslaForDdRequest request)
        {
            ActivateVslaForDdResponse response = new ActivateVslaForDdResponse();
            if (request.PhoneImei.Length > 5)
            {
                response.PassKey = request.PhoneImei.Substring(0, 5);
            }
            else
            {
                response.PassKey = "12345";
            }
            response.IsActivated = true;
            response.VslaName = request.VslaCode + "-" + request.NetworkOperatorName;

            return response;
        }

        public ActivateVslaForDdResponse ActivateVslaPhone(Stream jsonRequest)
        {
            
            ActivateVslaForDdResponse response = new ActivateVslaForDdResponse();
            ActivateVslaForDdRequest request = null;
            StreamReader reader = null;
            string jsonString = string.Empty;
            response.IsActivated = false;

            try
            {
                reader = new StreamReader(jsonRequest);
                jsonString = reader.ReadToEnd();
                request = JsonConvert.DeserializeObject<ActivateVslaForDdRequest>(jsonString);
                if (null != request)
                {
                    response.VslaName = request.VslaCode + " " + request.NetworkOperatorName;
                    response.PassKey = request.PhoneImei.Substring(0,5);
                    response.IsActivated = true;
                }
                
            }
            catch (Exception ex)
            {
                
            }

            

            return response;
        }

        public string ActivateAdminUser(string username, string securityToken)
        {
            return username + securityToken;
        }

        public string ActivateAdminUserGet(string username, string securityToken)
        {
            return username + securityToken;
        }

        public List<VslaInfo> GetVslas(string regionId)
        {
            List<VslaInfo> vslas = new List<VslaInfo>{
                new VslaInfo { VslaCode = "V001", VslaId = 13, VslaName ="ABAKISA BAKHONYANA"},
                new VslaInfo { VslaCode = "V002", VslaId = 21, VslaName ="IGANGA FARMERS"},
                new VslaInfo { VslaCode = "V003", VslaId = 24, VslaName ="BUGIRI DAIRY FARMERS ASSOCIATION"},
                new VslaInfo { VslaCode = "V004", VslaId = 35, VslaName ="AMEN A"},
                new VslaInfo { VslaCode = "V007", VslaId = 39, VslaName ="CARE UGANDA STAFF"},
                new VslaInfo { VslaCode = "V008", VslaId = 13, VslaName ="GRAMEEN STAFF"}
            };

            return vslas;
        }
    }
}
