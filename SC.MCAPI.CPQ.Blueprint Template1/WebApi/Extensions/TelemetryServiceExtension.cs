using Domain.Service.Configurations;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using System;
using System.Collections.Generic;
using WebApi.Middleware;
using WebApi.TelemetryConfiguration;

namespace WebApi.Extensions
{
    public static class TelemetryServiceExtension
    {
        public static IServiceCollection ConfigureTelemetry(this IServiceCollection services,
            IConfiguration configuration)
        {
            var telemetryConfiguration = configuration.GetSection("Telemetry").Get<Domain.Service.Configurations.TelemetryConfiguration>();
            services.AddSingleton(telemetryConfiguration);
            return services;
        }

        public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TelemetryMiddleware>();
            // Logs to all sources linked below
            /*  comment this, to block OpenTelemetry */
            services.AddLogging(builder =>
            {
                // sets up OpenTelemetry logs for Information and above. * refers to all categories.
                builder.ClearProviders();
                builder.AddFilter<OpenTelemetryLoggerProvider>("*", LogLevel.Information);
                builder.AddOpenTelemetry(loggerOptions =>
                {
                    loggerOptions.IncludeFormattedMessage = true;
                    loggerOptions.IncludeScopes = true;
                    loggerOptions.ParseStateValues = true;
                    loggerOptions.AddConsoleExporter(); // If you need Console

                    // Add the Geneva Exporter & configure it
                    loggerOptions.AddGenevaLogExporter(exporterOptions =>
                    {
                        exporterOptions.ConnectionString = "EtwSession=OpenTelemetry";
                        exporterOptions.PrepopulatedFields = new Dictionary<string, object>
                        {
                            ["BuildNumber"] = configuration.GetValue<string>("BUILD_NUMBER"),
                            ["RoleInstance"] = "",
                            ["Region"] = Environment.GetEnvironmentVariable("REGION_NAME"),
                        };
                    });
                });
            });

            /*  comment this, to block AppInsights */
            var aiOptions = new ApplicationInsightsServiceOptions
            {
                EnableAdaptiveSampling = false,
                InstrumentationKey = configuration.GetValue<string>("Telemetry:AppInsightsInstrumentationKey")
            };
            services.AddApplicationInsightsTelemetry(aiOptions);
            services.AddSingleton<ITelemetryInitializer, AppInsightsTelemetryInitializer>();
            return services;
        }
    }
}
