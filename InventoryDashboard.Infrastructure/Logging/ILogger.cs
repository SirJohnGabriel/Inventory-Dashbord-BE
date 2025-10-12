namespace InventoryDashboard.Infrastructure.Logging
{
    using System;
    using System.Collections.Generic;

    public interface ILogger
    {
        void WriteException(Exception ex, IDictionary<string, string> properties = null);

        void StackTrace(string eventName, IDictionary<string, string> properties = null);
    }
}