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
         * Get all registered users
         */
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getRegisteredUsers")]
        List<UsersDetails> getRegisteredUsers();

        /**
         * Get all VSLAs
         */
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getAllVslas")]
        List<VslaDetails> getAllVslas();
    }

}
