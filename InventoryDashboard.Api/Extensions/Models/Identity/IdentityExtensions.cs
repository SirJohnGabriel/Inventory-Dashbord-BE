namespace InventoryDashboard.Api.Extensions.Models.Identity
{
    using InventoryDashboard.Api.Messages.Identity;
    using InventoryDashboard.Infrastructure.Messages;

    public static class IdentityExtensions
    {
        public static SignUpWebResponse AsSignUpWebResponse(this Response response)
        {
            var result = new SignUpWebResponse(response.ErrorCode, response.Message);
            return result;
        }

        public static SignInWebResponse AsSignInWebResponse(this Response<string> response)
        {
            var result = new SignInWebResponse(response.Data, response.ErrorCode, response.Message);
            return result;
        }

        public static SignInExternalWebResponse AsSignInExternalWebResponse(this Response<string> response)
        {
            var result = new SignInExternalWebResponse(response.Data, response.ErrorCode, response.Message);
            return result;
        }

        public static ForgotPasswordWebResponse AsForgotPasswordWebResponse(this Response<string> response)
        {
            var result = new ForgotPasswordWebResponse(response.Data, response.ErrorCode, response.Message);
            return result;
        }

        public static ResetPasswordWebResponse AsResetPasswordWebResponse(this Response response)
        {
            var result = new ResetPasswordWebResponse(response.ErrorCode, response.Message);
            return result;
        }
    }
}