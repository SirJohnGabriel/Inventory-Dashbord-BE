namespace InventoryDashboard.Infrastructure.Constants.Errors
{
    public class IdentityServiceErrorCodes
    {
        public const string InvalidCredential = "identity/invalid-credential";

        public const string UserNotFound = "identity/user-not-found";

        public const string MicrosoftTokenValidationError = "identity/microsoft/token-validation-error";

        public const string GoogleTokenValidationError = "identity/google/token-validation-error";

        public const string GoogleEmailAlreadyInUse = "identity/google/duplicate-email";

        public const string SocialUserForgotPasswordError = "identity/forgot-password/social-user";

        public const string DuplicateEmailAddress = "identity/duplicate-email";

        public const string UnexpectedError = "identity/unexpected-error";

        public const string InvalidToken = "identity/reset-password/invalid-token";

        public const string ValidationError = "identity/validation-error";

        public const string UserSuspended = "identity/user-suspended";

        public const string UserDisabled = "identity/user-disabled";
    }
}
