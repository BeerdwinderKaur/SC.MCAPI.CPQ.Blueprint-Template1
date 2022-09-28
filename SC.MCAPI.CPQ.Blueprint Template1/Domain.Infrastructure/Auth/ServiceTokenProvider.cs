using Domain.Infrastructure.Extensions;
using Domain.Infrastructure.Interfaces.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure.Auth
{
    public class ServiceTokenProvider : IServiceTokenProvider
    {
        private readonly X509Certificate2 _tokenCertificate;

        public ServiceTokenProvider(IConfiguration configuration)
        {
            _tokenCertificate =
                configuration.GetCertificateBySecretName("token-secret-name");
        }


        public async Task<string> GetTokenForClient(string clientId, string authority, string resourceId)
        {
            // This approach is as suggested by the AAD team based on email discussion on 05/18/2022
            // Docs to support : https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-net-token-cache-serialization?tabs=aspnet#token-cache-without-serialization
            var confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(clientId)
                // TODO : the Configuration value should be cleaned up..we should use constants
                .WithCertificate(_tokenCertificate)
                .WithAuthority(authority)
                .WithAzureRegion()
                .WithCacheOptions(CacheOptions.EnableSharedCacheOptions)
                .WithLegacyCacheCompatibility(false)
                .Build();

            var authResult = await confidentialClientApplication
                .AcquireTokenForClient(
                    new[] { $"{resourceId}/.default" })
                .WithSendX5C(true)
                .ExecuteAsync()
                .ConfigureAwait(false);

            return authResult.AccessToken;
        }
    }
}
