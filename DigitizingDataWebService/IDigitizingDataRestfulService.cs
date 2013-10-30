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
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "vslas/activate")]
        ActivateVslaForDdResponse ActivateVslaPhone(Stream jsonRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "vslas/submitdata")]
        SubmitVslaDataResponse SubmitVslaData(Stream jsonRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "admin/activate")]
        ActivateAdminUserResponse ActivateAdminUser(Stream jsonRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "admin/deliverkit")]
        DeliverVslaKitResponse DeliverVslaKit(Stream jsonRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "admin/capturegps")]
        CaptureGpsLocationResponse CaptureGpsLocation(Stream jsonRequest);
        
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "admin/activate/{username}/{securityToken}")]
        string ActivateAdminUserGet(string username, string securityToken);
        
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "admin/getvslas/{regionId}")]
        List<VslaInfo> GetVslas(string regionId);
    }
}
