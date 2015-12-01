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















        /**
         * Login for a given CBT
         */
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "technicalTrainerLogin/{Username}/{PassKey}")]
        //CBTLoginDetails technicalTrainerLogin(String Username, String PassKey);

        /**
         * Search for a given VSLA
         */
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "searchForVsla/{VslaName}")]
        //List<VslaDetails> searchForVsla(string VslaName);

        /**
         * Get all registered users
         */
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getRegisteredUsers")]
        //List<UsersDetails> getRegisteredUsers();

        /**
         * Get all VSLAs
         */
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getAllVslas")]
        //List<VslaDetails> getAllVslas();

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
