namespace InventoryDashboard.Api.Messages.Product
{
    using InventoryDashboard.Api.Common.Responses;

    public class AddProductWebResponseV2 : WebResponse<ProductDataV2>
    {
        public AddProductWebResponseV2(ProductDataV2 data, string errorCode = "", string message = "")
            : base(data, errorCode, message)
        {
        }
    }
}
