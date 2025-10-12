namespace InventoryDashboard.Api.Extensions.Validations
{
    using InventoryDashboard.Infrastructure.Validations;

    public static class ValidationExtension
    {
        public static void RegisterValidators(this IServiceCollection services)
        {
            services.AddTransient<Validator>();
        }
    }
}