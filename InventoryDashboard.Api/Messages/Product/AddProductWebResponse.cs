namespace InventoryDashboard.Api.Messages.Product
{
    using InventoryDashboard.Api.Common.Responses;
    using InventoryDashboard.Infrastructure.Messages.Product;

    public class AddProductWebResponse : WebResponse<ProductData>
    {
        public AddProductWebResponse(ProductData data, string errorCode = "", string message = "")
            : base(data, errorCode, message)
        {
        }
    }
}