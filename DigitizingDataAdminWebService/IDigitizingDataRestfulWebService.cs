using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DigitizingDataAdminWebService
{
    [ServiceContract]
    public interface IDigitizingDataRestfulWebService
    {
        /**
         * Login for a given CBT
         */
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "loginCBT/{Username}/{PassKey}")]
        string loginCBT(String Username, String PassKey);


        /**
         * Search for a given VSLA
         */
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "searchForVsla/{VslaName}")]
        List<VslaDetails> searchForVsla(string VslaName);



        /**
         * Get all registered users
         */
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getRegisteredUsers")]
        List<UsersDetails> getRegisteredUsers();

        /**
         * Get all VSLAs
         */
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getAllVslas")]
        List<VslaDetails> getAllVslas();

        /**
         * Get VSLAs attached to a particular CBT
         */
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getVslaForParticularCBT/{id}")]
        List<VslaDetails> getVslaForParticularCBT(string id);
        /**
         * Get all information for a single VSLA
         */
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getSingleVslaDetails/{VslaId}")]
        VslaDetails getSingleVslaDetails(string VslaId);
    }

}
