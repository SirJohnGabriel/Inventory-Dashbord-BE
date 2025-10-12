namespace InventoryDashboard.Infrastructure.Logging
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;

    public class InsightsLogger : ILogger
    {
        private readonly TelemetryClient client;

        public InsightsLogger(string connectionString)
        {
#pragma warning disable CA2000
            var configuration = new TelemetryConfiguration();
            configuration.ConnectionString = connectionString;
            this.client = new TelemetryClient(configuration);
#pragma warning restore CA2000
        }

        public void StackTrace(string eventName, IDictionary<string, string> properties = null)
        {
            this.client.TrackEvent(eventName, properties);
        }

        public void WriteException(Exception ex, IDictionary<string, string> properties = null)
        {
            this.client.TrackException(ex, properties);
        }
    }
}