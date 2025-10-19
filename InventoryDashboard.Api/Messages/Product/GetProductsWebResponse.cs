namespace InventoryDashboard.Api.Messages.Product
{
    using InventoryDashboard.Api.Common.Responses;
    using InventoryDashboard.Infrastructure.Messages.Product;

    public class GetProductsWebResponse : WebResponse<ICollection<ProductData>>
    {
        public GetProductsWebResponse(ICollection<ProductData> data, string errorCode = "", string message = "")
            : base(data, errorCode, message)
        {
        }
    }
}