namespace InventoryDashboard.Api.Messages.Product
{
    using InventoryDashboard.Api.Common.Responses;

    public class UpdateProductWebResponse : WebResponse<ProductData>
    {
        public UpdateProductWebResponse(ProductData data, string errorCode = "", string message = "")
            : base(data, errorCode, message)
        {
        }
    }
}