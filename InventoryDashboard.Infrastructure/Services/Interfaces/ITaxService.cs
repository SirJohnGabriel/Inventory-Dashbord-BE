namespace InventoryDashboard.Infrastructure.Services.Interfaces
{
    public interface ITaxService
    {
        decimal GetTaxRateForCategory(string categoryId);
    }
}
