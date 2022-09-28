using Domain.Infrastructure.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static X509Certificate2 GetCertificateBySecretName(this IConfiguration configuration, string secretName)
        {
            string secretValue = configuration[secretName];
            if (string.IsNullOrEmpty(secretValue))
                throw new KeyNotFoundException($"{secretName} not found in the KV configuration");

            byte[] pfxBytesClientCertificate = Convert.FromBase64String(secretValue);
            var certificate = new X509Certificate2(pfxBytesClientCertificate, string.Empty, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            return certificate;
        }

        public static IServiceCollection AddInfrastructureConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var aadConfiguration = configuration.GetSection("AadConfiguration").Get<AadConfiguration>();
            serviceCollection.AddSingleton(aadConfiguration);

            return serviceCollection;
        }
    }
}
