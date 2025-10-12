namespace InventoryDashboard.Api.Messages.Identity
{
    using System.Collections.Generic;
    using System.Net;
    using InventoryDashboard.Infrastructure.Constants.Errors;
    using Newtonsoft.Json;

    public class AddToRoleWebResponse : Common.Responses.WebResponse
    {
        public AddToRoleWebResponse(string errorCode, string message)
            : base(errorCode, message)
        {
        }

        [JsonIgnore]
        public override Dictionary<string, HttpStatusCode> ErrorCodes => new Dictionary<string, HttpStatusCode>()
        {
            { IdentityServiceErrorCodes.UserNotFound, HttpStatusCode.BadRequest },
        };
    }
}
