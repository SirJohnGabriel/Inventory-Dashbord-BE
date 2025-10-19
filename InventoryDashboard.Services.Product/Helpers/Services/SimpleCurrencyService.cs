namespace InventoryDashboard.Services.Product.Helpers.Services
{
    using System;
    using System.Collections.Generic;
    using InventoryDashboard.Infrastructure.Services.Interfaces;

    public class SimpleCurrencyService : ICurrencyService
    {
        private readonly Dictionary<string, decimal> rates = new ()
        {
            { "USD", 0.020m }, // assuming base currency is PHP (1 PHP = 0.02 USD)
            { "EUR", 0.018m },
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
