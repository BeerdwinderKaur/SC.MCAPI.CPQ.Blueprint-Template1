using Domain.Infrastructure.BigCat;
using Domain.Infrastructure.Interfaces;
using Domain.Infrastructure.ServiceHandlers;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Domain.Infrastructure.Extensions
{
    public static class InfraServiceBuilderExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<RequestResponseLogHandler>();
            // AddHttpClient is optimized to handle and dispose HttpClient objects
            // TransientErrorPolicy is preconfigured to retry only on Network failures, 5XX and 408 ( timeout )
            // Add Transient only if it is a GET call ( This could be helpful if you want to selectively choose which API endpoints you 
            // want to retry.
            serviceCollection.AddHttpClient<IBigCatRepository, BigCatRepository>(c =>
                {
                    c.BaseAddress = new Uri("http://api.bigcat.com/v1/current.json");
                    c.Timeout = TimeSpan.FromSeconds(5);

                })
                .AddHttpMessageHandler<RequestResponseLogHandler>() // This will log all incoming and outgoing requests and responses
                .RedactLoggedHeaders(new List<string>() { "Authorization" })
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)))
                .AddPolicyHandler(request =>
                {
                    if (request.Method == HttpMethod.Get)
                    {
                        return HttpPolicyExtensions.HandleTransientHttpError()
                            .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2));
                    }

                    return Policy.NoOpAsync<HttpResponseMessage>();
                });

            serviceCollection.AddSingleton<IGreetingsRepository, GreetingsRepository>();
            return serviceCollection;
        }
    }
}
