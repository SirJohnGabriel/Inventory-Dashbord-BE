namespace InventoryDashboard.Infrastructure.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Product;
    using InventoryDashboard.Infrastructure.Models.Products;

    public interface IProductService
    {
        Task<Response<AddProductResponse>> AddProductAsync(AddProductRequest request);

        Task<Response<ICollection<GetProductModel>>> GetProductsAsync(string targetCurrency = null);

        Task<Response<GetProductModel>> GetProductByIdAsync(Guid id, string targetCurrency = null);

        Task<Response> DeleteProductByIdAsync(Guid productId, Guid currentUserId);
    }
}