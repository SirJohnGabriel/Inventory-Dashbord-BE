namespace InventoryDashboard.Api.Extensions.Adapters
{
    using System.Collections.Generic;
    using System.Linq;
    using InventoryDashboard.Api.Messages.Product;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Product;
    using InventoryDashboard.Infrastructure.Models.Products;
    using InventoryDashboard.Infrastructure.Services.Interfaces;

    public static class ProductAdapter
    {
        // Adapter helper to modernize legacy service Response<ICollection<GetProductModel>> -> GetProductsWebResponse
        public static GetProductsWebResponse ToWebResponse(this Response<ICollection<GetProductModel>> legacy)
        {
            if (legacy == null)
            {
                return new GetProductsWebResponse(new List<ProductData>(), string.Empty, string.Empty);
            }

            var productDataList = new List<ProductData>();

            if (legacy.Data != null)
            {
                foreach (var p in legacy.Data)
                {
                    var categoryId = p.Category?.Id.ToString() ?? string.Empty;

                    var pd = new ProductData(
                        p.Id.ToString(),
                        p.Name ?? string.Empty,
                        p.Description ?? string.Empty,
                        categoryId,
                        p.Price,
                        p.StockQuantity,
                        p.SKU ?? string.Empty,
                        false);

                    productDataList.Add(pd);
                }
            }

            return new GetProductsWebResponse(productDataList, legacy.ErrorCode, legacy.Message);
        }

        // Adapter helper to add Tax fields for V2 response
        public static GetProductsWebResponseV2 ToWebResponseWithTax(this Response<ICollection<GetProductModel>> legacy, ITaxService taxService)
        {
            if (legacy == null)
            {
                return new GetProductsWebResponseV2(new List<ProductDataV2>(), string.Empty, string.Empty);
            }

            // Reuse the base adapter to map common fields, then adapt with tax and optional decorator fields
            var baseResponse = legacy.ToWebResponse();
            var productDataList = new List<ProductDataV2>();

            if (baseResponse?.Data != null)
            {
                foreach (var basePd in baseResponse.Data)
                {
                    var basePrice = basePd.Price;
                    var rate = taxService?.GetTaxRateForCategory(basePd.CategoryId) ?? 0m;
                    var tax = decimal.Round(basePrice * rate, 2);
                    var priceWithTax = decimal.Round(basePrice + tax, 2);

                    var pd = new ProductDataV2(
                        basePd.Id,
                        basePd.Name ?? string.Empty,
                        basePd.Description ?? string.Empty,
                        basePd.CategoryId,
                        basePd.Price,
                        basePd.StockQuantity,
                        basePd.SKU ?? string.Empty,
                        basePd.IsDeleted)
                    {
                        TaxAmount = tax,
                        PriceWithTax = priceWithTax,

                        // preserve optional converted/currency fields from the original legacy model when available
                        ConvertedPrice = legacy.Data?.FirstOrDefault(m => m.Id.ToString() == basePd.Id)?.ConvertedPrice,
                        CurrencyCode = legacy.Data?.FirstOrDefault(m => m.Id.ToString() == basePd.Id)?.CurrencyCode ?? string.Empty,
                    };

                    productDataList.Add(pd);
                }
            }

            return new GetProductsWebResponseV2(productDataList, legacy.ErrorCode, legacy.Message);
        }

        // Overload: accept a single product response and reuse the collection adapters
        public static GetProductWebResponse ToWebResponse(this Response<GetProductModel> legacy)
        {
            if (legacy == null)
            {
                return new GetProductWebResponse(null, string.Empty, string.Empty);
            }

            var list = new List<GetProductModel>();
            if (legacy.Data != null)
            {
                list.Add(legacy.Data);
            }

            var wrapped = new Response<ICollection<GetProductModel>>();
            wrapped.Data = list;
            if (!string.IsNullOrEmpty(legacy.ErrorCode))
            {
                wrapped.SetError(legacy.ErrorCode, legacy.Message);
            }

            var collectionResp = wrapped.ToWebResponse();

            var single = collectionResp.Data?.FirstOrDefault();
            return new GetProductWebResponse(single, collectionResp.ErrorCode, collectionResp.Message);
        }

        public static GetProductWebResponseV2 ToWebResponseWithTax(this Response<GetProductModel> legacy, ITaxService taxService)
        {
            if (legacy == null)
            {
                return new GetProductWebResponseV2(null, string.Empty, string.Empty);
            }

            var list = new List<GetProductModel>();
            if (legacy.Data != null)
            {
                list.Add(legacy.Data);
            }

            var wrapped = new Response<ICollection<GetProductModel>>();
            wrapped.Data = list;
            if (!string.IsNullOrEmpty(legacy.ErrorCode))
            {
                wrapped.SetError(legacy.ErrorCode, legacy.Message);
            }

            var collectionResp = wrapped.ToWebResponseWithTax(taxService);
            var single = collectionResp.Data?.FirstOrDefault();
            return new GetProductWebResponseV2(single, collectionResp.ErrorCode, collectionResp.Message);
        }
    }
}
