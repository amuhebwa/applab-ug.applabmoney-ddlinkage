using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;


namespace DigitizingDataWebService
{
    [ServiceContract]
    public interface IDigitizingDataRestfulService
    {
        /**
         * Activate a VSLA
         */ 
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "vslas/activate")]
        ActivateVslaForDdResponse ActivateVslaPhone(Stream jsonRequest);

        /**
         * Submit VSLA Data
         */ 
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "vslas/submitdata")]
        SubmitVslaDataResponse SubmitVslaData(Stream jsonRequest);

        /**
         * Activate a CBT
         */ 
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "admin/activate")]
        ActivateAdminUserResponse ActivateAdminUser(Stream jsonRequest);

        /**
         * Capture KIT Delivery Info by CBT
         */ 
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "admin/deliverkit")]
        DeliverVslaKitResponse DeliverVslaKit(Stream jsonRequest);

        /**
         * Capture GPS of the VSLA by CBT
         */ 
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "admin/capturegps")]
        CaptureGpsLocationResponse CaptureGpsLocation(Stream jsonRequest);

        /**
         * Activate an Admin User - CBT
         */ 
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "admin/activate/{username}/{securityToken}")]
        string ActivateAdminUserGet(string username, string securityToken);

        /**
         * GET ALL VSLAS in a particular REGION - by CBT
         */ 
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "admin/getvslas/{regionId}")]
        List<VslaInfo> GetVslas(string regionId);

        /**
         * Used by Internal API Admin to force processing of submissions
         * This is an internal procedure that will be used to reprocess submissions
         */
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "internal/reprocesssubmissions/{username}/{securityToken}")]
        SubmitVslaDataResponse ReProcessSubmissions(string username, string securityToken);        

        /**
         * Used by Internal API Admin to Check on the Health of the API
         */
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "internal/healthstats/{username}/{securityToken}")]
        HealthStatsResponse GetHealthStats(string username, string securityToken);
    }
}
