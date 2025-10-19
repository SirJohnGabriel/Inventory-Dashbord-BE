namespace InventoryDashboard.Services.Product.Helpers.Services
{
    using System.Collections.Generic;
    using InventoryDashboard.Infrastructure.Services.Interfaces;

    public class CategoryTaxService : ITaxService
    {
        private readonly Dictionary<string, decimal> rates = new ()
        {
            { "eb360ca0-ee0c-4be6-bc9b-7cb45644ae53", 0.12m }, // Electronics
            { "00b08252-2abd-46a5-a696-05d3d0609349", 0.05m }, // Office Supplies
            { "2503ae27-3b60-43da-9a53-943f7cde5295", 0.0m },  // Books
        };

        public decimal GetTaxRateForCategory(string categoryId)
            => this.rates.TryGetValue(categoryId, out var rate) ? rate : 0m;
    }
}
