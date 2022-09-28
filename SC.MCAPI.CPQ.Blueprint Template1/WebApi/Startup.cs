using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Domain.Infrastructure.Extensions;
using Domain.Service.Extensions;
using MCAPI.Shared.Services.Http;
using MCAPI.Shared.Services.Models.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Commerce.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using OpenTelemetry.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WebApi.Extensions;
using WebApi.FeatureManagement;
using WebApi.Middleware;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDomainServices()
                .AddInfrastructureServices(); // Registers all dependencies in Domain and infra service

            services.AddTelemetry(Configuration).ConfigureTelemetry(Configuration);
            services.AddFeatureManagement().AddFeatureFilter<ClientIdentityFeatureFilter>();
            services.AddAzureAppConfiguration();
            services.AddHeaderPropagation(option =>
            {
                option.Headers.Add("MS-CV");
                option.Headers.Add("x-ms-correlationId");

            });

            /* Commenting this for now since according to email from Logging team on 09/13/2022 ( Chris Ross), this does \
             not log outgoing calls

            services.AddHttpLogging(httpLogging =>
            {
                httpLogging.LoggingFields = HttpLoggingFields.All;
                httpLogging.RequestHeaders.Add("MS-CV"); // All, except MS-CV, would be redacted.
                httpLogging.RequestBodyLogLimit = 4096;
                httpLogging.ResponseBodyLogLimit = 4096;
            });
            services.AddControllers();

            /*
             // Add  for non-local : Reads configs from KV

             IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
             var secretClient = new SecretClient(
                 new Uri(Configuration["environmentConfiguration:KeyVaultUrl"]), new DefaultAzureCredential());
             configurationBuilder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());

             */

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<TelemetryMiddleware>();
            if (env.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseHttpExceptions();
                app.UseAzureAppConfiguration(); // TODO : Include Dev App Configuration to read FF
                ValidationError.DefaultServiceName =
                    "MyBlueprintService"; // TODO: Change name from BlueprintService to your service name
            }

            /* Commenting this for now since according to email from Logging team on 09/13/2022 ( Chris Ross),
             this does not log outgoing calls */
            // app.UseHttpLogging();

            app.UseHttpsRedirection();
            app.UseHeaderPropagation();
            app.UseRouting();
            app.UseCors(policy =>
            {
                policy
                    .WithOrigins(Configuration.GetSection("CorsPolicy:AllowedOrigins").Get<string[]>())
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .WithExposedHeaders(Configuration.GetSection("CorsPolicy:ExposedHeaders").Get<string[]>())
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(604800));
            });

            app.UseAadServices(Configuration); // NOTE : There is a known non-blocking error coming from MISE.
            // It says Response headers cannot be changed.
            // Kelly Walker is looking into it.

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
