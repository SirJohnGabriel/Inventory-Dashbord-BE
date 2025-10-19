namespace InventoryDashboard.Api.Messages.Product
{
    using InventoryDashboard.Api.Common.Responses;

    public class DeleteProductWebResponse : WebResponse
    {
        public DeleteProductWebResponse(string errorCode = "", string message = "")
            : base(errorCode, message)
        {
        }
    }
}