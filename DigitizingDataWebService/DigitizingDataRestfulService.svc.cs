using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DigitizingDataBizLayer.Repositories;
using DigitizingDataDomain.Model;

namespace DigitizingDataWebService
{   
    public class DigitizingDataRestfulService : IDigitizingDataRestfulService
    {        
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
                    response.VslaName = request.VslaCode + "-ACTIVATED";
                    response.PassKey = request.PassKey.Trim();
                    response.IsActivated = true;

                    VslaRepo vslaRepo = new VslaRepo();
                    var vsla = vslaRepo.FindVslaByCode(request.VslaCode);
                    if(vsla != null)
                    {                        
                        response.VslaName = vsla.VslaName;
                        response.PassKey = request.PassKey.Trim();
                        response.IsActivated = true;
                    }
                }                
            }
            catch (Exception ex)
            {
                
            }
            return response;
        }

        public ActivateAdminUserResponse ActivateAdminUser(Stream jsonRequest)
        {
            ActivateAdminUserResponse response = new ActivateAdminUserResponse();
            ActivateAdminUserRequest request = null;
            StreamReader reader = null;
            string jsonString = string.Empty;
            response.ActivationStatus = -1;

            try
            {
                List<VslaInfo> vslas = new List<VslaInfo> {
                new VslaInfo { VslaCode = "V001", VslaId = 13, VslaName ="ABAKISA BAKHONYANA"},
                new VslaInfo { VslaCode = "V002", VslaId = 21, VslaName ="IGANGA FARMERS"},
                new VslaInfo { VslaCode = "V003", VslaId = 24, VslaName ="BUGIRI DAIRY FARMERS ASSOCIATION"},
                new VslaInfo { VslaCode = "V004", VslaId = 35, VslaName ="AMEN A"},
                new VslaInfo { VslaCode = "V007", VslaId = 39, VslaName ="CARE UGANDA STAFF"},
                new VslaInfo { VslaCode = "V008", VslaId = 13, VslaName ="GRAMEEN STAFF"}
            };

                reader = new StreamReader(jsonRequest);
                jsonString = reader.ReadToEnd();
                request = JsonConvert.DeserializeObject<ActivateAdminUserRequest>(jsonString);
                if (null != request)
                {

                    response.VslaList = vslas;
                    response.ActivationStatus = 0;
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

        public DeliverVslaKitResponse DeliverVslaKit(Stream jsonRequest)
        {

            DeliverVslaKitResponse response = new DeliverVslaKitResponse();
            DeliverVslaKitRequest request = null;
            StreamReader reader = null;
            string jsonString = string.Empty;
            response.DeliveryStatus = -1;

            try
            {
                reader = new StreamReader(jsonRequest);
                jsonString = reader.ReadToEnd();
                request = JsonConvert.DeserializeObject<DeliverVslaKitRequest>(jsonString);
                if (null != request && request.VslaPhoneImei.Trim().Length > 0 && request.PhoneImei.Trim().Length > 0)
                {
                    response.DeliveryStatus = Convert.ToInt32(request.VslaPhoneImei.Substring(0, 3) + request.PhoneImei.Substring(0, 3));
                }

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public CaptureGpsLocationResponse CaptureGpsLocation(Stream jsonRequest)
        {

            CaptureGpsLocationResponse response = new CaptureGpsLocationResponse();
            CaptureGpsLocationRequest request = null;
            StreamReader reader = null;
            string jsonString = string.Empty;
            response.StatusCode = -1;

            try
            {
                reader = new StreamReader(jsonRequest);
                jsonString = reader.ReadToEnd();
                request = JsonConvert.DeserializeObject<CaptureGpsLocationRequest>(jsonString);
                if (null != request && request.GpsLocation.Trim().Length > 0 && request.PhoneImei.Trim().Length > 0)
                {
                    response.StatusCode = 0;
                }

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public SubmitVslaDataResponse SubmitVslaData(Stream jsonRequest)
        {
            SubmitVslaDataResponse response = new SubmitVslaDataResponse();
            StreamReader reader = null;
            string jsonString = string.Empty;
            response.StatusCode = -1;
            DataSubmission dataSubmission = null;
            DataSubmissionRepo repo = null;

            try
            {
                reader = new StreamReader(jsonRequest);
                jsonString = reader.ReadToEnd();
                dynamic obj = JObject.Parse(jsonString);
                var headerInfo = obj.HeaderInfo;                

                if (null != headerInfo)
                {
                    dataSubmission = new DataSubmission();
                    dataSubmission.SourceVslaCode = headerInfo.VslaCode;
                    dataSubmission.SourcePhoneImei = headerInfo.PhoneImei;
                    dataSubmission.SourceNetworkOperator = headerInfo.SourceNetworkOperator;
                    dataSubmission.SourceNetworkType = headerInfo.SourceNetworkType;
                    dataSubmission.SubmissionTimestamp = DateTime.Now;
                    dataSubmission.Data = jsonString;
                }

                if(dataSubmission != null)
                {
                    repo = new DataSubmissionRepo();
                    //repo.FindAll();

                    bool retVal = repo.Insert(dataSubmission);
                    if(retVal)
                    {
                        response.StatusCode = 0;
                    }
                }
            }
            catch
            {
                response.StatusCode = -99;
            }

            return response;
        }

    }
}
