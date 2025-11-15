namespace InventoryDashboard.Api.Messages.Product
{
    using InventoryDashboard.Api.Common.Responses;

    public class UpdateProductWebResponseV2 : WebResponse<ProductDataV2>
    {
        public UpdateProductWebResponseV2(ProductDataV2 data, string errorCode = "", string message = "")
            : base(data, errorCode, message)
        {
        }
    }
}
