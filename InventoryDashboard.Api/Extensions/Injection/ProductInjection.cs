namespace InventoryDashboard.Api.Extensions.Injection
{
    using FluentValidation;
    using InventoryDashboard.Infrastructure.Messages.Product;
    using InventoryDashboard.Infrastructure.Services.Interfaces;
    using InventoryDashboard.Services.Product;
    using InventoryDashboard.Services.Product.Data;
    using InventoryDashboard.Services.Product.Helpers.Decorators;
    using InventoryDashboard.Services.Product.Helpers.Services;
    using InventoryDashboard.Services.Product.Validators;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class ProductInjection
    {
        public static void InjectProductService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProductDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQL") ??
                    throw new InvalidOperationException("PostgreSQL connection string is required")));

            services.AddScoped<IValidator<AddProductRequest>, AddProductRequestValidator>();

            services.AddScoped<ProductService>();

            services.AddSingleton<ICurrencyService, SimpleCurrencyService>();

            services.AddSingleton<ITaxService, CategoryTaxService>();

            services.AddScoped<IProductService>(sp =>
            {
                var inner = sp.GetRequiredService<ProductService>();
                var currency = sp.GetRequiredService<ICurrencyService>();
                return new CurrencyProductServiceDecorator(inner, currency);
            });
        }
    }
}