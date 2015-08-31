using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Channels;


namespace DigitizingDataAdminWebService
{
    public class RawContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            if (contentType.Contains("text/xml") || contentType.Contains("application/json"))
            {
                return WebContentFormat.Raw;
            }
            else
            {
                return WebContentFormat.Default;
            }
        }
    }
}