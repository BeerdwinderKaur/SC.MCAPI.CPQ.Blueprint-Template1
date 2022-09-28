using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.FeatureManagement
{
    [FilterAlias("ClientIdentity")]
    public class ClientIdentityFeatureFilter : IFeatureFilter, IFeatureFilterMetadata
    {
        private readonly ILogger<ClientIdentityFeatureFilter> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientIdentityFeatureFilter(ILogger<ClientIdentityFeatureFilter> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            if (_httpContextAccessor.HttpContext == null) return Task.FromResult(false);
            var userIdentity = _httpContextAccessor.HttpContext.Items["Commerce.CallerName"]?.ToString();
            if (string.IsNullOrWhiteSpace(userIdentity)) return Task.FromResult(false);
            var settings = context.Parameters.Get<ClientIdentityFilterSettings>();

            var flagEvaluation = settings != null &&
                                 (settings.AppIds.Any(id => id.Equals("*"))
                                  ||
                                  settings.AppIds.Any(
                                      id => id.Equals(userIdentity, StringComparison.OrdinalIgnoreCase)))
                                 && (
                                     settings.Regions.Any(region => region.Equals("*")) ||
                                     settings.Regions.Any(region =>
                                         region.Equals(Environment.GetEnvironmentVariable("REGION_NAME"))));


            _logger.LogInformation($"{context.FeatureName} flag is set to : {flagEvaluation}");
            return Task.FromResult(flagEvaluation);
        }
    }
}
