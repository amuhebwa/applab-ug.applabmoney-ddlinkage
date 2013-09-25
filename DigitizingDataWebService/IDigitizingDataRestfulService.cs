using System;
using System.Collections.Generic;
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
        string ActivateVslaForDigitizingData(string vslaCode, string sourceImei);

        
    }
}
