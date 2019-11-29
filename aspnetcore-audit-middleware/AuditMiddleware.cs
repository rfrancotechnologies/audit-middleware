using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Com.RFranco.AsptNetCore.Audit
{

    /// Middleware to audit request / responses
    public class AuditMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ILogger Logger;
        private AuditMiddlewareOptions Options;

        ///
        public AuditMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, AuditMiddlewareOptions options)
        {
            Next = next;
            Logger = loggerFactory.CreateLogger("AuditMiddleware");
            Options = options;
        }

        ///
        public async Task Invoke(HttpContext context)
        {
            if (!MustBeAudited(context)) await Next(context);
            else
            {
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;
                    try
                    {
                        await Next(context);
                    }
                    finally
                    {
                        AuditInfo auditInfo = await context.ExtractAuditInfo();
                        await responseBody.CopyToAsync(originalBodyStream);
                        using (Logger.BeginScope(auditInfo.ToDictionary()))
                        {
                            Logger.LogInformation($"{auditInfo}");
                        }
                    }
                }
            }
        }
        private bool MustBeAudited(HttpContext context)
        {

            return context.Request.Method != "OPTIONS"
                && Options.ExcludedPaths.Where(path => context.Request.GetDisplayUrl().IndexOf(path) != -1).Count() == 0;
        }
    }


    public static class AuditMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuditMiddleware(this IApplicationBuilder builder, AuditMiddlewareOptions options = null)
        {
            return builder.UseMiddleware<AuditMiddleware>(options ?? new AuditMiddlewareOptions());
        }
    }
}
