namespace InventoryDashboard.Api.Messages.Identity
{
    using System.Net;
    using System.Text.Json.Serialization;
    using InventoryDashboard.Infrastructure.Constants.Errors;

    public class SignUpWebResponse : Common.Responses.WebResponse
    {
        public SignUpWebResponse(string errorCode, string message)
            : base(errorCode, message)
        {
        }

        [JsonIgnore]
        public override Dictionary<string, HttpStatusCode> ErrorCodes => new Dictionary<string, HttpStatusCode>()
        {
            { IdentityServiceErrorCodes.DuplicateEmailAddress, HttpStatusCode.BadRequest },
            { IdentityServiceErrorCodes.UnexpectedError, HttpStatusCode.BadRequest },
        };

        [JsonIgnore]
        public override HttpStatusCode SuccessCode => HttpStatusCode.Created;
    }
}