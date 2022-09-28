using Domain.Service.Telemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Middleware
{
    public class TelemetryMiddleware : IMiddleware
    {
        private readonly ILogger<TelemetryMiddleware> _logger;
        private readonly IMetricsProvider _metricsProvider;

        public TelemetryMiddleware(ILogger<TelemetryMiddleware> logger, IMetricsProvider metricsProvider)
        {
            _logger = logger;
            _metricsProvider = metricsProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);

                if (context.Request.Path.Value != null &&
                    !context.Request.Path.Value.Contains("/ping", StringComparison.OrdinalIgnoreCase))
                {
                    AddReliabilityMetrics(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception :");
                throw;
            }
        }

        private void AddReliabilityMetrics(HttpContext context)
        {
            var urlDimension =
                new KeyValuePair<string, object>("Url", $"{context.Request.Method} {context.Request.Path}");
            var responseStatusDimension = new KeyValuePair<string, object>("status", $"{context.Response.StatusCode}");
            _metricsProvider.LogReliabilityMetric(1, urlDimension, responseStatusDimension);
        }
    }
}
