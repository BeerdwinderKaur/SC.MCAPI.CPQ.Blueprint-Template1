using Domain.Service.Configurations;
using Domain.Service.Telemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Commerce.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace WebApi.Extensions
{
    public static class SecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseAadServices(this IApplicationBuilder app, IConfiguration configuration)
        {
            // Registers the AadConfiguration from config file to Services for DI
            var aadConfiguration = configuration.GetSection("AadConfiguration").Get<AadConfiguration>();

            // Incoming AuthN using MISE
            var openIdConnectServiceUrl = "https://login.microsoftonline.com/common/.well-known/openid-configuration";
            var aadAuthHandler = new TokenAuthenticationHandler(
                aadConfiguration.FirstPartyAppId, new HashSet<string>() { openIdConnectServiceUrl },
                aadConfiguration.AllowedAudiences.ToDictionary(
                    audienceKey => audienceKey,
                    audienceValue => audienceValue),
                aadConfiguration.AllowedTenants.ToHashSet(),
                aadConfiguration.AllowedAppIds.ToDictionary(audienceKey => audienceKey,
                    audienceValue => audienceValue));

            var authenticationHandlers = new List<IAuthenticationHandler> { aadAuthHandler };
            var metricsProvider = app.ApplicationServices.GetService<IMetricsProvider>();
            var authDelegatingHandler =
                new AuthenticationDelegatingHandler(authenticationHandlers, ConfigureForPing(metricsProvider));

            authDelegatingHandler.Use(app);
            return app;
        }

        private static Action<HttpContext> ConfigureForPing(IMetricsProvider metricsProvider)
        {
            return async (context) =>
            {
                if (context.Request.Path.Value != null && context.Request.Path.HasValue)
                {
                    if (!context.Request.Path.Value.EndsWith("ping", StringComparison.OrdinalIgnoreCase))
                    {
                        AddReliabilityMetrics(context, metricsProvider);
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync(
                            "Authentication failed. Confirm valid credentials are passed.");
                    }
                    else if (context.Request.Path.Value.EndsWith("ping", StringComparison.OrdinalIgnoreCase))
                    {
                        AddAvailabilityMetrics(context, metricsProvider);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        await context.Response.WriteAsync(
                            "App is Healthy !");
                    }
                }
            };
        }

        private static void AddReliabilityMetrics(HttpContext context, IMetricsProvider metricsProvider)
        {
            var urlDimension =
                new KeyValuePair<string, object>("Url", $"{context.Request.Method} {context.Request.Path}");
            var responseStatusDimension = new KeyValuePair<string, object>("status", "401");
            metricsProvider.LogReliabilityMetric(1, urlDimension, responseStatusDimension);
        }

        private static void AddAvailabilityMetrics(HttpContext context, IMetricsProvider metricsProvider)
        {
            var urlDimension =
                new KeyValuePair<string, object>("Url", $"{context.Request.Method} {context.Request.Path}");
            var responseStatusDimension = new KeyValuePair<string, object>("status", "200");
            var applicationDimension = new KeyValuePair<string, object>("Proposal", "Blueprint"); // TODO : Replace with your app name
            metricsProvider.LogAvailabilityMetric(100, urlDimension, responseStatusDimension, applicationDimension);
        }
    }
}
