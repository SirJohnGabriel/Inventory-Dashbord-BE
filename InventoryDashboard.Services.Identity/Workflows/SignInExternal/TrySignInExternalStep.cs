namespace InventoryDashboard.Services.Identity.Workflows.SignInExternal
{
    using System.Threading.Tasks;
    using InventoryDashboard.Infrastructure.Entities.Identity;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Workflows;
    using InventoryDashboard.Services.Identity.Extensions;
    using InventoryDashboard.Services.Identity.Helpers;
    using Microsoft.AspNetCore.Identity;

    internal class TrySignInExternalStep : AsyncStep<SignInExternalWorkflowRequest, Response<string>>
    {
        private readonly UserManager<User> userManager;
        private readonly JwtHelper jwtHelper;

        public TrySignInExternalStep(UserManager<User> userManager, JwtHelper jwtHelper)
        {
            this.userManager = userManager;
            this.jwtHelper = jwtHelper;
        }

        public override async Task<Response<string>> ExecuteAsync(SignInExternalWorkflowRequest request)
        {
            var info = new UserLoginInfo(request.Request.Provider, request.Payload.Subject, request.Request.Provider);
            request.UserLoginInfo = info;

            var user = await this.userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                return await this.ExecuteNextAsync(request);
            }

            request.User = user;
            request.Response.Data = await this.userManager.GenerateJwtAsync(this.jwtHelper, user);
            return request.Response;
        }
    }
}
