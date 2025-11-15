namespace InventoryDashboard.Services.Product.Helpers.Decorators
{
    using System;
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
                var converted = this.currencyService.Convert(p.Price, "PHP", target);
                p.ConvertedPrice = converted;
                p.CurrencyCode = target;
            }

            return result;
        }

        public async Task<Response<GetProductModel>> GetProductByIdAsync(Guid id, string targetCurrency = null)
        {
            var result = await this.inner.GetProductByIdAsync(id, targetCurrency);

            if (result?.Data == null || string.IsNullOrWhiteSpace(targetCurrency))
            {
                return result;
            }

            var target = targetCurrency.ToUpperInvariant();
            var converted = this.currencyService.Convert(result.Data.Price, "PHP", target);
            result.Data.ConvertedPrice = converted;
            result.Data.CurrencyCode = target;

            return result;
        }

        public Task<Response> DeleteProductByIdAsync(Guid productId, Guid currentUserId)
            => this.inner.DeleteProductByIdAsync(productId, currentUserId);

        public async Task<Response<UpdateProductResponse>> UpdateProductAsync(UpdateProductRequest request, string targetCurrency = null)
        {
            var result = await this.inner.UpdateProductAsync(request);

            if (result?.Data == null || string.IsNullOrWhiteSpace(targetCurrency))
            {
                return result;
            }

            var target = targetCurrency.ToUpperInvariant();
            var converted = this.currencyService.Convert(result.Data.Price, "PHP", target);
            result.Data.ConvertedPrice = converted;
            result.Data.CurrencyCode = target;

            return result;
        }

        public Task<Response<Dictionary<string, IEnumerable<KeyValuePair<string, string>>>>> GetLookupsAsync()
            => this.inner.GetLookupsAsync();
    }
}
