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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDigitizingDataRestfulService" in both code and config file together.
    [ServiceContract]
    public interface IDigitizingDataRestfulService
    {
        [OperationContract]
        [WebInvoke(Method="GET",ResponseFormat = WebMessageFormat.Json, BodyStyle=WebMessageBodyStyle.Wrapped, UriTemplate="json/{vslaCode}/{sourceImei}")]
        string ActivateVslaForDigData(string vslaCode, string sourceImei);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "vsla/activate")]
        ActivateVslaForDdResponse ActivateVslaForDigitizingData(ActivateVslaForDdRequest request);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "vslas/activate")]
        ActivateVslaForDdResponse ActivateVslaPhone(Stream jsonRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "admin/activate")]
        ActivateAdminUserResponse ActivateAdminUser(Stream jsonRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "admin/deliverkit")]
        DeliverVslaKitResponse DeliverVslaKit(Stream jsonRequest);
        
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "admin/activate/{username}/{securityToken}")]
        string ActivateAdminUserGet(string username, string securityToken);
        
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "admin/getvslas/{regionId}")]
        List<VslaInfo> GetVslas(string regionId);
    }
}
