using Domain.Service.Configurations;
using OpenTelemetry;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Metrics;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Domain.Service.Telemetry
{
    public class GenevaMetricsProvider : IMetricsProvider
    {
        private static readonly Meter _myMeter = new Meter("Microsoft.Commerce.ProposalManagement.Runner.Metrics", "1.0");
        private static readonly Counter<long> _availabilityCounter = _myMeter.CreateCounter<long>("AvailabilityCounter");
        private static readonly Counter<long> _reliabilityCounter = _myMeter.CreateCounter<long>("ReliabilityCounter");

        public GenevaMetricsProvider(TelemetryConfiguration telemetryConfiguration)
        {
            // Create your meter provider
            var meterProvider = Sdk.CreateMeterProviderBuilder()
                // Add your meter to the provider as a source
                .AddMeter(_myMeter.Name)
                .AddGenevaMetricExporter(options =>
                {
                    options.ConnectionString =
                        $"Account={telemetryConfiguration.GenevaAccount};Namespace={telemetryConfiguration.GenevaNamespace}";
                })
                .Build();
        }

        public void LogAvailabilityMetric(long value, params KeyValuePair<string, object>[] customDimensions)
        {
            _availabilityCounter.Add(value, customDimensions);
        }

        public void LogReliabilityMetric(long value, params KeyValuePair<string, object>[] customDimensions)
        {
            _reliabilityCounter.Add(value, customDimensions);
        }
    }
}
