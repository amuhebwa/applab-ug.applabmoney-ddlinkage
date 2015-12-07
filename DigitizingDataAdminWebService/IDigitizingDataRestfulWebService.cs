using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;

namespace DigitizingDataAdminWebService
{
    [ServiceContract]
    public interface IDigitizingDataRestfulWebService
    {
        // Login and validate the Technical Trainer username & pass key
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "validateTrainer/{username}/{passkey}")]
        TechnicalTrainer validateTrainer(string username, string passkey);

        // search for vsla
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "searchVsla/{vslaName}")]
        List<VslaDetails> searchVsla(string vslaName);

        // get all information for one vsla
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "vslaInformation/{vslaId}")]
        VslaDetails vslaInformation(string vslaId);

        // add a new vsla
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "addNewVsla")]
        OperationResult addNewVsla(Stream jsonStream);

        // edit an existing vsla
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "editVsla")]
        OperationResult editVsla(Stream jsonStream);
    }

}
