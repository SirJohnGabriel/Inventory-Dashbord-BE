namespace InventoryDashboard.Services.Product.Helpers.Services
{
    using System;
    using System.Collections.Generic;
    using InventoryDashboard.Infrastructure.Services.Interfaces;

    public class SimpleCurrencyService : ICurrencyService
    {
        // Simple hardcoded currency conversion rates for demo purposes.
        // Default base currency is PHP.
        // Rates as of Oct 17, 9:01 PM UTC
        private readonly Dictionary<string, decimal> rates = new ()
        {
            { "USD", 0.017m },
            { "EUR", 0.015m },
            { "JPY", 2.59m },
            { "CN¥", 0.12m },
            { "NZD", 0.03m },
            { "PHP", 1m },
        };

        public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
        {
            if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return amount;
            }

            // For demo, assume 'fromCurrency' is PHP.
            var to = toCurrency.ToUpperInvariant();
            if (!this.rates.TryGetValue(to, out var rate))
            {
                return amount;
            }

            return decimal.Round(amount * rate, 2);
        }

        public bool IsSupportedCurrency(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
            {
                return false;
            }

            return this.rates.ContainsKey(currencyCode.ToUpperInvariant());
        }
    }
}
