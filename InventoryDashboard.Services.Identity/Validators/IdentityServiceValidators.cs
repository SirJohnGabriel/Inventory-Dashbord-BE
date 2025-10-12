namespace InventoryDashboard.Services.Identity.Validators
{
    using FluentValidation;
    using InventoryDashboard.Infrastructure.Messages.Identity;

    public sealed class IdentityServiceValidators
    {
        public IdentityServiceValidators(IValidator<SignUpRequest> signUpValidator, IValidator<SignInRequest> signInValidator)
        {
            this.SignUpValidator = signUpValidator;
            this.SignInValidator = signInValidator;
        }

        public IValidator<SignUpRequest> SignUpValidator { get; private set; }

        public IValidator<SignInRequest> SignInValidator { get; private set; }
    }
}