namespace InventoryDashboard.Services.Identity.Validators.SignUp
{
    using FluentValidation;
    using FluentValidation.Validators;
    using InventoryDashboard.Infrastructure.Messages.Identity;

    public class SignUpValidator : AbstractValidator<SignUpRequest>
    {
        public SignUpValidator()
        {
            this.RuleFor(req => req.UserName).NotEmpty();
            this.RuleFor(req => req.UserName).EmailAddress(EmailValidationMode.AspNetCoreCompatible);
            this.RuleFor(req => req.Password).NotEmpty();
            this.RuleFor(req => req.ConfirmPassword).NotEmpty();

            this.RuleFor(req => req.ConfirmPassword).Equal(req => req.Password).WithMessage("'Confirm Password' must be the same as Password.");
        }
    }
}
