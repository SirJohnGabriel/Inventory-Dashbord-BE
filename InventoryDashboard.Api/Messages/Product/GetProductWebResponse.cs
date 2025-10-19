namespace InventoryDashboard.Api.Messages.Product
{
    using InventoryDashboard.Api.Common.Responses;

    public class GetProductWebResponse : WebResponse<ProductData>
    {
        public GetProductWebResponse(ProductData data, string errorCode = "", string message = "")
            : base(data, errorCode, message)
        {
        }
    }
}
