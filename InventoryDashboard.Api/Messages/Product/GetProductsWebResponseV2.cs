namespace InventoryDashboard.Api.Messages.Product
{
    using System.Collections.Generic;
    using InventoryDashboard.Api.Common.Responses;

    public class GetProductsWebResponseV2 : WebResponse<ICollection<ProductDataV2>>
    {
        public GetProductsWebResponseV2(ICollection<ProductDataV2> data, string errorCode = "", string message = "")
            : base(data, errorCode, message)
        {
        }
    }
}
