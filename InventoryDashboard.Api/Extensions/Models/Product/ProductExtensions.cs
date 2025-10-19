namespace InventoryDashboard.Api.Extensions.Models.Product
{
    using System;
    using InventoryDashboard.Api.Messages.Product;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Product;
    using InventoryDashboard.Infrastructure.Models.Products;

    public static class ProductExtensions
    {
        public static AddProductRequest ToAddProductRequest(this AddProductWebRequest request, Guid userId)
        {
            return new AddProductRequest
            {
                Name = request.Name,
                Description = request.Description ?? string.Empty,
                CategoryId = request.CategoryId,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                SKU = request.SKU,
                CreatedBy = userId,
                UpdatedBy = userId,
            };
        }

        public static AddProductWebResponse AsAddProductWebResponse(this Response<AddProductResponse> response)
        {
            var productData = response.Data != null
                ? new ProductData(
                    response.Data.ProductId,
                    response.Data.Name,
                    response.Data.Description,
                    response.Data.CategoryId,
                    response.Data.Price,
                    response.Data.StockQuantity,
                    response.Data.SKU,
                    false)
                : new ProductData(string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, string.Empty, false);

            return new AddProductWebResponse(
                productData,
                response.ErrorCode,
                response.Message);
        }

        public static GetProductsWebResponse AsGetProductsWebResponse(this Response<ICollection<GetProductModel>> response)
        {
            var productDataList = new List<ProductData>();

            if (response.Data != null)
            {
                foreach (var product in response.Data)
                {
                    var productData = new ProductData(
                        product.Id.ToString(),
                        product.Name,
                        product.Description,
                        product.Category.Name,
                        product.Price,
                        product.StockQuantity,
                        product.SKU,
                        false);

                    productDataList.Add(productData);
                }
            }

            return new GetProductsWebResponse(
                productDataList,
                response.ErrorCode,
                response.Message);
        }

        public static GetProductWebResponse AsGetProductWebResponse(this Response<GetProductModel> response)
        {
            var productData = response.Data != null
                ? new ProductData(
                    response.Data.Id.ToString(),
                    response.Data.Name,
                    response.Data.Description,
                    response.Data.Category.Name,
                    response.Data.Price,
                    response.Data.StockQuantity,
                    response.Data.SKU,
                    false)
                : new ProductData(string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, string.Empty, false);

            return new GetProductWebResponse(
                productData,
                response.ErrorCode,
                response.Message);
        }

        public static DeleteProductWebResponse AsDeleteProductWebResponse(this Response response)
        {
            return new DeleteProductWebResponse(
                response.ErrorCode,
                response.Message);
        }
    }
}