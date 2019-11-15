using System.Collections.Generic;

namespace Com.RFranco.AsptNetCore.Audit
{
    /// AuditInfo object
    public class AuditInfo
    {

        public AuditRequest Request { get; set; }

        public AuditResponse Response { get; set; }


        /// Override ToString
        public override string ToString()
        {
            return $"{{\"Request\" : \"{Request} ,\"Response\":\"{Response}\"}}";
        }

        public IDictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string> {
                {"Protocol", Request.Protocol },
                {"Method", Request.Method },
                {"Url", Request.Url },
                {"RequestHeaders", Request.RequestHeaders },
                {"RequestBody", Request.RequestBodyText },
                {"ResponseHeaders", Response.ResponseHeaders },
                {"ResponseBody", Response.ResponseText },
                {"StatusCode", Response.StatusCode }
            };
        }
    }

    public class AuditRequest
    {
        /// Protocol used
        public string Protocol;
        /// HTTP Method used
        public string Method;
        /// URL (including queryparameters)
        public string Url;
        /// Request body text-format
        public string RequestBodyText;
        /// Request headers
        public string RequestHeaders;

        /// Override ToString
        public override string ToString()
        {
            return $"{{\"Path\" : \"{Protocol} {Method} {Url}\", \"RequestBody\" : \"{RequestBodyText}\"}}";
        }
    }

    public class AuditResponse
    {
        /// Response Headers
        public string ResponseHeaders;
        /// Response body in text-formtat
        public string ResponseText;
        ///HTTP Status code of the response
        public string StatusCode;

        /// Override ToString
        public override string ToString()
        {
            return $"{{\"StatusCode\":{StatusCode} , \"ResponseBody\":\"{ResponseText}\"}}";
        }
    }
}