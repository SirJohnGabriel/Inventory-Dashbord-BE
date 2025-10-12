namespace InventoryDashboard.Api.Extensions.ServiceConfigurations
{
    using InventoryDashboard.Api.Filters;

    public static class MiddlewareConfiguration
    {
        public static void RegisterMiddlewares(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<HandleExceptionAttribute>();
            });

            services.AddOpenApi();
        }
    }
}