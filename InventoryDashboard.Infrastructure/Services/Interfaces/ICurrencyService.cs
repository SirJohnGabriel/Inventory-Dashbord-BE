namespace InventoryDashboard.Infrastructure.Services.Interfaces
{
    public interface ICurrencyService
    {
        decimal Convert(decimal amount, string fromCurrency, string toCurrency);

        bool IsSupportedCurrency(string currencyCode);
    }
}
