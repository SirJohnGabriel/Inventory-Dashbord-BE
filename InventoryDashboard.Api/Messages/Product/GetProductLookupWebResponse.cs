namespace InventoryDashboard.Api.Messages.Product
{
    using System.Collections.Generic;
    using InventoryDashboard.Api.Common.Responses;

    public class GetProductLookupWebResponse : WebResponse<Dictionary<string, IEnumerable<LookupItem>>>
    {
        public GetProductLookupWebResponse(Dictionary<string, IEnumerable<LookupItem>> data, string errorCode, string message)
            : base(data, errorCode, message)
        {
        }
    }
}