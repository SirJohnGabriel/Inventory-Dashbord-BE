namespace InventoryDashboard.Services.Identity.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using InventoryDashboard.Infrastructure.Constants.Errors;
    using InventoryDashboard.Infrastructure.Messages;
    using Microsoft.AspNetCore.Identity;

    internal static class IdentityResultExtensions
    {
        private static readonly Dictionary<string, string> ErrorMessageMapping = new Dictionary<string, string>
        {
            { "DuplicateUserName", IdentityServiceErrorCodes.DuplicateEmailAddress },
            { IdentityServiceErrorCodes.UnexpectedError, string.Empty },
        };

        public static void HandleIdentityResultError(this IdentityResult result, ref Response response)
        {
            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault()?.Code ?? IdentityServiceErrorCodes.UnexpectedError;

                if (!string.IsNullOrEmpty(error))
                {
                    if (ErrorMessageMapping.TryGetValue(error, out var mappedError))
                    {
                        response.SetError(mappedError);
                    }
                    else
                    {
                        response.SetError(IdentityServiceErrorCodes.UnexpectedError);
                    }
                }
            }
        }
    }
}
