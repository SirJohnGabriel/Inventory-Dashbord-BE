namespace InventoryDashboard.Services.Product.Validators
{
    using FluentValidation;
    using InventoryDashboard.Infrastructure.Messages.Product;

    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            this.RuleFor(x => x.Name)
                .MaximumLength(250).WithMessage("Product name must not exceed 250 characters.");

            this.RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Product description must not exceed 250 characters.");

            this.RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be a non-negative value.");

            this.RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be a non-negative value.");

            this.RuleFor(x => x.SKU)
                .MaximumLength(100).WithMessage("SKU must not exceed 100 characters.");
        }
    }
}