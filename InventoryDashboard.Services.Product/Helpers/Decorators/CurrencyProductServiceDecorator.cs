namespace InventoryDashboard.Services.Product.Helpers.Decorators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Product;
    using InventoryDashboard.Infrastructure.Models.Products;
    using InventoryDashboard.Infrastructure.Services.Interfaces;

    // Decorator that wraps an IProductService and applies currency conversion to product models
    public class CurrencyProductServiceDecorator : IProductService
    {
        private readonly IProductService inner;
        private readonly ICurrencyService currencyService;

        public CurrencyProductServiceDecorator(IProductService inner, ICurrencyService currencyService)
        {
            this.inner = inner;
            this.currencyService = currencyService;
        }

        public Task<Response<AddProductResponse>> AddProductAsync(AddProductRequest request)
            => this.inner.AddProductAsync(request);

        public async Task<Response<ICollection<GetProductModel>>> GetProductsAsync(string targetCurrency = null)
        {
            var result = await this.inner.GetProductsAsync(targetCurrency);

            if (result?.Data == null || string.IsNullOrWhiteSpace(targetCurrency))
            {
                return result;
            }

            var target = targetCurrency.ToUpperInvariant();

            foreach (var p in result.Data)
            {
                // Convert the base price (assumed PHP) to target currency and place into optional fields
                var converted = this.currencyService.Convert(p.Price, "PHP", target);
                p.ConvertedPrice = converted;
                p.CurrencyCode = target;
            }

            return result;
        }
    }
}
