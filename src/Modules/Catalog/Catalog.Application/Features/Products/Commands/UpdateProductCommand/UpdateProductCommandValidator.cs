using FluentValidation;

namespace Catalog.Application.Features.Products.Commands.UpdateProductCommand;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Product ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(256).WithMessage("Product name cannot exceed 256 characters.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("Product SKU is required.")
            .MaximumLength(250).WithMessage("Product SKU cannot exceed 250 characters.");

        RuleFor(x => x.Slug)
            .MaximumLength(256).WithMessage("Product slug cannot exceed 256 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Slug));

        RuleFor(x => x.Description)
            .MaximumLength(50000).WithMessage("Description cannot exceed 50000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleForEach(x => x.Images).ChildRules(img =>
        {
            img.RuleFor(i => i.DesktopUrl)
                .NotEmpty().WithMessage("Desktop image URL is required.");

            img.RuleFor(i => i.MobileUrl)
                .NotEmpty().WithMessage("Mobile image URL is required.");
        }).When(x => x.Images != null);

        RuleForEach(x => x.Variants).ChildRules(variant =>
        {
            variant.RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Variant name is required.")
                .MaximumLength(256).WithMessage("Variant name cannot exceed 256 characters.");

            variant.RuleFor(v => v.Sku)
                .NotEmpty().WithMessage("Variant SKU is required.")
                .MaximumLength(250).WithMessage("Variant SKU cannot exceed 250 characters.");

            variant.RuleFor(v => v.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Variant price must be greater than or equal to 0.");

            variant.RuleFor(v => v.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Variant stock quantity must be greater than or equal to 0.");
        }).When(x => x.Variants != null);

        RuleForEach(x => x.CategoryIds)
            .GreaterThan(0).WithMessage("Category ID must be greater than 0.")
            .When(x => x.CategoryIds != null);
    }
}
