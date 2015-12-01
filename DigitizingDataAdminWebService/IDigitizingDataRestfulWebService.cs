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
        TechnicalTrainerCtx validateTrainer(string username, string passkey);

        // search for vsla
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "searchVsla/{vslaName}")]
        List<VslaDetails> searchVsla(string vslaName);

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "vslaInformation/{vslaId}")]
        VslaDetails vslaInformation(string vslaId);













        /**
         * Get VSLAs attached to a particular CBT
         */
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getVslaForParticularCBT/{id}")]
        List<VslaDetails> getVslaForParticularCBT(string id);
       

        // Create a new VSLA and add it into the database
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "createNewVsla")]
        RegistrationResult createNewVsla(Stream jsonStream);

        // Edit an exisiting VSLA
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "editExistingVsla")]
        RegistrationResult editExistingVsla(Stream jsonStreamObject);
    }

}
