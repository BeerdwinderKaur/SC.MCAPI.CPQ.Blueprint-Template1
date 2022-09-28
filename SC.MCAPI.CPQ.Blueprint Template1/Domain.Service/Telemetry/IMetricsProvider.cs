using System.Collections.Generic;

namespace Domain.Service.Telemetry
{
    public interface IMetricsProvider
    {
        // TODO : Combine the below into one
        public void LogAvailabilityMetric(long value, params KeyValuePair<string, object>[] customDimensions);
        public void LogReliabilityMetric(long value, params KeyValuePair<string, object>[] customDimensions);

    }
}
