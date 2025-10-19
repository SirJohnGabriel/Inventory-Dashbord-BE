namespace InventoryDashboard.Api.Messages.Product
{
    using InventoryDashboard.Api.Common.Responses;

    public class GetProductWebResponseV2 : WebResponse<ProductDataV2>
    {
        public GetProductWebResponseV2(ProductDataV2 data, string errorCode = "", string message = "")
            : base(data, errorCode, message)
        {
        }
    }
}
