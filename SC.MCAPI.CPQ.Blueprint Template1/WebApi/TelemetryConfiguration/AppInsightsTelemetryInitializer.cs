using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.CommonSchema.Services.Logging;
using Microsoft.VisualBasic;
using System.Linq;

namespace WebApi.TelemetryConfiguration
{
    public class AppInsightsTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppInsightsTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var props = telemetry.Context.GlobalProperties;

                if (telemetry.Context.User.AuthenticatedUserId == null && !string.IsNullOrEmpty(context.User.Identity?.Name))
                {
                    telemetry.Context.User.AuthenticatedUserId = context.User.Identity.Name;
                }

                var callerApp = context.User.FindFirst("appid")?.Value;
                var cv = context.Request.Headers["MS-CV"];
                var test = context.Request.Headers["x-ms-test"];
                var correlationId = context.Request.Headers["x-ms-correlationId"];

                if (!cv.Any())
                {
                    cv = new CorrelationVector().Value;
                    context.Request.Headers.Add("MS-CV", cv);
                }
                if (!props.Keys.Contains("MS-CV"))
                {
                    props.Add("MS-CV", cv);
                }

                if (test.Any() && !props.Keys.Contains("x-ms-test"))
                {
                    props.Add("x-ms-test", test);
                }

                if (correlationId.Any() && !props.Keys.Contains("x-ms-correlationId"))
                {
                    props.Add("x-ms-correlationId", correlationId);
                }

                if (callerApp?.Any() == true && !props.Keys.Contains("CallerApp"))
                {
                    props.Add("CallerApp", callerApp);
                }
            }
        }
    }
}
