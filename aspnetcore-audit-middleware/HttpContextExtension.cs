using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

namespace Com.RFranco.AsptNetCore.Audit
{
    public static class HttpContextAuditExtension
    {
        private static string[] REQUEST_HEADERS_TO_IGNORE = new string[]
        {
            "A-IM",
            "Accept",
            "Accept-Charset",
            "Accept-Datetime",
            "Accept-Encoding",
            "Accept-Language",
            "Access-Control-Request-Method,",
            "Access-Control-Request-Headers",
            //"Authorization",
            "Cache-Control",
            "Connection",
            "Content-Length",
            "Content-MD5",
            "Content-Type",
            "Cookie",
            "Date",
            "Expect",
            "Forwarded",
            "From",
            "Host",
            "HTTP2-Settings",
            "If-Match",
            "If-Modified-Since",
            "If-None-Match",
            "If-Range",
            "If-Unmodified-Since",
            "Max-Forwards",
            "Origin",
            "Pragma",
            "Proxy-Authorization",
            "Range",
            "Referer",
            "TE",
            "Trailer",
            "Transfer-Encoding",
            "User-Agent",
            "Upgrade",
            "Via",
            "Warning",
            "Upgrade-Insecure-Requests",
            "X-Requested-With",
            "DNT",
            //"X-Forwarded-For",
            "X-Forwarded-Host",
            "X-Forwarded-Proto",
            "Front-End-Https",
            "X-Http-Method-Override",
            "X-ATT-DeviceId",
            "X-Wap-Profile",
            "Proxy-Connection",
            "X-UIDH",
            "X-Csrf-Token",
            //"X-Request-ID",
            //"X-Correlation-ID",
            "Save-Data"
        };

        private static readonly string[] RESPONSE_HEADERS_TO_IGNORE = new string[]
        {
            "Access-Control-Allow-Origin",
            "Access-Control-Allow-Credentials",
            "Access-Control-Expose-Headers",
            "Access-Control-Max-Age",
            "Access-Control-Allow-Methods",
            "Access-Control-Allow-Headers",
            "Accept-Patch",
            "Accept-Ranges",
            "Age",
            "Allow",
            "Alt-Svc",
            "Cache-Control",
            "Connection",
            "Content-Disposition",
            "Content-Encoding",
            "Content-Language",
            "Content-Length",
            "Content-Location",
            "Content-MD5",
            "Content-Range",
            "Content-Type",
            "Date",
            "Delta-Base",
            "ETag",
            "Expires",
            "IM",
            "Last-Modified",
            "Link",
            "Location",
            "P3P",
            "Pragma",
            "Proxy-Authenticate",
            "Public-Key-Pins",
            "Retry-After",
            "Server",
            //"Set-Cookie",
            "Strict-Transport-Security",
            "Trailer",
            "Transfer-Encoding",
            "Tk",
            "Upgrade",
            "Vary",
            "Via",
            "Warning",
            "WWW-Authenticate",
            "X-Frame-Options",
            "Content-Security-Policy,",
            "X-Content-Security-Policy,",
            "X-WebKit-CSP",
            "Refresh",
            "Status",
            "Timing-Allow-Origin",
            "X-Content-Duration",
            "X-Content-Type-Options",
            "X-Powered-By",
            //"X-Request-ID,",
            //"X-Correlation-ID",
            "X-UA-Compatible",
            "X-XSS-Protection"
        };
        
        public static async Task<AuditInfo> ExtractAuditInfo(this HttpContext context)
        {
            return new AuditInfo{Request = await context.AuditRequest(), Response = await context.AuditResponse()};
        }
        private static async Task<AuditRequest> AuditRequest(this HttpContext context)
        {
            var requestBodyStream = new MemoryStream();
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            await context.Request.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            context.Request.Body = requestBodyStream;

            return new AuditRequest
            {
                Protocol = context.Request.Protocol,
                Method = context.Request.Method,
                Url = UriHelper.GetDisplayUrl(context.Request),
                RequestBodyText = MinifyJson(requestBodyText),
                RequestHeaders = GetHeadersToString(context.Request.Headers.Where(header => !REQUEST_HEADERS_TO_IGNORE.Contains(header.Key)))
            };
        }

        private static async Task<AuditResponse> AuditResponse(this HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            return new AuditResponse
            {
                ResponseHeaders = GetHeadersToString(context.Response.Headers.Where(header => !RESPONSE_HEADERS_TO_IGNORE.Contains(header.Key))),
                ResponseText = MinifyJson(responseBodyText),
                StatusCode = context.Response.StatusCode.ToString()
            };
        }
        
        /// Get Headers in string format separated by ;
        private static string GetHeadersToString(IEnumerable<KeyValuePair<string,StringValues>> headers)
        {
            return String.Join(";", headers.Select(kv => $"{kv.Key}={kv.Value}"));
        }
        private static string MinifyJson(string json)
        {
            return Regex.Replace(json, @"\t|\n|\r|\s(?=([^""]*""[^""]*"")*[^""]*$)", string.Empty);
        }
    }  
}