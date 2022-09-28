using Domain.Service.Configurations;
using Domain.Service.Hello;
using Domain.Service.Interface;
using Domain.Service.Telemetry;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Service.Extensions
{
    public static class ServiceBuilderExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IHelloDomainService, HelloDomainService>();
            serviceCollection.AddSingleton<IMetricsProvider, GenevaMetricsProvider>();
            serviceCollection.AddSingleton<TelemetryConfiguration>();
            return serviceCollection;
        }
    }
}
