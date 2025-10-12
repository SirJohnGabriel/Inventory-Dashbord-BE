namespace InventoryDashboard.Services.Identity.Workflows.SignInExternal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using InventoryDashboard.Infrastructure.Constants.Errors;
    using InventoryDashboard.Infrastructure.Enums.Identity;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Workflows;
    using InventoryDashboard.Services.Identity.Helpers;

    internal class ValidateExternalTokenStep : AsyncStep<SignInExternalWorkflowRequest, Response<string>>
    {
        private static readonly IDictionary<Provider, string> PossibleErrors = new Dictionary<Provider, string>()
        {
            { Provider.GOOGLE, IdentityServiceErrorCodes.GoogleTokenValidationError },
            { Provider.MICROSOFT, IdentityServiceErrorCodes.MicrosoftTokenValidationError },
        };

        private readonly JwtHelper jwtHelper;

        public ValidateExternalTokenStep(JwtHelper jwtHelper)
        {
            this.jwtHelper = jwtHelper;
        }

        public override async Task<Response<string>> ExecuteAsync(SignInExternalWorkflowRequest request)
        {
            var payload = this.jwtHelper.ValidateToken(request.Request.IdToken, request.Request.Provider);

            if (payload == null)
            {
                _ = Enum.TryParse(request.Request.Provider, out Provider provider);

                _ = PossibleErrors.TryGetValue(provider, out var errorCode);

                if (!string.IsNullOrEmpty(errorCode))
                {
                    request.Response.SetError(errorCode);
                    return request.Response;
                }
            }

            request.Payload = payload;
            return await this.ExecuteNextAsync(request);
        }
    }
}