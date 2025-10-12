namespace InventoryDashboard.Services.Identity.Workflows.SignInExternal
{
    using InventoryDashboard.Infrastructure.Entities.Identity;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Identity;
    using InventoryDashboard.Services.Identity.Models;
    using Microsoft.AspNetCore.Identity;

    public class SignInExternalWorkflowRequest
    {
        public SignInExternalWorkflowRequest(SignInExternalRequest request, Response<string> response)
        {
            this.Request = request;
            this.Response = response;
        }

        public SignInExternalRequest Request { get; private set; }

        public Response<string> Response { get; private set; }

        public TokenPayload Payload { get; set; }

        public UserLoginInfo UserLoginInfo { get; set; }

        public User User { get; set; }
    }
}