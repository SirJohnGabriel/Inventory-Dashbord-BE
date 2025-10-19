namespace InventoryDashboard.Services.Product.Validators
{
    using FluentValidation;
    using InventoryDashboard.Infrastructure.Messages.Product;

    public class AddProductRequestValidator : AbstractValidator<AddProductRequest>
    {
        public AddProductRequestValidator()
        {
            this.RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(250).WithMessage("Product name must not exceed 250 characters.");

            this.RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Product description must not exceed 250 characters.");

            this.RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID is required.");

            this.RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be a non-negative value.");

            this.RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be a non-negative value.");

            this.RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("SKU is required.")
                .MaximumLength(100).WithMessage("SKU must not exceed 100 characters.");
        }
    }
}