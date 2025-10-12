namespace InventoryDashboard.Infrastructure.Logging
{
    using System;
    using System.Collections.Generic;

    public class ConsoleLogger : ILogger
    {
        public void StackTrace(string eventName, IDictionary<string, string> properties = null)
        {
            Console.WriteLine($"[EVENT] {eventName}");
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    Console.WriteLine($"  {prop.Key}: {prop.Value}");
                }
            }
        }

        public void WriteException(Exception ex, IDictionary<string, string> properties = null)
        {
            Console.WriteLine($"[EXCEPTION] {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            if (properties != null)
            {
                Console.WriteLine("Properties:");
                foreach (var prop in properties)
                {
                    Console.WriteLine($"  {prop.Key}: {prop.Value}");
                }
            }
        }
    }
}